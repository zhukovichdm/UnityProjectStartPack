using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility.Toolbox;

namespace SelectionObjects
{
    public class SelectionObjectsViaMesh
    {
        public SelectionObjectsViaMesh(int meshLayer, LayerMask maskForSelection)
        {
            mainCamera = Camera.main;
            mesh = new Mesh();
            this.maskForSelection = maskForSelection;

            var selectionMesh = new GameObject("[SelectionMesh]", typeof(MeshFilter)) {layer = meshLayer};
            selectionMesh.AddComponent<Rigidbody>().isKinematic = true;
            selectionMesh.GetComponent<MeshFilter>().mesh = mesh;
            meshCollider = selectionMesh.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;
            meshCollider.isTrigger = true;
            meshCollider.sharedMesh = mesh;
            selectionMesh.AddComponent<CollisionTrigger>().selectionObjectsViaMesh = this;
            SetMesh();
        }

        public SelectionObjectsViaMesh(GUISkin skin, int meshLayer, LayerMask maskForSelection) : this(meshLayer,
            maskForSelection)
        {
            this.skin = skin;
        }

        public readonly List<GameObject> selectedObjects = new List<GameObject>(LIST_COUNT);
        public readonly MyAction selectionCompletedAction = new MyAction();
        private const string SELECTION_BUTTON = "Fire1";
        private const int LIST_COUNT = 50000;
        private readonly Camera mainCamera;
        private readonly LayerMask maskForSelection;
        private readonly MeshCollider meshCollider;
        private readonly Mesh mesh;
        private Vector2 startPosition;
        private Vector2 endPosition;
        private Vector2 boxStartPos;
        private Rect rect;
        private readonly GUISkin skin;

        private bool _canDrawing;


        /// <summary>
        /// Вызывать в OnGUI.
        /// Отрисовка зоны веделения.
        /// </summary>
        public void DrawBox()
        {
            if (Input.GetButtonDown(SELECTION_BUTTON))
            {
                _canDrawing = !EventSystem.current.IsPointerOverGameObject();
                boxStartPos = Input.mousePosition;
            }

            if (_canDrawing && Input.GetButton(SELECTION_BUTTON))
            {
                Vector2 boxEndPos = Input.mousePosition;

                if (boxStartPos == boxEndPos) return;
                rect = new Rect(Mathf.Min(boxEndPos.x, boxStartPos.x),
                    Screen.height - Mathf.Max(boxEndPos.y, boxStartPos.y),
                    Mathf.Max(boxEndPos.x, boxStartPos.x) - Mathf.Min(boxEndPos.x, boxStartPos.x),
                    Mathf.Max(boxEndPos.y, boxStartPos.y) - Mathf.Min(boxEndPos.y, boxStartPos.y)
                );

                GUI.skin = skin;
                GUI.depth = 99;
                GUI.Box(rect, "");
            }

            if (Input.GetButtonUp(SELECTION_BUTTON))
                boxStartPos = Vector2.zero;
        }

        /// <summary>
        /// Вызывать в Update.
        /// При клике по объекту происходит выделение одного объекта.
        /// При выделении нескольких объектов создание меша для их получения произойдет после того как будет отпущена кнопка выделения <see cref="SELECTION_BUTTON"/>.
        /// <remarks> После отпускания кнопки выделения, вызывается событие <see cref="selectionCompletedAction"/>, а все выделенные объекты помещаются в <see cref="selectedObjects"/>. </remarks>
        /// </summary>
        public void MultipleSelection_UppingMode()
        {
            if (_canDrawing == false) return;

            var mousePosition = Input.mousePosition;

            if (Input.GetButtonDown(SELECTION_BUTTON))
            {
                selectedObjects.Clear();
                startPosition = mousePosition;
            }

            if (Input.GetButtonUp(SELECTION_BUTTON))
            {
                endPosition = mousePosition;
                if (startPosition == endPosition)
                {
                    SingleSelection();
                }
                else if (startPosition.x != endPosition.x && startPosition.y != endPosition.y)
                {
                    UpdateMesh();
                    Toolbox.Get<ManagerUpdateComponent>().StartCoroutine(SelectionCompleted());
                }

                IEnumerator SelectionCompleted()
                {
                    yield return new WaitForFixedUpdate();
                    ResetMesh();
                    selectionCompletedAction.Publish();
                }
            }
        }

        /// <summary>
        /// Вызывать в Update.
        /// При клике по объекту происходит выделение одного объекта.
        /// При выделении нескольких объектов пересоздание меша для их получения происхоит пока нажата кнопка выделения <see cref="SELECTION_BUTTON"/>.
        /// <remarks> При каждом пересоздании меша, либо выделении одного объекта вызывается событие <see cref="selectionCompletedAction"/>, а все выделенные объекты помещаются в <see cref="selectedObjects"/>. </remarks>
        /// </summary>
        public void MultipleSelection_DraggingMode()
        {
            if (_canDrawing == false) return;

            // BUG: объект не уходит из списка выбранных, если его сначала поместить в область выделания а затем убрать (не отпуская кнопку) 
            var mousePosition = Input.mousePosition;

            if (Input.GetButtonDown(SELECTION_BUTTON))
            {
                selectedObjects.Clear();
                startPosition = mousePosition;
            }

            if (Input.GetButton(SELECTION_BUTTON))
            {
                endPosition = mousePosition;
                if (startPosition.x != endPosition.x && startPosition.y != endPosition.y)
                {
                    UpdateMesh();
                    selectionCompletedAction.Publish();
                }
            }

            if (Input.GetButtonUp(SELECTION_BUTTON))
            {
                if (startPosition == endPosition)
                    SingleSelection();
                else
                    ResetMesh();
            }
        }

        /// <summary>
        /// При клике по объекту происходит выделение одного объекта.
        /// </summary>
        public void SingleSelection()
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            // TODO: добавить маску для рейкаста как при создании PhsTool Объекта
            if (Physics.Raycast(ray, out var hit, mainCamera.farClipPlane))
                Select(hit.transform.gameObject);

            selectionCompletedAction.Publish();
        }

        public void Select(GameObject obj)
        {
            if (maskForSelection == (maskForSelection | (1 << obj.layer)))
                if (selectedObjects.Contains(obj) == false)
                    selectedObjects.Add(obj);
        }

        public void Deselect(GameObject obj)
        {
            selectedObjects.Remove(obj);
        }


        private void UpdateMesh()
        {
            const float topZ = 0.0001f;
            var lowerZ = mainCamera.farClipPlane;

            mesh.vertices = new[]
            {
                mainCamera.ScreenToWorldPoint(new Vector3(startPosition.x, startPosition.y, topZ)),
                mainCamera.ScreenToWorldPoint(new Vector3(endPosition.x, startPosition.y, topZ)),
                mainCamera.ScreenToWorldPoint(new Vector3(startPosition.x, endPosition.y, topZ)),
                mainCamera.ScreenToWorldPoint(new Vector3(endPosition.x, endPosition.y, topZ)),
                mainCamera.ScreenToWorldPoint(new Vector3(startPosition.x, startPosition.y, lowerZ)),
                mainCamera.ScreenToWorldPoint(new Vector3(endPosition.x, startPosition.y, lowerZ)),
                mainCamera.ScreenToWorldPoint(new Vector3(startPosition.x, endPosition.y, lowerZ)),
                mainCamera.ScreenToWorldPoint(new Vector3(endPosition.x, endPosition.y, lowerZ))
            };

            meshCollider.sharedMesh = mesh;
            meshCollider.enabled = true;
        }

        private void SetMesh()
        {
            mesh.vertices = new Vector3[8];
            mesh.triangles = new[] {0, 1, 2, 1, 3, 2, 4, 5, 6, 5, 7, 6, 5, 1, 7, 1, 3, 7, 0, 2, 6, 0, 6, 4};
        }

        private void ResetMesh()
        {
            meshCollider.enabled = false;

            // mesh.vertices = new Vector3[8];
            // meshCollider.sharedMesh = mesh;
        }

        public void DrawDebugLines()
        {
            foreach (var vertex in mesh.vertices)
                Debug.DrawLine(vertex, mainCamera.transform.position, Color.yellow);
        }
    }
}