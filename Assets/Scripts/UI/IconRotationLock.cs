using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconRotationLock : MonoBehaviour
{
    Vector3 rot;


    void Start()
    {
        
    }

    void FixedUpdate()
    {
        rot = transform.rotation.eulerAngles;
        rot.y = 0;
        transform.rotation = Quaternion.Euler(rot);
    }
}
