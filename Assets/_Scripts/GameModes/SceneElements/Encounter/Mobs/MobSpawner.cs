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

        private Coroutine mRespawnCoroutine = null;
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
                            mRespawnCoroutine = StartCoroutine(_Respawn(mRespawnDelaySeconds));
                        }
                        else if (levelMessage.Type == LevelManagement.MessageType.EncounterAvailable)
                        {
                            mMobsInEncounter = true;
                            if (mRespawnCoroutine != null)
                            {
                                StopCoroutine(mRespawnCoroutine);
                                mRespawnCoroutine = null;
                            }
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
