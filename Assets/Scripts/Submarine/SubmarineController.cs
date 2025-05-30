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
    public Vector2 moveInput;
    private Rigidbody rb;
    public Vector2 boundsSubmarineLight = new Vector2(80, -80); // Limites do submarino para a luz
    public bool canDie = true;
    public bool isAnimation = false;

    public ParticleSystem bubbleEffect;
    public string animName;
    private float time;
    public AudioSource clip;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if((SceneManager.GetActiveScene().name == "SampleScene" || SceneManager.GetActiveScene().name == "TutoDemo") && !FindFirstObjectByType<GameManager>().IsBackFromUpgrade()) {
            animName = "StartAnimation";
            StartCoroutine(StartAnimation());
            isAnimation = true;
        }

        GameManager manager = FindFirstObjectByType<GameManager>();
        if (PlayerPrefs.HasKey($"{manager.currentLevel}_fishs") && SceneManager.GetActiveScene().name == "SampleScene") {
            int count = PlayerPrefs.GetInt($"{manager.currentLevel}_fishs", 0);
            Debug.Log(count);
            Transform fishes = this.transform.parent.Find("FollowFishes");
            if(fishes != null) {
                fishes.position = transform.position + new Vector3(0, 0, -1f);
                for (int i = 0; i < Mathf.Min(count, 3); i++) {
                    fishes.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
       

        if (submarineLight != null && stats != null)
        {
            submarineLight.range = stats.lightDistance;
            submarineLight.intensity *= (stats.lightDistance); // Ajuste a intensidade com base na distância
        }
    }


    IEnumerator StartAnimation() {
        submarineLight.gameObject.SetActive(false);

        for(int i = 0; i < 4; i++) {
            submarineLight.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            submarineLight.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        submarineLight.gameObject.SetActive(true);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(isAnimation) {
            moveInput = Vector2.zero; // Para o movimento durante a animação
            return;
        }

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
        if(animName == "StartAnimation" && isAnimation) {
            moveInput = new Vector2(0, -1);
            if(time > 2f) {
                moveInput = Vector2.zero;
                if(time > 2.5f) {
                    animName = "";
                    isAnimation = false;
                }
            }
        }

        Vector3 force = new Vector3(moveInput.x, moveInput.y, 0f) * moveForce;
        rb.AddForce(force * stats.speed);

        // Limita a velocidade máxima
        if (rb.linearVelocity.magnitude > maxSpeed * stats.speed)
        {
            rb.linearVelocity = (rb.linearVelocity.normalized * maxSpeed)*stats.speed;
        }

        bool isMoving = moveInput.magnitude > 0.1f;
        var emission = bubbleEffect.emission;

        if (isMoving && !clip.isPlaying) {
            clip.Play();
        } else if (!isMoving && clip.isPlaying) {
            clip.Stop();
        }

        emission.enabled = isMoving;
    }


    private void Update()
    {
        time += Time.deltaTime;
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


        Vector3 pos = Camera.main.WorldToScreenPoint(submarineLightTransform.position);
        Vector3 angle = new Vector3(submarineLightTransform.rotation.x, 270, submarineLightTransform.rotation.z);
        if (pos.x < Mouse.current.position.ReadValue().x) {
            angle.y = 90;
        }

        // Limita a rotação da luz dentro dos limites definidos
        float inputPercentage = Mathf.Min(Mathf.Max(0, Mouse.current.position.ReadValue().y), Screen.height) / Screen.height;
        angle.x = Mathf.Lerp(boundsSubmarineLight.x, boundsSubmarineLight.y, inputPercentage);
        submarineLightTransform.rotation = Quaternion.Slerp(submarineLightTransform.rotation, Quaternion.Euler(angle), lightRotationSpeed * Time.deltaTime);
    }

    private float extractSpeed = 10f;
    private float extractDuration = 1.5f;
    [SerializeField] private string upgradeSceneName = "UpgradeScene";

    private bool isBeingExtracted = false;

    public void OnExtract(InputAction.CallbackContext context)
    {
        if (context.performed && !isBeingExtracted)
        {
            if(SceneManager.GetActiveScene().name == "TutoDemo") {
                if (PlayerPrefs.GetInt("no_tutorial", 0) == 0) {
                    PlayerPrefs.SetInt("tutorial", 1);
                }
            }
        
            PlayerPrefs.SetString("last_scene", SceneManager.GetActiveScene().name);

            FindFirstObjectByType<GameManager>().SavePosition();
            FindAnyObjectByType<SubmarineHUD>().PlayFadeOut();
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
            float percent = extractTimer / (extractDuration - 1f);
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
        if (isDead || !canDie) return;
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
        yield return new WaitForSeconds(0.3f);
        FindAnyObjectByType<SubmarineHUD>().PlayFadeOut();
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("OceanScene");

        // Aqui você pode:
        // Destroy(gameObject);
        // ou chamar GameManager.GameOver();
        // ou deixar o corpo implodido na cena
    }
}
