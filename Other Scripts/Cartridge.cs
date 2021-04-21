using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cartridge : MonoBehaviour
{
    [SerializeField]
    bool isPushCatride = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == GameManager.PLAYER_NAME)
        {
            WeaponFacade.Carent.OnPickUp(isPushCatride);
            Debug.Log("pick up");
            Destroy(gameObject);
        }
    }
}
