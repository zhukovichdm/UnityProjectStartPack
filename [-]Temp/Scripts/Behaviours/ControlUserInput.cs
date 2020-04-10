using Scripts.Component;
using UnityEngine;

namespace Scripts.Behaviours
{
    public class ControlUserInput : MonoBehaviour
    {
        private void Update()
        {
            UserInput.InputEscape = Input.GetKeyDown(KeyCode.Escape);
            if (UserInput.InputEscape)
                UserInput.InputEscapeAction.Publish();

            UserInput.InputMouseLeft = Input.GetMouseButton(0);
            UserInput.InputMouseRight = Input.GetMouseButton(1);
            UserInput.InputScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        }

        public void InputEscape() => UserInput.InputEscapeAction.Publish();
    }
}