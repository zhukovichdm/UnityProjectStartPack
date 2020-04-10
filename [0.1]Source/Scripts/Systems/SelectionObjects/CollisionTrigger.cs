using UnityEngine;

namespace SelectionObjects
{
    public class CollisionTrigger : MonoBehaviour
    {
        public SelectionObjectsViaMesh selectionObjectsViaMesh;

        private void OnTriggerEnter(Collider other)
        {
            selectionObjectsViaMesh.selectedObjects.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            selectionObjectsViaMesh.selectedObjects.Remove(other.gameObject);
        }
    }
}