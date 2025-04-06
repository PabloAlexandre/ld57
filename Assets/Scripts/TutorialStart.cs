using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialStart : MonoBehaviour
{
    public string gameSceneName = "IgorTest"; // ou nome da sua cena

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
