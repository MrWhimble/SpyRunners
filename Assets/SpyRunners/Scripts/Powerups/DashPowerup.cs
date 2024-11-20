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

        public int NumOfUses = 2;

        //private Vector3 oldVelocity;

        public override void Apply(GameObject target)
        {
            rb = target.GetComponent<Rigidbody>();
            GameObject camera = CameraManager._instance.GetComponent<Camera>().gameObject;

            rb.useGravity = false;
            //  rb.AddForce(camera.transform.forward * dashForce); 

            if(rb.velocity.y < 1)
            {
            //    oldVelocity = rb.velocity;
                Vector3 vel = rb.velocity;
                vel.y = 0;
                rb.velocity = vel;
            }

            Vector3 forceToApply = camera.transform.forward * dashForce + camera.transform.up;

            rb.AddForce(forceToApply, ForceMode.Impulse); 
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
            target.GetComponent<PlayerEffectManager>().RemoveEffect();
            target.GetComponent<PlayerMovement>().AdjustSpeed(-dashForce);
            //rb.velocity = oldVelocity;
        }   
    }
}