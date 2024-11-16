using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyRunners.Player {
    [CreateAssetMenu(menuName = "Powerups/SpeedPowerup")]
    public class SpeedBuff : PowerupEffect
    {
        public float amount;
        public GameObject effect;
        private GameObject playerTarget;
        public override void Apply(GameObject target)
        { 
            target.GetComponent<PlayerPowerupManager>().AddPowerup(this);
            target.GetComponent<PlayerMovement>().AdjustSpeed(amount);
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
            target.GetComponent<PlayerMovement>().AdjustSpeed(-amount);
            target.GetComponent<PlayerEffectManager>().RemoveEffect();
        }
    }
}