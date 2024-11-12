using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpyRunners.Player
{
    public class PlayerPowerupManager : MonoBehaviour
    {
        public PowerupEffect currentPowerup;
        bool powerUpActive = false;

        public void AddPowerup(PowerupEffect newpowerUp)
        {
            currentPowerup = newpowerUp;
            powerUpActive = true;
            StartCoroutine(Duration(currentPowerup.duration));
        }

        public IEnumerator Duration(float duration)
        { 
            yield return new WaitForSeconds(duration);
            currentPowerup.Remove(gameObject);
        } 
    }
}