using UnityEngine;
using TMPro;
using System.Collections;

public class SubmarineHUD : MonoBehaviour
{
    public SubmarineStats stats;
    public Transform submarineTransform;

    [Header("UI Elements")]
    public TMP_Text goldText;
    public TMP_Text depthText;

    [Header("Flash & Pop")]
    public Color flashColor = Color.yellow;
    public float flashDuration = 0.3f;
    public float popScale = 1.3f;
    public float popSpeed = 10f;

    [Header("Pop Sound")]
    public AudioClip popSound;
    public float popVolume = 0.6f;
    private AudioSource audioSource;

    [Header("Pop Sync Timing")]
    public float visualDelay = 0.1f; // delay antes do flash+pop

    private float lastGoldAmount;
    private Color originalGoldColor;
    private Vector3 originalScale;

    private void Start()
    {
        if (goldText != null)
        {
            originalGoldColor = goldText.color;
            originalScale = goldText.transform.localScale;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        lastGoldAmount = stats.gold;
    }

    private void Update()
    {
        UpdateGold();
        UpdateDepth();
    }

    void UpdateGold()
    {
        goldText.text = $"Gold: {stats.gold}";

        if (stats.gold > lastGoldAmount)
        {
            StartCoroutine(FlashAndPopGold());
        }

        lastGoldAmount = stats.gold;
    }

    void UpdateDepth()
    {
        if (submarineTransform != null && stats != null)
        {
            float currentDepth = submarineTransform.position.y;
            float maxDepth = stats.maxDepth;

            depthText.text = $"Depth: {currentDepth:0.0} / {maxDepth}m";
        }
    }


    IEnumerator FlashAndPopGold()
    {
        // 🔊 Toca som do pop primeiro
        if (popSound != null && audioSource != null)
            audioSource.PlayOneShot(popSound, popVolume);

        // 🕐 Espera antes de mostrar o efeito visual (pra sincronizar com o pico do som)
        yield return new WaitForSeconds(visualDelay);

        // Pop visual
        goldText.color = flashColor;
        goldText.transform.localScale = originalScale * popScale;

        float timer = 0f;
        while (timer < flashDuration)
        {
            goldText.transform.localScale = Vector3.Lerp(
                goldText.transform.localScale,
                originalScale,
                timer / flashDuration * popSpeed
            );
            timer += Time.deltaTime;
            yield return null;
        }

        goldText.transform.localScale = originalScale;
        goldText.color = originalGoldColor;
    }

}
