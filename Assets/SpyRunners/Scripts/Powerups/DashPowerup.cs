using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyRunners.Managers;
namespace SpyRunners.Player
{
    [CreateAssetMenu(menuName = "Powerups/DashPowerup")]
    public class DashPowerup : PowerupEffect
    {
        public float dashForce; 
        private GameObject playerTarget; 
        public GameObject effect;
        private Rigidbody rb;
        private Camera camera;
        public override void Apply(GameObject target)
        {
            rb = target.GetComponent<Rigidbody>();
            GameObject camera = target.GetComponent<CameraManager>().GetComponent<Camera>().gameObject;

            rb.useGravity = false; 
            rb.AddForce(camera.transform.forward * dashForce);

            target.GetComponent<PlayerPowerupManager>().AddPowerup(this); 
            target.GetComponent<PlayerEffectManager>().SpawnEffect(effect);

             
        }
         
        public override void Pickup(GameObject target)
        {
            playerTarget = target;
            if (instant)
            {
                Apply(target);
            }
            else
            {
                target.GetComponent<PlayerPowerupManager>().AddPowerup(this);
            }
        }

        public override void Remove(GameObject target)
        {
            rb.useGravity = true;

        }   
    }
}