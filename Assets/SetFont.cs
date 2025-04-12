using TMPro;
using UnityEngine;

public class SetFont : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindBestPointSizeForThisResolution();
    }

    // Update is called once per frame
    void FindBestPointSizeForThisResolution() {
        this.GetComponent<TMP_Text>().enableAutoSizing = true;
        //this.GetComponent<TMP_Text>().text = "Placeholder text to check the best point size for this resolution";
        this.GetComponent<TMP_Text>().enableAutoSizing = false;
    }
}
