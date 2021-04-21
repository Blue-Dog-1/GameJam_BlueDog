using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Main { get; private set; }
    static public readonly string PLAYER_NAME = "PLAYER";
    [SerializeField] UIController m_UIController = null;

    static public UIController UIController { get; private set; }
    [SerializeField] UnityEvent OnEndGameEvent;
    void Awake()
    {
        Main = this;
        UIController = m_UIController;
    }

    static public void OnEndGame()
    {
        Main.OnEndGameEvent.Invoke();
        Debug.Log("END GAME");

    }
}
