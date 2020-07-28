using System;
using System.Collections.Generic;
using Scripts.System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Behaviours
{
    public enum AnimationModes
    {
        All,
        Queue,
        Target
    }

    [Serializable]
    public struct AnimatorStruct
    {
        public AnimatorSystem animatorSystem;
        public List<AnimatorStruct> animatorStructs;
    }

    public class AnimatorControl : MonoBehaviour, IMouseOver
    {
        public bool ignoreUi = true;
        public bool useInputButton;
        public AnimationModes mode;
        public PointerEventData.InputButton inputButton;
        public List<AnimatorStruct> animatorStructs = new List<AnimatorStruct>();
        public List<AnimatorSystem> animatorSystems = new List<AnimatorSystem>();

        // QueueMode
        private int _id;
        private int _instanceId;
        private AnimationDirection _direction = AnimationDirection.ToBeginning;

        // TargetMode
        // Хранит InstanceId всех коллайдеров. (Key - InstanceId GameObject объекта на котором имеется коллайдер. Value - InstanceId GameObject родителя с аниматором)
        private readonly Dictionary<int, int> _instancesId = new Dictionary<int, int>();

//        public void ApplyPreset(Preset preset)
//        {
//            ResetAll();
//            preset.ApplyTo(this);
//            Initialize();
//        }

        private void Awake()
        {
            Initialize();
        }

        public void DeInitialize()
        {
            ResetAll();
            MouseEventsRedirection.StopRedirectFromChildObjects(this);
            animatorSystems.Clear();
            _instancesId.Clear();
        }

        public void Initialize()
        {
            MouseEventsRedirection.RedirectFromChildObjects(this);
            animatorSystems.Clear();
            _instancesId.Clear();
            GenerateList(animatorStructs);

            foreach (var animatorSystem in animatorSystems)
            {
                animatorSystem.Initialize(this);
                animatorSystem.UpdateParameters();

                foreach (var child in animatorSystem.animator.GetComponentsInChildren<Collider>())
                    _instancesId.Add(child.gameObject.GetInstanceID(),
                        animatorSystem.animator.gameObject.GetInstanceID());
            }

            ModeChanged();

            void GenerateList(List<AnimatorStruct> structs)
            {
                foreach (var item in structs)
                {
                    animatorSystems.Add(item.animatorSystem);
                    if (item.animatorStructs.Count != 0)
                        GenerateList(item.animatorStructs);
                }
            }
        }

        public void ModeChanged()
        {
            _id = 0;
            _instanceId = 0;
            _direction = AnimationDirection.ToBeginning;

            foreach (var animatorSystem in animatorSystems)
                animatorSystem.actionPlayback_End.Unsubscribe(Reverse_QueueMode_Next);

            switch (mode)
            {
                case AnimationModes.Queue:
                    foreach (var animatorSystem in animatorSystems)
                        animatorSystem.actionPlayback_End.Subscribe(Reverse_QueueMode_Next);
                    break;
            }
        }

        private void OnEnable()
        {
            foreach (var animatorSystem in animatorSystems)
                animatorSystem.OnEnable();
        }

        public void SetSignAndPlaybackAll(bool direction)
        {
            foreach (var animatorSystem in animatorSystems)
                animatorSystem.SetSignAndPlayback(direction
                    ? AnimationDirection.ToEnd
                    : AnimationDirection.ToBeginning);
        }

        public void SetSignAndPlaybackAll(AnimationDirection direction)
        {
            foreach (var animatorSystem in animatorSystems)
                animatorSystem.SetSignAndPlayback(direction);
        }

        public void ResetAll()
        {
            foreach (var animatorSystem in animatorSystems)
                animatorSystem.Reset();
        }

        public void Reverse_AllMode()
        {
            foreach (var animatorSystem in animatorSystems)
                animatorSystem.ReverseAndPlayback();
        }

        /// <summary>
        /// Реверс анимации объектов, до указанного объекта включительно.
        /// </summary>
        /// <param name="instanceId">Id GameObject'а на котором находится Animator</param>
        public void Reverse_QueueMode_ToId(int instanceId)
        {
            var systemAnimator = GetSystemAnimator(instanceId);
            if (systemAnimator == null) return;
            _direction = systemAnimator.Sign.Reverse();
            _instanceId = instanceId;
            animatorSystems[_id].SetSignAndPlayback(_direction);
        }

        /// <summary>
        /// Вызывается после того как проиграет анимация предыдущего объекта.
        /// </summary>
        private void Reverse_QueueMode_Next()
        {
            if (_id + _direction.Value() >= animatorSystems.Count || _id + _direction < 0) return;
            if (_direction == AnimationDirection.ToEnd &&
                animatorSystems[_id].animator.gameObject.GetInstanceID() == _instanceId) return;
//            if (_direction == -1 && systemAnimators[_id].animator.gameObject.GetInstanceID() == _instanceId) return;
            animatorSystems[_id + _direction.Value()].ReverseAndPlayback();
            _id += _direction.Value();
        }

        /// <param name="instanceId">Id GameObject'а на котором находится Animator</param>
        public void Reverse_TargetMode(int instanceId)
        {
            var systemAnimator = GetSystemAnimator(instanceId);
            systemAnimator?.ReverseAndPlayback();
        }

        public AnimatorSystem GetSystemAnimator(int instanceId)
        {
            if (animatorSystems.Exists(x => x.animator.gameObject.GetInstanceID() == instanceId) == false) return null;
            var selectedId = animatorSystems.FindIndex(x => x.animator.gameObject.GetInstanceID() == instanceId);
            return animatorSystems[selectedId];
        }

        private void OnMouseOver() => OnMouseOver_Redirected(gameObject);

        private static GameObject _mouseDownTarget;

        public void OnMouseOver_Redirected(GameObject target)
        {
            if (ignoreUi && EventSystem.current.IsPointerOverGameObject()) return;
//            if (Input.GetMouseButtonDown((int) inputButton) == false || useInputButton == false) return;

            if (Input.GetMouseButtonDown((int) inputButton))
                _mouseDownTarget = target;

            if (Input.GetMouseButtonUp((int) inputButton) && _mouseDownTarget == target || useInputButton == false)
            {
                _mouseDownTarget = null;

                if (_instancesId.ContainsKey(target.GetInstanceID()) == false) return;
                switch (mode)
                {
                    case AnimationModes.All:
                        Reverse_AllMode();
                        break;
                    case AnimationModes.Queue:
                        Reverse_QueueMode_ToId(_instancesId[target.GetInstanceID()]);
                        break;
                    case AnimationModes.Target:
                        Reverse_TargetMode(_instancesId[target.GetInstanceID()]);
                        break;
                }
            }
        }
    }
}