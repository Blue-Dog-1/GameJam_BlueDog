using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == GameManager.PLAYER_NAME)
        {
            GameManager.OnEndGame();
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
