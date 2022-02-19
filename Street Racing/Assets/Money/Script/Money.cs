using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public float speed = 5f;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(40 * Time.deltaTime * speed, 0, 0);
    }
}
