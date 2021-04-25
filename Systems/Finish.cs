using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [SerializeField] float m_force;
    [SerializeField] float m_radius = 1;

    [Range(.1f, 3f)]
    [SerializeField] float m_radiusSubTrigger = 1f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.gameObject.name == GameManager.PLAYER_NAME)
        {
            GameManager.OnWin();
        }
        else
        {
            Destroy(collision.transform.gameObject);
        }
    }

    public void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_radius);
        foreach (var item in colliders)
        {
            item.attachedRigidbody?.AddForce((transform.position - item.transform.position).normalized * m_force * Time.deltaTime, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }

}
