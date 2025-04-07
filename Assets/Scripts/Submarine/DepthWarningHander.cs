using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DepthWarningHandler : MonoBehaviour
{
    [Header("Referências")]
    public SubmarineStats stats;
    public Image screenFlash;
    public SubmarineController deathTarget;

    [Header("Distância Dinâmica de Aviso")]
    public float warningMargin = 2f;        // distância base


    private bool warningActive = false;
    private Coroutine warningCoroutine;

    private void Update()
    {
        float y = transform.position.y;
        float limit = stats.maxDepth;

        float verticalSpeed = GetComponent<Rigidbody>()?.linearVelocity.y ?? 0f;

        // A margem agora leva em conta a velocidade vertical * o speed do stats
        float dynamicMargin = warningMargin * stats.speed;
        float warningThreshold = limit + dynamicMargin;

        if (y <= limit)
        {
            TriggerDeath();
        }
        else if (y <= warningThreshold && !warningActive)
        {
            warningActive = true;
            warningCoroutine = StartCoroutine(FlashScreenWarning());
        }
        else if (y > warningThreshold && warningActive)
        {
            warningActive = false;
            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
                warningCoroutine = null;
            }
            ResetFlash();
        }
    }


    private IEnumerator FlashScreenWarning()
    {
        if (screenFlash == null) yield break;

        float t = 0f;
        while (warningActive)
        {
            t += Time.deltaTime * 4f;
            float alpha = Mathf.PingPong(t, 0.5f); // Alpha entre 0 e 0.5
            Color it = screenFlash.color;
            it.a = alpha;
            screenFlash.color = it;
            yield return null;
        }

        Color c = screenFlash.color;
        c.a = 0;
        screenFlash.color = c;
    }

    private void ResetFlash()
    {
        if (screenFlash != null) {
            Color c = screenFlash.color;
            c.a = 0;
            screenFlash.color = c;
        }
    }

    private void TriggerDeath()
    {
        //warningActive = false;
        //ResetFlash();

        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);

        //if (screenFlash != null) {
        //    Color c = screenFlash.color;
        //    c.a = 1;
        //    screenFlash.color = c;
        //}

        if (deathTarget != null)
            deathTarget.Die();
        else
            Debug.LogWarning("Nenhum SubmarineController atribuído como deathTarget.");
    }

}
