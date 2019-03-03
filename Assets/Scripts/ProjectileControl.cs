using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is an automated script for enemy fired projectiles at regular intervals

public class ProjectileControl : MonoBehaviour
{
    public GameObject projectile;
    public Transform projectileSpawn;
    public float fireRate;
    public float delay;

    void Start()
    {
        InvokeRepeating("Fire", delay, fireRate);
    }

    void Fire()
    {
        Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
        GetComponent<AudioSource>().Play();
    }
}
