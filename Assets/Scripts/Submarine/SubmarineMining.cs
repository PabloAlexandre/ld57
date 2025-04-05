using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SubmarineMining : MonoBehaviour
{
    public SubmarineStats stats;
    public Slider miningSlider; // ← UI Slider na cena
    private Coroutine miningCoroutine;
    public AudioSource audioSource;
    public AudioClip miningClip;


    private void Start()
    {
        if (miningSlider != null)
        {
            miningSlider.gameObject.SetActive(false);
            miningSlider.value = 0f;
        }
    }

    private void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, stats.miningDistance);

        foreach (Collider hit in hits)
        {
            MiningResource resource = hit.GetComponent<MiningResource>();
            if (resource != null)
            {
                if (miningCoroutine == null)
                {
                    Debug.Log($"🛠️ Iniciando mineração em: {resource.name}");
                    miningCoroutine = StartCoroutine(MineResource(resource));
                    break;
                }
            }
        }
    }

    private IEnumerator MineResource(MiningResource resource)
    {
        float timer = 0f;

        if (miningSlider != null)
        {
            miningSlider.gameObject.SetActive(true);
            miningSlider.maxValue = stats.miningSpeed;
            miningSlider.value = 0f;
        }

        if (audioSource != null && miningClip != null)
        {
            audioSource.clip = miningClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        while (timer < stats.miningSpeed)
        {
            if (resource == null || Vector3.Distance(transform.position, resource.transform.position) > stats.miningDistance)
            {
                Debug.Log("❌ Mineração cancelada.");

                if (audioSource != null && audioSource.isPlaying)
                    audioSource.Stop();

                if (miningSlider != null)
                    miningSlider.gameObject.SetActive(false);

                miningCoroutine = null;
                yield break;
            }

            timer += Time.deltaTime;
            if (miningSlider != null) miningSlider.value = timer;

            yield return null;
        }

        stats.gold += resource.goldValue;
        resource.ShrinkAndDestroy();

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (miningSlider != null)
            miningSlider.gameObject.SetActive(false);

        miningCoroutine = null;
    }

}
