using UnityEngine;

namespace Utility.Toolbox
{
    public class ManagerUpdateComponent : MonoBehaviour
    {
        private UpdateManager mng;

        public void Setup(UpdateManager mng)
        {
            this.mng = mng;
        }

        private void Update()
        {
            mng.Tick();
        }

        private void FixedUpdate()
        {
            mng.FixedTick();
        }
    }
}