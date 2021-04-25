using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] bool x, y;
    [SerializeField] float smoothness;

    Vector2 tmp;
    void Update()
    {
        if (smoothness <= .05f)
            Hard();
        else
            Somooth();
    }

    void Hard()
    {
        tmp.x = x ? target.position.x : transform.position.x;
        tmp.y = y ? target.position.y : transform.position.y;

        transform.position = tmp;
    }
    void Somooth()
    {
        tmp.x = x ? target.position.x : transform.position.x;
        tmp.y = y ? target.position.y : transform.position.y;

        transform.position = Vector3.Lerp(transform.position, tmp, smoothness * Time.deltaTime);
    }
}
