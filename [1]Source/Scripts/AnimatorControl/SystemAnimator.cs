using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.System
{
    public enum AnimationDirection
    {
        ToEnd = 1,
        ToBeginning = -1
    }

    [Serializable]
    public class SystemAnimator
    {
        public MonoBehaviour mono;
        public Animator animator;
        public List<string> allAnimatorParameters = new List<string>();
        public float frameRate = 1;
        public int selectedParameter = 0;
        public bool playFromStart;
        public bool isPlayback;
        private Coroutine _coroutine;

        // INFO:  [field: SerializeField] требуется что бы значение не сбрасывалось редактором в ноль.
        [field: SerializeField] public float State { get; private set; } // От 0 до 1
        [field: SerializeField] public AnimationDirection Sign { get; private set; } // 1 либо -1

        public void Initialize(MonoBehaviour monoBehaviour)
        {
            mono = monoBehaviour;
        }

        public void OnEnable()
        {
            if (playFromStart && animator)
                Playback();
        }

        public void Playback()
        {
            if (isPlayback == false && allAnimatorParameters.Count > selectedParameter)
                _coroutine = mono.StartCoroutine(Playback());

            IEnumerator Playback()
            {
                isPlayback = true;
                const float tolerance = 0f;
                while (isPlayback && (
                           Math.Abs(State) > tolerance && Sign == AnimationDirection.ToBeginning ||
                           Math.Abs(State - 1f) > tolerance && Sign == AnimationDirection.ToEnd))
                {
                    var step = State + Time.deltaTime * (int) Sign * frameRate;
                    State = Mathf.Clamp(step, 0, 1f);
                    animator.SetFloat(allAnimatorParameters[selectedParameter], State);
                    yield return null;
                }

                animator.SetFloat(allAnimatorParameters[selectedParameter], State);

                isPlayback = false;
            }
        }

        public void ReverseAndPlayback()
        {
            Reverse();
            Playback();
        }

        public void Reverse()
        {
            Sign = (AnimationDirection) ((int) Sign * -1);
        }

        public void SetSignAndPlayback(AnimationDirection direction)
        {
            SetSign(direction);
            Playback();
        }

        public void SetSign(AnimationDirection direction)
        {
            Sign = direction;
        }

        public void SetState(float value)
        {
            animator.SetFloat(allAnimatorParameters[selectedParameter], value);
            State = value;
        }

        public void Break()
        {
            if (_coroutine != null) mono.StopCoroutine(_coroutine);
            isPlayback = false;
            State = 0;
            Sign = AnimationDirection.ToEnd;
        }

        public void UpdateParameters()
        {
            allAnimatorParameters.Clear();
            if (animator == false) return;
            foreach (var parameter in animator.parameters)
                allAnimatorParameters.Add(parameter.name);
        }
    }
}