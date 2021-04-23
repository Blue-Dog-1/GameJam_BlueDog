using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Main { get; private set; }
    static public readonly string PLAYER_NAME = "PLAYER";
    [SerializeField] UIController m_UIController = null;
    [SerializeField] [Range(.5f, 3f)] float m_velocityMoveCoins;

    [SerializeField] UnityEvent OnEndGameEvent;

    static public UIController UIController { get; private set; }
    static public float velocityMoveCoins  { get; private set;}

    void Awake()
    {
        Main = this;
        UIController = m_UIController;
        velocityMoveCoins = m_velocityMoveCoins;
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
    public void Exit()
    {
        Application.Quit();
    }    
}
