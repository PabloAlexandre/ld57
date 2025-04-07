using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System;

public class SubmarineHUD : MonoBehaviour
{
    public SubmarineStats stats;
    public Transform submarineTransform;

    [Header("UI Elements")]
    public TMP_Text goldText;
    public TMP_Text depthText;

    public Image fadeImage;

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
    private float time;
    private int target = 0;
    private float fadeOutSpeed = 0.8f;
    private Action onFadeEnd;

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
        
        if(fadeImage != null) {
            Color color = fadeImage.color;
            color.a = 1;
            fadeImage.color = color;
        }



        lastGoldAmount = stats.gold;
    }


    private void Update()
    {
        UpdateGold();
        UpdateDepth();


        if (fadeImage != null && fadeImage.color.a >= 0 && fadeImage.color.a <= 1) {
            time += Time.deltaTime;
            if ((time < 1.0f && target != 1) || (target == 1 && time < 0.7f)) return;
            Color color = fadeImage.color;
            color.a = Mathf.Lerp(color.a, target, fadeOutSpeed * Time.deltaTime);
            fadeImage.color = color;
        }
    }

    public void PlayFadeOut() {
        target = 1;
        time = 0;
        fadeOutSpeed = 5;
        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;
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
            float currentDepth = (submarineTransform.position.y/5) - Constants.Y_OFFSET;
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
