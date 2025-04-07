using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour {
    public TMP_Text label;
    public UnityEngine.UI.Image image;
    public float speed = 0;
    public GameManager manager;
    public bool btnPressed = false;
    float time;

    private void Update() {
        if(btnPressed) {
            time += Time.deltaTime;
            
            Color color = image.color;
            color.a = Mathf.Lerp(color.a, 1, Time.deltaTime * 4f);
            image.color = color;

            Color colorB = label.color;
            colorB.a = Mathf.Lerp(colorB.a, 0, Time.deltaTime * speed * 3);
            label.color = colorB;

            if (time > 1.5f) {
                Debug.Log("Load Scene");
                manager.CreateGame();
                SceneManager.LoadScene("TutoDemo");
                return;
            }
            return;
        }

        Color c = label.color;
        c.a = Mathf.PingPong(Time.time * speed, 0.7f) + 0.3f;
        label.color = c;

        if(Input.anyKeyDown) {
            btnPressed = true;
        }
    }
}
