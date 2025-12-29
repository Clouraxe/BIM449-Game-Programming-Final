using UnityEngine;

public class MainMenuUI : MonoBehaviour
{

    public void OnPlayButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene"); // Load the main game scene
    }
    
    public void OnQuitButtonPressed()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
