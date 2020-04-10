using UnityEngine;

public class TextLookAt : MonoBehaviour
{
    [SerializeField] private Transform camera;

    private void Start()
    {
        if (camera == null)
            camera = Camera.main.transform;
    }

    private void Update()
    {
        transform.forward = camera.forward;
    }
}
