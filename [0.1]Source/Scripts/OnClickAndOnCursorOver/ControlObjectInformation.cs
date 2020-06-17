using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Data
{
    /// <summary>
    /// Информация об объекте.
    /// </summary>
    public class ControlObjectInformation : MonoBehaviour
    {
        [SerializeField] private bool thisMesh;
        public DataObjectInformation dataObjectInformation;

        public List<MeshRenderer> meshRenderers;

        private void Awake()
        {
            if (!thisMesh) return;
            var meshRenderer = GetComponent<MeshRenderer>();
            if (!meshRenderers.Contains(meshRenderer))
                meshRenderers.Add(meshRenderer);
        }

        private void Start()
        {
            foreach (var meshRenderer in meshRenderers)
                meshRenderer.material.EnableKeyword("_EMISSION");
        }

        [ContextMenu("FindAllMeshRenderer")]
        private void FindAllMeshRenderer()
        {
            meshRenderers.Clear();
            List<Transform> list = new List<Transform>();
            list.Add(transform);
            int i = 0;
            while (i < list.Count)
            {
                var meshRenderer = list[i].GetComponent<MeshRenderer>();
                if (meshRenderer)
                    meshRenderers.Add(meshRenderer);
                if (list[i].childCount > 0)
                    for (int j = 0; j < list[i].childCount; j++)
                        list.Add(list[i].GetChild(j));
                i++;
            }
        }
    }
}