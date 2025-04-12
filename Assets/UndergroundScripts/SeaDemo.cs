using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SeaDemo : MonoBehaviour {
    public Volume globalVolume;

    [Header("Center X Range")]
    public float minX = 0.45f;
    public float maxX = 0.55f;

    [Header("Center Y Range")]
    public float minY = 0.45f;
    public float maxY = 0.55f;

    [Header("Scale Range")]
    public float minScale = 0.7f;
    public float maxScale = 1.2f;

    [Header("Animation Settings")]
    public float speed = 3.0f;
    public float scaleSpeedMultiplier = 1.0f; // Optional multiplier to animate scale at a different rate

    private LensDistortion lensDistortion;
    private ColorAdjustments colorAdjustments;

    [Header("Fog Settings")]
    public float minFog = 0.001f;
    public float fogStep = 0.005f;
    public float increaseEachCycle = 20;
    public float maxFog = 0.023f;

    public Transform submarine;

    public Color minFilter = new Color(0, 94, 142);
    public Color maxFilter = new Color(93, 178, 221);
    public int depthChange = 100;

    public AudioSource audioSource;

    void Start() {
        if (globalVolume == null) {
            Debug.LogError("Global Volume not assigned.");
            enabled = false;
            return;
        }

        if (globalVolume.profile.TryGet(out colorAdjustments)) {
            colorAdjustments.colorFilter.value = Color.Lerp(minFilter, maxFilter, (-submarine.position.y / depthChange));
            colorAdjustments.colorFilter.overrideState = true;
        }

        if (globalVolume.profile.TryGet(out lensDistortion)) {
            lensDistortion.center.overrideState = true;
            lensDistortion.scale.overrideState = true;
        } else {
            Debug.LogError("LensDistortion not found in Volume Profile.");
            enabled = false;
        }

        if (audioSource != null) {
            int max = 600;
            float percent = -submarine.position.y / max;
            float pitch = Mathf.Lerp(3, 1, Mathf.Max(0, Mathf.Min(1, percent)));
            audioSource.pitch = pitch;
        }

            if (submarine) {
            UpdateFog();
        }

    }

    void Update() {
        if (lensDistortion == null) return;

        float time = Time.time * speed;

        float centerX = Mathf.Lerp(minX, maxX, (Mathf.Sin(time) + 1f) / 2f);
        float centerY = Mathf.Lerp(minY, maxY, (Mathf.Cos(time) + 1f) / 2f);

        float scaleTime = Time.time * speed * scaleSpeedMultiplier;
        float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(scaleTime) + 1f) / 2f);

        lensDistortion.center.value = new Vector2(centerX, centerY);
        lensDistortion.scale.value = scale;

        UpdateFog();
    }

    void UpdateFog() {
        int steps =  Mathf.CeilToInt(-submarine.position.y / increaseEachCycle);
        float currentTargetFog = Mathf.Max(minFog, Mathf.Min(maxFog, minFog + (steps * fogStep)));

        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, currentTargetFog, Time.deltaTime * 0.5f);
        colorAdjustments.colorFilter.value = Color.Lerp(colorAdjustments.colorFilter.value, Color.Lerp(minFilter, maxFilter, (-submarine.position.y / depthChange)), 0.5f * Time.deltaTime);
        colorAdjustments.colorFilter.overrideState = true;

        if (audioSource != null) {
            int max = 1200;
            float percent = -submarine.position.y/max;
            float pitch = Mathf.Lerp(3, 1, Mathf.Max(0, Mathf.Min(1,percent)));
            audioSource.pitch = Mathf.Lerp(audioSource.pitch, pitch, Time.deltaTime * 0.5f);
        }
    }

    Color HDRColor(Color c, float intensity) {
        float factor = Mathf.Pow(2, intensity);
        return new Color(c.r * factor, c.g * factor, c.b * factor);
    }
}
