using UnityEngine;

public class FloatingArrow : MonoBehaviour
{
    [Header("Movimento Vertical")]
    public float floatAmplitude = 0.5f;     // Altura do movimento
    public float floatSpeed = 1f;           // Velocidade do movimento

    [Header("Rotação (opcional)")]
    public bool rotate = false;
    public float rotationSpeed = 90f;       // Graus por segundo

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = startPosition + new Vector3(0f, offset, 0f);

        if (rotate)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}
