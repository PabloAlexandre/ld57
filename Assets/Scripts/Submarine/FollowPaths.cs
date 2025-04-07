using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class FollowPaths : MonoBehaviour {
    public SubmarineController submarineController;
    public Transform[] pathPoints;
    public float speed = 3f;
    public float reachThreshold = 0.05f;

    private int currentIndex = 0;
    public Vector2 moveDirection { get; private set; }

    private Vector2 previousPosition;
    private bool canRun;
    public string sceneName;

    void Init() {
        if (pathPoints.Length == 0) {
            Debug.LogWarning("No path points set!");
            enabled = false;
            return;
        }

        previousPosition = new Vector2(submarineController.transform.position.x, submarineController.transform.position.y);

        submarineController.isAnimation = true;
        Camera.main.GetComponent<SmoothFollow>().enabled = false;
        currentIndex = 0;
    }

    void Update() {
        if (!canRun) return;

        if (currentIndex >= pathPoints.Length) {
            submarineController.moveInput = Vector2.zero;
            Destroy(this);
            submarineController.submarineLight.gameObject.SetActive(false);
            SceneManager.LoadScene(sceneName);
            return;
        }

        Vector2 currentPosition = new Vector2(submarineController.transform.position.x, submarineController.transform.position.y);
        Vector2 targetPosition = pathPoints[currentIndex].position;

        Vector2 dirVector = (targetPosition - currentPosition);

        // Move towards target
        Vector2 newPosition = Vector2.Lerp(currentPosition, pathPoints[currentIndex].position, speed * Time.deltaTime);

        // Update moveDirection
        Vector2 delta = newPosition - previousPosition;
        submarineController.moveInput = dirVector.normalized * speed;
        previousPosition = newPosition;

        // Check if target reached
        if (Vector2.Distance(newPosition, targetPosition) < reachThreshold) {
            currentIndex++;
        }
    }


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Init();
            canRun = true;
        }
    }
}
