using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float followSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        // MOVIMENTO com suavidade
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // ROTACIONA para olhar diretamente para o submarino
        transform.LookAt(target.position, Vector3.up);
    }
}
