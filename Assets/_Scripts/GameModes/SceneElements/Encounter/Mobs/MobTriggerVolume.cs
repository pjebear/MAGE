using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Encounters
{
    class MobTriggerVolume : TriggerVolumeBase<ThirdPersonActorController>
    {
        protected override int GetLayer()
        {
            return (int)Layer.Mob;
        }

        private void Awake()
        {
            gameObject.AddComponent<SphereCollider>().isTrigger = true;
        }

        protected override void HandleTriggerEntered(ThirdPersonActorController entered)
        {
            MobSpawner spawner = GetComponentInParent<MobSpawner>();
            if (spawner != null)
            {
                spawner.NotifyMobTriggered(this, entered);
            }
        }
    }
}
