using System.Collections;
using UnityEngine;

public class MiningResource : MonoBehaviour
{
    public float goldValue = 1f;
    public AudioClip destroySound;

    private bool isBeingDestroyed = false;

    public void ShrinkAndDestroy()
    {
        if (!isBeingDestroyed)
            StartCoroutine(ShrinkAndDestroyRoutine());
    }

    private IEnumerator ShrinkAndDestroyRoutine()
    {
        isBeingDestroyed = true;

        Vector3 originalScale = transform.localScale;
        Vector3 growScale = originalScale * 1.2f;

        float growDuration = 0.05f;
        float shrinkDuration = 0.15f;

        // Cresce
        float t = 0f;
        while (t < growDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, growScale, t / growDuration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = growScale;

        // 🔊 Toca som com volume mais alto
        if (destroySound != null)
        {
            GameObject tempGO = new GameObject("BreakSound");
            tempGO.transform.position = transform.position;

            AudioSource src = tempGO.AddComponent<AudioSource>();
            src.clip = destroySound;
            src.volume = 10f;
            src.spatialBlend = 0f; // 2D, toca forte em qualquer lugar
            src.bypassEffects = true;
            src.bypassListenerEffects = true;
            src.bypassReverbZones = true;
            src.Play();

            Destroy(tempGO, destroySound.length);

        }

        // Encolhe
        t = 0f;
        while (t < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(growScale, Vector3.zero, t / shrinkDuration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;

        Destroy(gameObject);
    }
}
