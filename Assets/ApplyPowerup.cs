using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyPowerup : MonoBehaviour
{
    public PowerupEffect powerup; 
    bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !used)
        { 
            Pickup(other.gameObject);
            used = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && used)
        { 
            used = false;
        }
    }

    public void Pickup(GameObject target)
    { 
        powerup.Pickup(target); 
    }
}
