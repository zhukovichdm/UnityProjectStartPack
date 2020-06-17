using UnityEngine;

namespace SelectionObjects
{
    public class CollisionTrigger : MonoBehaviour
    {
        public SelectionObjectsViaMesh selectionObjectsViaMesh;

        private void OnTriggerEnter(Collider other)
        {
            selectionObjectsViaMesh.Select(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            selectionObjectsViaMesh.Deselect(other.gameObject);
        }
    }
}