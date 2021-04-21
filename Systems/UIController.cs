using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class UIController
{
    [SerializeField] Image m_chargePuhs = null;
    [SerializeField] Image m_chargePull = null;
    [SerializeField] Text m_countTextPush = null;
    [SerializeField] Text m_countTextPull = null;
    [SerializeField] Text m_countHP = null;


    public float chargePuhs {
        get => m_chargePuhs.fillAmount;
        set {
            m_chargePuhs.fillAmount = value;

        }
    }
    public float chargePull {
        get => m_chargePull.fillAmount;
        set {
            m_chargePull.fillAmount = value;
        }
    }
    public string CoutTextPush {
        get => m_countTextPush.text;
        set {
            m_countTextPush.text = value;
        }
    }
    public string CoutTextPull
    {
        get => m_countTextPull.text;
        set
        {
            m_countTextPull.text = value;
        }
    }
    public int HP { set { m_countHP.text = value.ToString(); } }



}
