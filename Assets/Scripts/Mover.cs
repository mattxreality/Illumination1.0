using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public float speed;
    Rigidbody rigidpody;


    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
}
