using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SubmarineMining : MonoBehaviour
{
    public SubmarineStats stats;
    public Slider miningSlider;
    public AudioSource audioSource;
    public AudioClip miningClip;

    [Header("Input System")]
    public InputActionReference mineAction;

    private MiningResource currentTarget;
    private float miningTimer;
    private bool isMining = false;
    private bool isHoldingMine = false;

    [Header("Drill Rotation")]
    public Transform drillMesh;
    public float drillRotationSpeed = 360f; // graus por segundo

    private void OnEnable()
    {
        if (mineAction != null)
        {
            mineAction.action.started += ctx => isHoldingMine = true;
            mineAction.action.canceled += ctx => isHoldingMine = false;
            mineAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (mineAction != null)
            mineAction.action.Disable();
    }

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
        FindClosestMiningResource();

        if (currentTarget != null && isHoldingMine)
        {
            if (!isMining)
                StartMiningFeedback();

            miningTimer += Time.deltaTime;

            if (miningSlider != null)
                miningSlider.value = miningTimer;

            if (miningTimer >= stats.miningSpeed)
                CompleteMining();
        }
        else if (isMining)
        {
            StopMiningFeedback();
        }

        // Roda a broca enquanto estiver minerando
        if (drillMesh != null && isHoldingMine)
        {
            drillMesh.Rotate(Vector3.forward, drillRotationSpeed * Time.deltaTime);
        }
    }

    void FindClosestMiningResource()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, stats.miningDistance);
        MiningResource closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            MiningResource resource = hit.GetComponent<MiningResource>();
            if (resource != null)
            {
                float dist = Vector3.Distance(transform.position, resource.transform.position);
                if (dist < closestDist)
                {
                    closest = resource;
                    closestDist = dist;
                }
            }
        }

        currentTarget = closest;

        if (miningSlider != null)
        {
            miningSlider.gameObject.SetActive(currentTarget != null);
            miningSlider.maxValue = stats.miningSpeed;
        }
    }

    void StartMiningFeedback()
    {
        isMining = true;
        miningTimer = 0f;

        if (miningSlider != null)
            miningSlider.value = 0f;

        if (audioSource != null && miningClip != null)
        {
            audioSource.clip = miningClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        Debug.Log($"⛏️ Começou mineração em {currentTarget.name}");
    }

    void StopMiningFeedback()
    {
        isMining = false;
        miningTimer = 0f;

        if (miningSlider != null)
            miningSlider.value = 0f;

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        Debug.Log("⛔ Mineração pausada.");
    }

    void CompleteMining()
    {
        Debug.Log($"✅ Mineração completa! +{currentTarget.goldValue} gold");

        stats.gold += currentTarget.goldValue;
        currentTarget.ShrinkAndDestroy();

        StopMiningFeedback();
        currentTarget = null;

        if (miningSlider != null)
            miningSlider.gameObject.SetActive(false);
    }
}
