using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootBehaviour : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    float forceAmount = 1200f;
    float timeFromLastShoot;

    public void Shoot(float shootFreq)
    {
        if((timeFromLastShoot += Time.deltaTime) >= 1/shootFreq)
        {
            InstantiateBullet();
            timeFromLastShoot = 0;
        }

    }

    public void Shoot()
    {
        InstantiateBullet();
    }

    private void InstantiateBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(forceAmount * transform.forward);
    }
}
