using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionUI_DW : MonoBehaviour
{
    public GameObject menuPanel;

    public void MainMenu_button()
    {
        Time.timeScale = 0;
        menuPanel.SetActive(true);
    }

    public void ContinueGameButton()
    {
        Time.timeScale = 1;
        menuPanel.SetActive(false);
    }

    public void ProgramQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
