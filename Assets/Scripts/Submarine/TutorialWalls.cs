using UnityEngine;

public class TutorialWalls : MonoBehaviour
{
    void Start()
    {
        if(PlayerPrefs.GetInt("no_tutorial", 0) == 1) {
            Destroy(gameObject);
        }    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
