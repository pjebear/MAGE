﻿using MAGE.GameServices.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class AuraDirector 
        : MonoBehaviour
        , Messaging.IMessageHandler
    {
        public Aura AuraPrefab;

        private Dictionary<EncounterActorController, List<Aura>> mAuras = new Dictionary<EncounterActorController, List<Aura>>();

        public void Init()
        {
            Messaging.MessageRouter.Instance.RegisterHandler(this);
        }

        private void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        public void RegisterAura(AuraInfo auraInfo, EncounterActorController actorController, bool activateImmediately)
        {
            Aura aura = Instantiate(AuraPrefab, actorController.transform);
            aura.Initialize(auraInfo, actorController);

            if (!mAuras.ContainsKey(actorController)) { mAuras.Add(actorController, new List<Aura>()); }

            mAuras[actorController].Add(aura);

            aura.SetActive(activateImmediately);
        }

        public void RemoveActor(EncounterActorController actor)
        {
            if (mAuras.ContainsKey(actor))
            {
                foreach (Aura aura in mAuras[actor])
                {
                    Destroy(aura.gameObject);
                }
                mAuras.Remove(actor);
            }
        }

        public void HandleMessage(Messaging.MessageInfoBase messageInfoBase)
        {
            switch (messageInfoBase.MessageId)
            {
                case EncounterMessage.Id:
                {
                    EncounterMessage message = messageInfoBase as EncounterMessage;

                    switch (message.Type)
                    {
                        case EncounterMessage.EventType.UnitPlacementComplete:
                        {
                            foreach (List<Aura> auras in mAuras.Values)
                            {
                                foreach (Aura aura in auras)
                                {
                                    aura.SetActive(true);
                                }
                            }
                        }
                        break;

                        case EncounterMessage.EventType.CharacterKO:
                        {
                            EncounterActorController controller = EncounterModule.CharacterDirector.CharacterActorLookup[message.Arg<EncounterCharacter>()];
                            if (mAuras.ContainsKey(controller))
                            {
                                foreach (Aura aura in mAuras[controller])
                                {
                                    aura.SetActive(false);
                                }
                            }

                        }
                        break;
                    }
                }
                break;
            }
        }

    }
}


