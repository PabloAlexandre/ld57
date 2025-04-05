using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Alinha a rotação com a câmera (apenas em Y se quiser travar)
            transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }
    }
}
