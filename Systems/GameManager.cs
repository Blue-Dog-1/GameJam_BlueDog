using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Main { get; private set; }
    static public readonly string PLAYER_NAME = "PLAYER";
    [Header("                                         UIController")]
    [SerializeField] UIController m_UIController = null;
    [Space(30)]
    [SerializeField] [Range(.02f, .04f)] float m_velocityMoveCoins;

    [SerializeField] UnityEvent OnEndGameEvent = null;
    [SerializeField] UnityEvent OnWinEvent = null;

    static public UIController UIController { get; private set; }
    static public float velocityMoveCoins { get; private set; }

    public void AddLisenerEndGame(UnityAction method)
    {
        OnEndGameEvent.AddListener(method);
        OnWinEvent.AddListener(method);
    }
    void Awake()
    {
        Main = this;
        UIController = m_UIController;
        velocityMoveCoins = m_velocityMoveCoins;
    }

    static public void OnWin()
    {
        Main.OnWinEvent.Invoke();
        CharacterController2D.Main.gameObject.SetActive(false);
        PlayerPrefs.SetInt("LASTLEVEL", SceneManager.GetActiveScene().buildIndex);
        Debug.Log("PLAYER WON");
    }
    static public void OnEndGame()
    {
        Main.OnEndGameEvent.Invoke();
        Debug.Log("END GAME");
    }

    void Update()
    {
        m_UIController.Update();
    }

    public void Replay()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Contiune()
    {
        Time.timeScale = 1;

        var indexNextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > indexNextScene)
            SceneManager.LoadScene(indexNextScene);
        else
        {

        }
    }
    public void Exit()
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
