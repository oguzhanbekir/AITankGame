using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 5);
    }

    private void OnCollisionEnter(Collision collision)
    {
        healthBehaviour healthBehaviour = collision.gameObject.GetComponent<healthBehaviour>();
        if (healthBehaviour != null)
        {
            healthBehaviour.TakeDamage(20);
        }
        Destroy(gameObject, 1);
    }
}
