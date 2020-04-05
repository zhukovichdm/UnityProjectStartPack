using System.Collections;
using UnityEngine;

namespace Scripts.System
{
    public static class DoubleClick
    {
        private const float CLICK_TIME = 0.2f;
        private static float _time;
        private static int _count;
        private static bool _flag;

        public static bool Check(MonoBehaviour monoBehaviour, int clickCount, int inputButton)
        {
            if (Input.GetMouseButtonDown(inputButton))
            {
                _count++;
                if (!_flag) monoBehaviour.StartCoroutine(Execute());
                return clickCount == _count;
            }

            return false;
        }

        private static IEnumerator Execute()
        {
            _flag = true;
            while (_time < CLICK_TIME)
            {
                yield return new WaitForEndOfFrame();
                _time += Time.deltaTime;
            }

            _time = 0;
            _count = 0;
            _flag = false;
        }
    }
}