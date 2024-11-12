using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyRunners.Player {
    [CreateAssetMenu(menuName = "Powerups/SpeedPowerup")]
    public class SpeedBuff : PowerupEffect
    {
        public float amount;
        public GameObject effect;
        public override void Apply(GameObject target)
        {
            target.GetComponent<PlayerPowerupManager>().AddPowerup(this);
            target.GetComponent<PlayerMovement>().AdjustSpeed(amount);
            target.GetComponent<PlayerEffectManager>().SpawnEffect(effect);
        }

        public override void Remove(GameObject target)
        {
            target.GetComponent<PlayerMovement>().AdjustSpeed(-amount);
            target.GetComponent<PlayerEffectManager>().RemoveEffect();
        }
    }
}