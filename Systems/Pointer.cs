using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    [SerializeField] Transform charecter;
    [SerializeField] Transform finihs;
    
    void Update()
    {
        var vector = (finihs.position - charecter.position);
        vector.z = 0;
        
        transform.rotation =  Quaternion.FromToRotation(Vector3.right, vector);
    }
}
