using UnityEngine;
using UnityEngine.InputSystem;

public class DemoInput : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected InputAction moveAction;

    void Start() {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update() {
        if(moveAction.ReadValue<Vector2>().magnitude > 0) {
            Vector2 move = moveAction.ReadValue<Vector2>();
            transform.rotation = Quaternion.Euler(0, Mathf.Atan2(move.x, move.y) * Mathf.Rad2Deg, 0);
            transform.position += new Vector3(move.x, 0, move.y) * Time.deltaTime;
        }
    }
}
