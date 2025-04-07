using UnityEngine;

public class AfterDeath : MonoBehaviour {
    public Transform player;
    public Transform[] points;

    void Start() {
        int index = PlayerPrefs.GetInt("desired_level", 0);
        player.position = points[index].position;
    }

    // Update is called once per frame
    void Update() {

    }
}
