using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class FollowPaths : MonoBehaviour {
    public SubmarineController submarineController;
    public Transform[] pathPoints;
    public float speed = 3f;
    public float reachThreshold = 0.05f;

    private int currentIndex = 0;
    public Vector2 moveDirection { get; private set; }

    private Vector2 previousPosition;
    private bool canRun;
    public bool disableCamera = true;
    public string sceneName;
    public int levelIndex = 0;

    void Init() {
        submarineController = FindFirstObjectByType<SubmarineController>();
        if (pathPoints.Length == 0) {
            Debug.LogWarning("No path points set!");
            enabled = false;
            return;
        }

        previousPosition = new Vector2(submarineController.transform.position.x, submarineController.transform.position.y);

        submarineController.isAnimation = true;
        Camera.main.GetComponent<SmoothFollow>().enabled = !disableCamera;
        currentIndex = 0;
    }

    void Update() {
        if (!canRun) return;

        if (currentIndex >= pathPoints.Length) {
            submarineController.moveInput = Vector2.zero;
            //submarineController.submarineLight.gameObject.SetActive(false);
            //SceneManager.LoadScene(sceneName);
            FindFirstObjectByType<SubmarineHUD>().PlayFadeOut();
            FindAnyObjectByType<GameManager>().ResetPlayer();
            PlayerPrefs.SetInt("desired_level", levelIndex);

            Timeout(() => {
                SceneManager.LoadScene(sceneName);
                Debug.Log("Loading scene: " + sceneName);
            }, 1.3f);
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

        submarineController.transform.LookAt(new Vector3(targetPosition.x, submarineController.transform.position.y, targetPosition.y));

        // Check if target reached
        if (Vector2.Distance(newPosition, targetPosition) < reachThreshold) {
            currentIndex++;
        }
    }

    void Timeout(Action callback, float time) {
        StartCoroutine(TimeoutCoroutine(callback, time));
    }
    IEnumerator TimeoutCoroutine(Action callback, float time) {
        yield return new WaitForSeconds(time);
        callback();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Init();
            canRun = true;
        }
    }
}
