using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{

    [Header ("shoot VFX ") ]
    [SerializeField] ParticleSystem m_laser;
    [SerializeField] ParticleSystem m_light;
    [SerializeField] ParticleSystem m_lightningSpark;

    public void OnShoot()
    {
        m_light.Play();
        m_laser.Play();

        m_lightningSpark.Stop();
    }

    public void OnCast()
    {
        m_lightningSpark.Play();
    }




}
