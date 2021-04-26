using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cartridge : MonoBehaviour
{
    [Range(1, 10)]
    [SerializeField] public int m_count = 5;
    [SerializeField]
    bool isPushCatride = true;
    [SerializeField]
    bool isSuperPower;

    private void Start()
    {
        var scale = 1f + (m_count * 0.1f);
        var localScale = transform.localScale;
        localScale *= scale;
        transform.localScale = localScale;

        var particleSystem = GetComponent<ParticleSystem>();

        particleSystem.startSize *= scale;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject.GetComponent<Collider>());
        if (isSuperPower)
        {
            return;
        }
        if (collision.gameObject.name == GameManager.PLAYER_NAME)
        {
            StartCoroutine(Move(collision.transform));
        }
    }

    IEnumerator Move(Transform target)
    {
        float distance;
        while(true)
        {
            distance = Vector3.Distance(transform.position, target.position);
            transform.position = Vector3.Lerp(transform.position, target.position, GameManager.velocityMoveCoins);   
            yield return new WaitForFixedUpdate();
            if (distance < .5f)
                break;
        }
        Destroy();
    }
    void AddSuperPower()
    {
        
    }
    void Destroy()
    {
        WeaponFacade.Carent.OnPickUp(isPushCatride, m_count);
        CharacterController2D.PickUp();
        Destroy(gameObject);
    }
}
