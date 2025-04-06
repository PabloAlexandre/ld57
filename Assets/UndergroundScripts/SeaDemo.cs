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

    void Start() {
        if (globalVolume == null) {
            Debug.LogError("Global Volume not assigned.");
            enabled = false;
            return;
        }

        if (globalVolume.profile.TryGet(out lensDistortion)) {
            lensDistortion.center.overrideState = true;
            lensDistortion.scale.overrideState = true;
        } else {
            Debug.LogError("LensDistortion not found in Volume Profile.");
            enabled = false;
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
    }
}
