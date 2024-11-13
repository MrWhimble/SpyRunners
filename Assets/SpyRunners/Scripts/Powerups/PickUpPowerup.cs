using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpPowerup : MonoBehaviour
{
    public PowerupEffect powerup;
    public float duration;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Pickup(other.gameObject);
        }
    }

    public void Pickup(GameObject target)
    {
        powerup.Pickup(target);
        Destroy(gameObject);
    }
}
