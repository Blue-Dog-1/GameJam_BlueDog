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
            StartCoroutine(Move(collision.transform));
        }
    }

    IEnumerator Move(Transform target)
    {
        float distance;
        while(true)
        {
            distance = Vector3.Distance(transform.position, target.position);
            transform.position = Vector3.Lerp(transform.position, target.position, GameManager.velocityMoveCoins * Time.fixedDeltaTime );   
            yield return new WaitForFixedUpdate();
            if (distance < .5f)
                break;
        }
        Destroy();
    }

    void Destroy()
    {
        WeaponFacade.Carent.OnPickUp(isPushCatride);
        Debug.Log("pick up");
        Destroy(gameObject);
    }
}
