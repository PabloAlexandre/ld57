using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;


public class SubmarineController : MonoBehaviour
{
    public float moveForce = 10f; // Força aplicada ao movimento
    public Transform submarineLightTransform;
    public float lightRotationSpeed = 10f;
    public float maxSpeed = 5f;
    private float targetYRotation = 0f;
    public float tiltAngle = 15f; // ângulo máximo de inclinação
    private float targetZRotation = 0f; // inclinação visual
    public float tiltRotationSpeed = 1f; // suavidade do tilt (Z)
    public float horizontalRotationSpeed = 2f; // suavidade do giro horizontal (Y)
    private bool isDead = false;
    public Light submarineLight; // referência à SpotLight

    public SubmarineStats stats;
    private Vector2 moveInput;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (submarineLight != null && stats != null)
        {
            submarineLight.range = stats.lightDistance;
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // Define a rotação alvo com base na direção X
        if (moveInput.x < -0.1f)
            targetYRotation = -180f;
        else if (moveInput.x > 0.1f)
            targetYRotation = 0f;

        // Inclinação vertical (Z)
        targetZRotation = moveInput.y * tiltAngle;
    }



    private void FixedUpdate()
    {
        Vector3 force = new Vector3(moveInput.x, moveInput.y, 0f) * moveForce;
        rb.AddForce(force * stats.speed);

        // Limita a velocidade máxima
        if (rb.linearVelocity.magnitude > maxSpeed * stats.speed)
        {
            rb.linearVelocity = (rb.linearVelocity.normalized * maxSpeed)*stats.speed;
        }
    }


    private void Update()
    {
        RotateSubmarine();
        if (!isBeingExtracted) // ← se estiver usando a extração
        {
            RotateLightTowardMousePerspective();
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    private void RotateSubmarine()
    {
        // Obtem a rotação atual em Euler
        Vector3 currentEuler = transform.rotation.eulerAngles;

        // Corrige o valor do eixo Z para lidar com wrap (360° -> -180° etc.)
        float currentZ = NormalizeAngle(currentEuler.z);
        float newZ = Mathf.LerpAngle(currentZ, targetZRotation, tiltRotationSpeed * Time.deltaTime * stats.speed);

        // Mesma coisa pro eixo Y (horizontal flip)
        float currentY = NormalizeAngle(currentEuler.y);
        float newY = Mathf.LerpAngle(currentY, targetYRotation, horizontalRotationSpeed * Time.deltaTime * stats.speed);

        // Aplica nova rotação com X fixo em 0
        transform.rotation = Quaternion.Euler(0f, newY, newZ);
    }

    private void RotateLightTowardMousePerspective()
    {
        if (submarineLightTransform == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, submarineLightTransform.position.z));

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            Vector3 direction = point - submarineLightTransform.position;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                submarineLightTransform.rotation = Quaternion.Slerp(submarineLightTransform.rotation, targetRotation, lightRotationSpeed * Time.deltaTime);
            }
        }
    }

    [SerializeField] private float extractSpeed = 10f;
    [SerializeField] private float extractDuration = 0.5f;
    [SerializeField] private string upgradeSceneName = "UpgradeScene";

    private bool isBeingExtracted = false;

    public void OnExtract(InputAction.CallbackContext context)
    {
        if (context.performed && !isBeingExtracted)
        {
            StartCoroutine(ExtractAndLoadUpgradeScene());
        }
    }

    private IEnumerator ExtractAndLoadUpgradeScene()
    {
        isBeingExtracted = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false; // libera física para torque
        }

        Vector3 originalPosition = transform.position;
        float anticipationDuration = 0.3f;
        float anticipationDepth = 0.2f;
        float anticipationTorqueZ = 15f; // valor ajustável (negativo = nariz pra cima)

        // 🔻 Etapa 1: Antecipação com torque + leve descida
        float t = 0f;
        while (t < anticipationDuration)
        {
            if (rb != null)
            {
                rb.AddTorque(Vector3.forward * anticipationTorqueZ);
            }

            transform.position = Vector3.Lerp(originalPosition, originalPosition - Vector3.up * anticipationDepth, t / anticipationDuration);
            t += Time.deltaTime;
            yield return null;
        }

        // 🛑 Congela física para controlar a subida diretamente
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // 🔺 Etapa 2: Subida com aceleração + rotação manual
        float extractTimer = 0f;
        float tiltZ = -30f; // nariz pra baixo
        float maxSpeed = extractSpeed;
        float currentSpeed = 0f;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, startRotation.eulerAngles.y, tiltZ);

        while (extractTimer < extractDuration)
        {
            float percent = extractTimer / extractDuration;
            currentSpeed = Mathf.Lerp(0f, maxSpeed, percent);
            transform.position += Vector3.up * currentSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, percent);

            extractTimer += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(upgradeSceneName);
    }





    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Para o movimento
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // impede forças futuras

        // Inicia animação de encolher
        StartCoroutine(ShrinkAndDie());
    }

    private IEnumerator ShrinkAndDie()
    {
        float duration = 0.1f;
        float elapsed = 0f;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;

        // Aqui você pode:
        // Destroy(gameObject);
        // ou chamar GameManager.GameOver();
        // ou deixar o corpo implodido na cena
    }
}
