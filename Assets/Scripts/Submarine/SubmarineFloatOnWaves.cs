using UnityEngine;

public class SubmarineFloatOnWaves : MonoBehaviour
{
    [Header("Parâmetros da Onda (copiados do Shader)")]
    public float waveScale = 0.5f;       // equivale a _wave_scale
    public float waveSpeed = 1.0f;       // equivale a _wave_speed
    public float waveTile = 1.0f;        // equivale a _wave_tile
    public float waveStrength = 1.0f;    // equivale a _wave_strength

    [Header("Ajuste da posição")]
    public float baseHeight = 0f;
    public float verticalOffset = 0f;

    [Header("Inclinação")]
    public bool applyTilt = true;
    public float tiltAmount = 15f;

    private void Start()
    {
        baseHeight = transform.position.y;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        float t = Time.time * waveSpeed;

        // Simula a altura da onda com base na posição XZ
        float waveX = Mathf.Sin((pos.x + t) * waveTile);
        float waveZ = Mathf.Cos((pos.z + t) * waveTile);
        float height = (waveX + waveZ) * 0.5f * waveScale * waveStrength;

        // Aplica altura ao submarino
        pos.y = baseHeight + height + verticalOffset;
        transform.position = pos;

        // Aplica inclinação para dar efeito de balanço
        if (applyTilt)
        {
            float pitch = Mathf.Cos((pos.z + t) * waveTile) * tiltAmount;
            float roll = Mathf.Sin((pos.x + t) * waveTile) * tiltAmount;
            transform.rotation = Quaternion.Euler(pitch, transform.rotation.eulerAngles.y, roll);
        }
    }
}
