using System.Collections;
using Scripts.Component;
using Scripts.Component.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Modules.CameraPlayer
{
    [global::System.Serializable]
    public class ControlCamera : MonoBehaviour
    {
        [field:SerializeField] public bool ignoreEscape { get; set; }
        public bool StopControling { get; set; } = true;

        private readonly CameraState _targetCameraState = new CameraState();
        private readonly CameraState _interpolatingCameraState = new CameraState();

        [SerializeField] private bool notAcceptNewParameters;
        [SerializeField] private bool ignoreLookAtAction;
        
        [SerializeField] private string rotationButton = "Fire2";
        
        [SerializeField] private Transform mainCamera;
        [SerializeField] private Transform preposition;

        [Header("Main parameters")] [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)] [SerializeField]
        private float positionLerpTime = 0.2f;

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)] [SerializeField]
        private float rotationLerpTime = 0.01f;

        [SerializeField] private AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));
        [SerializeField] private CameraModes cameraMode;
        public CameraModes CameraMode => cameraMode;
        public bool invertY;

        [Header("PlayerMode")] [SerializeField]
        private float speed = 6;

        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private Transform head;

        [Header("FreeMode")] [SerializeField] private float freeModeBoost = 3f;
        [Header("PivotMode")] [SerializeField] private float pivotModeBoost = 0.4f;
        [Range(0.001f, 1f)] [SerializeField] private float boostCameraRotation = 0.5f;
        [Range(0.001f, 1f)] [SerializeField] private float boostCameraSpeed = 0.5f;
        [Header("LookAtOnObject")] private Transform lookAtObject;
        [Header("Scroll")] [SerializeField] private DataScrollParameter dataScrollParameter;
        [Header("Rotation limitation")] [SerializeField] private DataLimitationParameter dataLimitationParameter;

        // Для востановления параметров камеры при переключении между режимами.
        private (CameraModes, DataScrollParameter, DataLimitationParameter)? bufferParameters;
        
        private bool needJerkingCorrection = true;
        private Vector3 bufferPlayerRotation;
        
        private Vector3 GetInputTranslationDirection()
        {
            var direction = new Vector3();
            if (Input.GetKey(KeyCode.W) || (cameraMode == CameraModes.Pivot && Input.GetAxis("Mouse ScrollWheel") < 0))
                direction += Vector3.forward;
            if (Input.GetKey(KeyCode.S) || (cameraMode == CameraModes.Pivot && Input.GetAxis("Mouse ScrollWheel") > 0))
                direction += Vector3.back;
            if (Input.GetKey(KeyCode.A))
                direction += Vector3.left;
            if (Input.GetKey(KeyCode.D))
                direction += Vector3.right;
            if (Input.GetKey(KeyCode.Q))
                direction += Vector3.down;
            if (Input.GetKey(KeyCode.E))
                direction += Vector3.up;
            return direction;
        }

        private void LookAt_Subscriber((GameObject target, CameraModes cameraMode, DataScrollParameter scrollParameter, DataLimitationParameter limitationParameter, bool cameraToMaxDistance) newParameters)
        {
            if(notAcceptNewParameters == false)
            {
                if (bufferParameters == null)
                {
                    bufferPlayerRotation = transform.rotation.eulerAngles;

                    // TODO: Исправить.
                    // HACK: Фикс вращения камеры при возвращении камеры в режим player из pivot. (Если выбрать объект выше горизонта и нажать ESC, камера делала оборот на 360).
                    if (bufferPlayerRotation.x > 300)
                        bufferPlayerRotation.x = 360 - bufferPlayerRotation.x ;

                    bufferParameters = (cameraMode, dataScrollParameter, dataLimitationParameter);
                }

                // В режиме LooksAt перемещается mainCamera а не preposition, по этому при переходе в Pivot режим нужно сбросить вращение камеры. 
                if (cameraMode == CameraModes.LooksAt && newParameters.Item2 == CameraModes.Pivot)
                    StartCoroutine(SmoothCameraRotationToPreposition());

                if (newParameters.Item2 == CameraModes.LooksAt)
                {
                    cameraMode = CameraModes.LooksAt;
                }
                else
                {
                    cameraMode = newParameters.Item2;
                    dataScrollParameter = newParameters.Item3;
                    dataLimitationParameter = newParameters.Item4;
                }
            }

            LookAt_Subscriber(newParameters.target, newParameters.cameraToMaxDistance);
        }

        private void LookAt_Subscriber(GameObject target, bool cameraToMaxDistance = false)
        {
            if (rigidbody) rigidbody.velocity = Vector3.zero;

            if (cameraMode == CameraModes.Pivot) // Задаем новую позицию всего объекта.
            {
                if(needJerkingCorrection)
                {
                    JerkingCorrection(head);
                    needJerkingCorrection = false;
                }
                _targetCameraState.SetPositionFromTransform(target.transform);

                if (cameraToMaxDistance)
                    preposition.localPosition = new Vector3(0, 0, dataScrollParameter.maxDistance);
            }
            else // Задаем объект к которому стремится камера.
            {
                lookAtObject = target ? target.transform : preposition;

            }
        }

        private void Awake()
        {
            //ignoreEscape = cameraMode == CameraModes.Pivot;
            JerkingCorrection(head);

            if (!ignoreLookAtAction)
                GameActions.LookAt.Subscribe(LookAt_Subscriber);
            UserInput.InputEscapeAction.Subscribe(InputEscape_Subscriber);
        }

        private void Start()
        {
            // HACK: 
            if (cameraMode == CameraModes.Pivot)
                LookAt_Subscriber((head.gameObject, CameraModes.Pivot, dataScrollParameter, dataLimitationParameter, true));
        }

        private void OnDestroy()
        {
            if (!ignoreLookAtAction)
                GameActions.LookAt.Unsubscribe(LookAt_Subscriber);
            UserInput.InputEscapeAction.Unsubscribe(InputEscape_Subscriber);
        }

        private void OnDisable()
        {
            if(rigidbody)
                rigidbody.velocity = Vector3.zero;
        }

        private void OnEnable()
        {
            LookAt_Subscriber(preposition.gameObject);
            _targetCameraState.SetFromTransform(transform);
            _interpolatingCameraState.SetFromTransform(transform);
        }

        /// <summary>
        /// Т.к. внутри SystemCameraStat'ов остается позиция (p1) последнего выбранного объекта,
        /// делаем сброс этой позиции на позицию из Origin (p0), что бы при следующем выборе камера летела из p0 а не p1.
        /// </summary>
        private void JerkingCorrection(Transform origin)
        {
            if (!origin) return;
            _targetCameraState.SetPositionFromTransform(origin);
            _interpolatingCameraState.SetPositionFromTransform(origin);
        }

        private float positionLerpPct;
        private float rotationLerpPct;

        private void InputEscape_Subscriber()
        {
            if (!StopControling) return;
            
            if (!ignoreEscape && Testing.TestingMode == false)
            {
                if (bufferParameters != null)
                {
                    // HACK: Не знаю на сколько это правильно. Когда персонаж в режиме Pivot, не получалось сбросить камеру в стандартный пивот, по этому кидаю псевдо событие с этим пивотом вместо таргета.
                    // **************************************
                    if (bufferParameters.Value.Item1 == CameraModes.Pivot)
                    {
                        LookAt_Subscriber((head.gameObject, CameraModes.Pivot, bufferParameters.Value.Item2,bufferParameters.Value.Item3, true));
                        return;
                    }
                    // **************************************
                    
                    if (cameraMode == CameraModes.Pivot)
                    {
                        if (bufferParameters.Value.Item1 == CameraModes.Player)
                            JerkingCorrection(head);

                        if (bufferParameters.Value.Item1 == CameraModes.Free)
                        {
                            // TODO: для свободного режима сбрасывать SystemCameraStat'ы на позицию до выбора объекта.
                        }
                    }

                    cameraMode = bufferParameters.Value.Item1;
                    dataScrollParameter = bufferParameters.Value.Item2;
                    dataLimitationParameter = bufferParameters.Value.Item3;
                    bufferParameters = null;
                    ResetPosition();

                    needJerkingCorrection = true;
                    _targetCameraState.SetRotation(bufferPlayerRotation);
                    _interpolatingCameraState.SetRotation(bufferPlayerRotation);
                }
                
                LookAt_Subscriber(preposition.gameObject);
            }
        }

        private void Update()
        {
            if (!StopControling) return;

            positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
            rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);

            // Если при быстром вращении проседает фпс или зацикливается.
//            var positionLerpPct = 1f-Time.deltaTime;
//            var rotationLerpPct = 1f-Time.deltaTime;

            switch (cameraMode)
            {
                case CameraModes.LooksAt:
                    LookAtOnObject();
                    break;
                case CameraModes.Player:
                    PlayerMode();
                    LookAtOnObject();
                    break;
                case CameraModes.Free:
                    FreeMode();
                    LookAtOnObject();
                    break;
                case CameraModes.Pivot:
                    PivotMode();
                    Scroll();
                    break;
            }
        }

        
        [ContextMenu("ResetPosition")]
        private void ResetPosition()
        {
            switch (cameraMode)
            {
                case CameraModes.Player:
                    preposition.transform.localPosition = Vector3.zero;
                    StartCoroutine(SmoothReturnToHead());
                    // transform.localPosition = head.transform.localPosition;
                    break;
                case CameraModes.LooksAt:
                    break;
                case CameraModes.Pivot:
                    StartCoroutine(SmoothReturnToHead());
                    break;
            }
        }

       private IEnumerator SmoothReturnToHead()
       {
            while (true)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, head.localPosition, positionLerpPct);
                yield return new WaitForEndOfFrame();
                if (transform.localPosition == head.localPosition)
                    break;
            }
       }

       private IEnumerator SmoothCameraRotationToPreposition()
       {
           while (true)
           {
               mainCamera.rotation = Quaternion.Lerp(mainCamera.rotation, preposition.rotation, rotationLerpPct * boostCameraRotation);
               yield return new WaitForEndOfFrame();
               if (mainCamera.localRotation == preposition.localRotation) 
                   break;
           }
       }
       
        [ContextMenu("AutoCreatePlayer")]
        private void AutoCreatePlayer()
        {
            cameraMode = CameraModes.Player;
        
            var mainCamera = new GameObject();
            mainCamera.transform.parent = transform;
            mainCamera.transform.localPosition = Vector3.zero;
            mainCamera.name = "Main Camera";
            mainCamera.tag = "MainCamera";
            mainCamera.AddComponent<Camera>();
            mainCamera.AddComponent<PhysicsRaycaster>();
            this.mainCamera = mainCamera.transform;

            var preposition = new GameObject();
            preposition.transform.parent = transform;
            preposition.transform.localPosition = Vector3.zero;
            preposition.name = "Preposition";
            this.preposition = preposition.transform;

            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.transform.parent = null;
            player.transform.position = new Vector3(0, 1, 0);
            player.name = "Player";
            var rb = player.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.useGravity = false;
            rigidbody = rb;
            
            var head = new GameObject();
            head.transform.parent = player.transform;
            head.transform.localPosition = new Vector3(0, 0.8f, 0);
            head.name = "Head";
            this.head = head.transform;
            
            transform.parent = player.transform;
            transform.localPosition = head.transform.localPosition;
            transform.name = "CameraController";
        }
        

        private void FreeMode()
        {
            if (lookAtObject != preposition) return;

            var translation = GetInputTranslationDirection() * Time.deltaTime;
            translation *= Mathf.Pow(2.0f, freeModeBoost);
            _targetCameraState.Translate(translation);
            _interpolatingCameraState.LerpTowards(_targetCameraState, positionLerpPct, rotationLerpPct);
            _interpolatingCameraState.UpdateTransform(transform);
            Rotation();
        }

        private void PlayerMode()
        {
            if (lookAtObject != preposition)
            {
                rigidbody.velocity = Vector3.zero;
                return;
            }

            var x = Input.GetAxis("Horizontal") * speed;
            var z = Input.GetAxis("Vertical") * speed;
            var y = -0.5f;

            var velocity = transform.forward * z + transform.right * x;
            velocity.y = 0;
            rigidbody.velocity = velocity + rigidbody.transform.up * y;
            _interpolatingCameraState.LerpTowardsAngle(_targetCameraState, positionLerpPct, rotationLerpPct);
            _interpolatingCameraState.UpdateTransformAngle(transform);
            Rotation();
        }

        private void PivotMode()
        {
            _interpolatingCameraState.LerpTowards(_targetCameraState, positionLerpPct * pivotModeBoost,
                rotationLerpPct);
            _interpolatingCameraState.UpdateTransform(transform);
            Rotation();
        }

        private void Rotation()
        {
//            print(targetSystemCameraState.Pitch +"  "+ (targetSystemCameraState.Pitch-360) );
            if (Cursor.lockState != CursorLockMode.Locked)
                if (!Input.GetButton(rotationButton))
                    return;
            var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));
            var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);
            _targetCameraState.Yaw += mouseMovement.x * mouseSensitivityFactor;
            _targetCameraState.Pitch += mouseMovement.y * mouseSensitivityFactor;
            var value = dataLimitationParameter.Limit(new Vector2(_targetCameraState.Pitch, _targetCameraState.Yaw));
            _targetCameraState.Pitch = value.x;
            _targetCameraState.Yaw = value.y;
        }

        private void LookAtOnObject()
        {
            mainCamera.rotation = Quaternion.Lerp(mainCamera.rotation, lookAtObject.rotation, rotationLerpPct * boostCameraRotation);
            mainCamera.position = Vector3.Lerp(mainCamera.position, lookAtObject.position, positionLerpPct * boostCameraSpeed);
        }

        private void Scroll()
        {
            var axis = UserInput.InputScrollWheel;

            if (dataScrollParameter.scrollCollisionDetect && Physics.Linecast(transform.position, preposition.position, out var hit))
            {
                mainCamera.position = Vector3.Lerp(mainCamera.position, hit.point,
                    Time.fixedDeltaTime * dataScrollParameter.autoScrollSensitivity);
                if (Mathf.Abs(axis) > 0)
                    preposition.position = hit.point;
            }
            else
                mainCamera.position = Vector3.Lerp(mainCamera.position, preposition.position,
                    Time.fixedDeltaTime * dataScrollParameter.autoScrollSensitivity);

            if (Mathf.Abs(axis) > 0)
                preposition.localPosition = new Vector3(0, 0,
                    Mathf.Clamp(preposition.localPosition.z + axis * dataScrollParameter.scrollSensitivity, dataScrollParameter.maxDistance,
                        dataScrollParameter.minDistance));
        }
    }
}