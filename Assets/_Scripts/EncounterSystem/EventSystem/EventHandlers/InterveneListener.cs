using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace EncounterSystem.EventSystem
{
    using Character;
    using MapTypes;
    class InterveneListener : ListenerHandler
    {
        CharacterManager mIntervenedUnit = null;
        protected override void Awake()
        {
            mListenedForEvents = new List<EventType>()
            {
                EventType.OnTargeted
            };
            mListenerRange = 2f;
        }

        public override void HandleEvent(EventType type, object payload)
        {
            UnityEngine.Debug.Log("Intervene recieved event call for " + type.ToString());
            switch (type)
            {
                case (EventType.OnTargeted):
                    {
                        OnTargetedPayload onTargetedPayload = payload as OnTargetedPayload;
                        if (!(mListener.IsPlayerControlled ^ onTargetedPayload.Targeted.IsPlayerControlled) // allied character
                            && mListener.IsPlayerControlled != onTargetedPayload.Targeted.IsPlayerControlled // Don't defend your own attack
                            && !onTargetedPayload.Action.ActionInfo.BaseResourceChangeInfo.IsBeneficial
                            && mIntervenedUnit == null) // Don't attempt to block multiple units
                        {
                            mIntervenedUnit = onTargetedPayload.Targeted;
                            SwapTiles();

                            Vector3 displacement = onTargetedPayload.Actor.transform.position - onTargetedPayload.Targeted.transform.position;
                            displacement.y = 0;

                            mListener.transform.forward = displacement;
                            mListener.transform.position = mIntervenedUnit.transform.position + displacement.normalized * 0.5f;
                            ApplyEventHook(EventType.OnTargetedComplete, mListener.gameObject);
                        }
                        break;
                    }
                case (EventType.OnTargetedComplete):
                    {
                        OnTargetedPayload onTargetedPayload = payload as OnTargetedPayload;
                        if (mIntervenedUnit != null && onTargetedPayload.Targeted == mListener)
                        {
                            SwapTiles();
                            mListener.GetCurrentTile().PlaceCharacterAtTileCenter();
                            mIntervenedUnit = null;
                            var eventHooks = mListener.GetComponents<EventHook>()
                    .Where(hook => (hook.HookTag - mListenerTag < (int)EventType.NUM) && (hook.HookTag - mListenerTag >= 0)).ToList();

                            if (eventHooks.Count == 0)
                            {
                                Debug.Log("No event hooks on character leaving listener range");
                            }
                            for (int i = 0; i < eventHooks.Count; i++)
                            {
                                Destroy(eventHooks[i]);
                            }
                        }
                        break;
                    }
            }
        }

        private void SwapTiles()
        {
            MapTile intervenedTile = mIntervenedUnit.GetCurrentTile();
            MapTile currentTile = mListener.GetCurrentTile();
            intervenedTile.RemoveCharacterFromTile();
            mListener.UpdateCurrentTile(intervenedTile, false);
            currentTile.RemoveCharacterFromTile();
            mIntervenedUnit.UpdateCurrentTile(currentTile, false);
        }
    }
}
