using UnityEngine;

/// <summary>
/// Makes an object follow a target like a fish, with smooth movement and distance keeping.
/// </summary>
public class FishFollow : MonoBehaviour {
    [Header("Target Settings")]
    [Tooltip("The transform to follow")]
    public Transform target;

    [Tooltip("Minimum distance to keep from the target")]
    public float minDistance = 2.0f;


    [Header("Movement Settings")]
    [Tooltip("Maximum movement speed")]
    public float maxSpeed = 5.0f;

    [Tooltip("How quickly the fish accelerates")]
    public float acceleration = 2.0f;

    [Tooltip("How quickly the fish rotates toward the target direction")]
    public float rotationSpeed = 3.0f;

    [Header("Fish Behavior")]
    [Tooltip("Amount of random movement to add")]
    public float wanderStrength = 0.5f;

    [Tooltip("Frequency of direction changes when wandering")]
    public float wanderFrequency = 0.2f;

    [Tooltip("Vertical movement amplitude")]
    public float verticalMovement = 0.5f;

    [Tooltip("Vertical movement speed")]
    public float verticalFrequency = 1.0f;

    // Private variables
    private Vector3 velocity;
    private Vector3 wanderDirection;
    private float wanderTimer;
    private float verticalOffset;
    public Vector3 offset = new Vector3(0, 0, 6);

    public bool canFollow = false;

    private void Start() {
        // Initialize with a random direction for wandering
        wanderDirection = Random.insideUnitSphere;

        // Initialize random timer offset so fish don't all wander in sync
        wanderTimer = Random.Range(0f, 3f);
        verticalOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update() {
        if (target == null || canFollow == false)
            return;

        // Calculate distance and direction to target
        Vector3 targetPosition = target.position + offset;
        Vector3 toTarget = targetPosition - transform.position;
        float distance = toTarget.magnitude;

        // Calculate desired speed based on distance
        float targetSpeed = 0;

        if (distance > minDistance) {
            targetSpeed = maxSpeed;
        }

        // Update wander behavior
        wanderTimer += Time.deltaTime;
        if (wanderTimer > wanderFrequency) {
            wanderDirection = Random.insideUnitSphere;
            wanderTimer = 0;
        }

        // Combine follow and wander directions
        Vector3 desiredDirection = toTarget.normalized;
        if (distance > minDistance) {
            desiredDirection = Vector3.Lerp(wanderDirection, desiredDirection, 1);
        } else {
            desiredDirection = wanderDirection;
        }

        // Apply vertical movement
        verticalOffset += Time.deltaTime * verticalFrequency;
        Vector3 verticalWave = Vector3.up * Mathf.Sin(verticalOffset) * verticalMovement;

        // Update velocity with acceleration
        Vector3 targetVelocity = desiredDirection * targetSpeed;
        velocity = Vector3.Lerp(velocity, targetVelocity + verticalWave, acceleration * Time.deltaTime);

        // Move the fish
            transform.position += velocity * Time.deltaTime;

        // Rotate the fish if it's moving
        if (velocity.magnitude > 0.1f) {
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Visualize the minimum and maximum distance in the editor
    private void OnDrawGizmosSelected() {
        if (target != null) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.position, minDistance);

        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            canFollow = true;
            if(GetComponent<CollectResource>() != null) {
                GetComponent<CollectResource>().OnCollectFish();
            }
        }
    }
}