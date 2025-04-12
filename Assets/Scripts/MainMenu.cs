using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour {
    public TMP_Text label;
    public UnityEngine.UI.Image image;
    public float alphaSpeed = 0;
    public GameManager manager;
    public bool btnPressed = false;
    float time;
    public SubmarineStats stats;

    [Header("Base Attrs reset game")]
    public float speed = 3;
    public float maxDepth = -80;
    public float miningSpeed = 3;
    public float miningDistance = 2;
    public float gold = 5;
    public float lightDistance = 30;

    private void Update() {
        if(btnPressed) {
            time += Time.deltaTime;
            
            Color color = image.color;
            color.a = Mathf.Lerp(color.a, 1, Time.deltaTime * 4f);
            image.color = color;

            Color colorB = label.color;
            colorB.a = Mathf.Lerp(colorB.a, 0, Time.deltaTime * alphaSpeed * 3);
            label.color = colorB;

            if (time > 1.5f) {
                manager.CreateGame();
                SceneManager.LoadScene("TutoDemo");
                return;
            }
            return;
        }

        Color c = label.color;
        c.a = Mathf.PingPong(Time.time * alphaSpeed, 0.7f) + 0.3f;
        label.color = c;

        if(Input.anyKeyDown) {
            btnPressed = true;

            stats.speed = speed;
            stats.miningDistance = miningDistance;
            stats.miningSpeed = miningSpeed;
            stats.maxDepth = maxDepth;
            stats.gold = gold;
            stats.lightDistance = lightDistance;
        }
    }
}
