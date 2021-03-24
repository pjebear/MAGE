using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAGE.Messaging;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Encounters
{
    class MobSpawner 
        : MonoBehaviour
        , Messaging.IMessageHandler
    {
        private bool mMobsInEncounter = false;
        public float mRespawnDelaySeconds = 15;
        public List<MobControl> Mobs = new List<MobControl>();
        
        public void Awake()
        {
            Mobs = GetComponentsInChildren<MobControl>(true).ToList();
        }

        void Start()
        {
            MessageRouter.Instance.RegisterHandler(this);
        }

        void OnDestroy()
        {
            MessageRouter.Instance.UnRegisterHandler(this);
        }

        public void NotifyMobTriggered(MobTriggerVolume triggerVolume, ThirdPersonActorController entered)
        {
            Vector3 triggerVolumePos = triggerVolume.transform.position;
            Vector3 mapPos = RaycastUtil.GetRayCastHit(triggerVolumePos + Vector3.up * 100, Vector3.down, 500, new List<RayCastLayer>() { RayCastLayer.Terrain });
            EncounterContainerGenerator.GenerateEncounterContainer(entered, Mobs.Select(x=>x.GetComponent<ActorSpawner>()).ToList(), mapPos);

            EnableHeirarchy(false);

            mMobsInEncounter = true;
        }

        public void HandleMessage(MessageInfoBase eventInfoBase)
        {
            switch (eventInfoBase.MessageId)
            {
                case LevelManagement.LevelMessage.Id:
                {
                    LevelManagement.LevelMessage levelMessage = eventInfoBase as LevelManagement.LevelMessage;
                    if (levelMessage != null)
                    {
                        if (levelMessage.Type == LevelManagement.MessageType.EncounterComplete)
                        {
                            mMobsInEncounter = false;
                            StartCoroutine(_Respawn(mRespawnDelaySeconds));
                        }
                    }
                }
                break;
            }
            
        }

        private void EnableHeirarchy(bool enable)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(enable);
            }
        }

        private System.Collections.IEnumerator _Respawn(float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);

            EnableHeirarchy(true);
        }
    }
}
