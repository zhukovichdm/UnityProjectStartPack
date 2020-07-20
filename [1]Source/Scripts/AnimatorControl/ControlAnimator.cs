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

    public class ControlAnimator : MonoBehaviour, IMouseOver
    {
        public bool useInputButton;
        public AnimationModes mode;
        public PointerEventData.InputButton inputButton;
        public List<SystemAnimator> systemAnimators = new List<SystemAnimator>();

        // QueueMode
        private int _id;
        private int _instanceId;
        private AnimationDirection _direction = AnimationDirection.ToBeginning;

        // TargetMode
        // Хранит InstanceId всех коллайдеров. (Key - InstanceId GameObject объекта на котором имеется коллайдер. Value - InstanceId GameObject родителя с аниматором)
        private readonly Dictionary<int, int> _instancesId = new Dictionary<int, int>();

        private void Awake()
        {
            MouseEventsRedirection.RedirectFromChildObjects(this);

            foreach (var systemAnimator in systemAnimators)
            {
                systemAnimator.Initialize(this);
                systemAnimator.UpdateParameters();
            }
        }

        public void Start()
        {
            _id = 0;
            _instanceId = 0;
            _direction = AnimationDirection.ToBeginning;
            _instancesId.Clear();

            switch (mode)
            {
                case AnimationModes.Queue:
                    foreach (var systemAnimator in systemAnimators)
                        systemAnimator.actionPlayback_End.Subscribe(Reverse_QueueMode_Next);
                    break;
                default:
                    foreach (var systemAnimator in systemAnimators)
                        systemAnimator.actionPlayback_End.Unsubscribe(Reverse_QueueMode_Next);
                    break;
            }

            foreach (var systemAnimator in systemAnimators)
            foreach (var child in systemAnimator.animator.GetComponentsInChildren<Collider>())
                _instancesId.Add(child.gameObject.GetInstanceID(), systemAnimator.animator.gameObject.GetInstanceID());
        }

        private void OnEnable()
        {
            foreach (var systemAnimator in systemAnimators)
                systemAnimator.OnEnable();
        }

        public void SetSignAndPlaybackAll(bool direction)
        {
            foreach (var systemAnimator in systemAnimators)
                systemAnimator.SetSignAndPlayback(direction
                    ? AnimationDirection.ToEnd
                    : AnimationDirection.ToBeginning);
        }

        public void SetSignAndPlaybackAll(AnimationDirection direction)
        {
            foreach (var systemAnimator in systemAnimators)
                systemAnimator.SetSignAndPlayback(direction);
        }

        public void Reverse_AllMode()
        {
            foreach (var systemAnimator in systemAnimators)
                systemAnimator.ReverseAndPlayback();
        }

        /// <summary>
        /// Реверс анимации объектов, до указанного объекта включительно.
        /// </summary>
        /// <param name="instanceId">Id GameObject'а на котором находится Animator</param>
        public void Reverse_QueueMode_ToId(int instanceId)
        {
            if (systemAnimators.Exists(x => x.animator.gameObject.GetInstanceID() == instanceId) == false) return;
            var selectedId = systemAnimators.FindIndex(x => x.animator.gameObject.GetInstanceID() == instanceId);
            _direction = systemAnimators[selectedId].Sign.Reverse();
            _instanceId = instanceId;
            systemAnimators[_id].SetSignAndPlayback(_direction);
        }

        /// <summary>
        /// Вызывается после того как проиграет анимация предыдущего объекта.
        /// </summary>
        private void Reverse_QueueMode_Next()
        {
            if (_id + _direction.Value() >= systemAnimators.Count || _id + _direction < 0) return;
            if (_direction == AnimationDirection.ToEnd &&
                systemAnimators[_id].animator.gameObject.GetInstanceID() == _instanceId) return;
//            if (_direction == -1 && systemAnimators[_id].animator.gameObject.GetInstanceID() == _instanceId) return;
            systemAnimators[_id + _direction.Value()].ReverseAndPlayback();
            _id += _direction.Value();
        }

        /// <param name="instanceId">Id GameObject'а на котором находится Animator</param>
        public void Reverse_TargetMode(int instanceId)
        {
            if (systemAnimators.Exists(x => x.animator.gameObject.GetInstanceID() == instanceId) == false) return;
            var selectedId = systemAnimators.FindIndex(x => x.animator.gameObject.GetInstanceID() == instanceId);
            systemAnimators[selectedId].ReverseAndPlayback();
        }

        private void OnMouseOver() => OnMouseOver_Redirected(gameObject);

        public void OnMouseOver_Redirected(GameObject target)
        {
            if (Input.GetMouseButtonDown((int) inputButton) == false || useInputButton == false) return;
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