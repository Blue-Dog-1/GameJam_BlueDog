using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public const string LASTLEVEL = "LASTLEVEL"; 
    [SerializeField] Button m_ContiuneButton;
    private void Start()
    {
        var level = PlayerPrefs.GetInt(LASTLEVEL, 0) + 1;
        m_ContiuneButton.interactable = level > 1 && SceneManager.sceneCountInBuildSettings > level;
    }
    public void StartNewGame()
    {
        SceneManager.LoadScene(1);
    }
    public void ContiuneGame()
    {
        var index = PlayerPrefs.GetInt(LASTLEVEL, 0) + 1;

        if (SceneManager.sceneCountInBuildSettings > index)
        {
            SceneManager.LoadScene(index);
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }

#if UNITY_EDITOR
    [ContextMenu("Clear Player Prefs")]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
#endif

}
