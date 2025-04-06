using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CaveEntranceTrigger : MonoBehaviour
{
    [Header("Configuração")]
    public float descendDistance = 2f;
    public float descendDuration = 1f;
    public float delayBeforeSceneLoad = 1.5f;
    public string sceneToLoad = "CaveScene"; // nome da próxima cena

    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            isTriggered = true;

            // Desativa o controle do submarino
            SubmarineController controller = other.GetComponent<SubmarineController>();
            if (controller != null)
                controller.enabled = false;

            SmoothFollow camFollow = Camera.main.GetComponent<SmoothFollow>();
            if (camFollow != null)
                camFollow.StopFollowingPosition();


            StartCoroutine(EnterCaveSequence(other.transform));
        }
    }

    private IEnumerator EnterCaveSequence(Transform submarine)
    {
        Vector3 startPos = submarine.position;
        Vector3 endPos = startPos - new Vector3(0, descendDistance, 0);

        float t = 0f;
        while (t < descendDuration)
        {
            submarine.position = Vector3.Lerp(startPos, endPos, t / descendDuration);
            t += Time.deltaTime;
            yield return null;
        }

        submarine.position = endPos;

        // Espera um pouco antes de carregar a cena
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        SceneManager.LoadScene(sceneToLoad);
    }
}
