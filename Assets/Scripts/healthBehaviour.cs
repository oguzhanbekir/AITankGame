using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBehaviour : MonoBehaviour
{
    public Text healthText;
    public Image healthBar;
    float health = 100f;
   

    public void TakeDamage(float amount)
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }
        health -= amount;
        healthText.text = string.Format("%{0}", health);
        healthBar.fillAmount = health / 100f;
    }
}
