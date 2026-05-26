using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private SettingTabManager settingTabManager;

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingPanel.SetActive(true);

        settingTabManager.ResetToDefaultTab();
    }
    
    //변경했음
}