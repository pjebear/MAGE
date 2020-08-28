
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class CharacterActorController : MonoBehaviour
    {
        private float mBillboardDuration = 1f;
        private float mBillboardHeight = 2;
        public BillboardEmitter BillboardEmitter;
        public Actor Actor;
        public Character Character;
        public ActorController ActorController;

        private void Awake()
        {

        }

        public void DisplayStateChange(StateChange stateChange)
        {
            if (stateChange.Type == StateChangeType.ActionTarget || stateChange.Type == StateChangeType.StatusEffect)
            {
                if (stateChange.healthChange != 0)
                {
                    Billboard.Params data = new Billboard.Params(stateChange.healthChange.ToString(), transform, Vector3.up * mBillboardHeight);
                    BillboardEmitter.Emitt(data, mBillboardDuration);
                }
                if (stateChange.resourceChange != 0)
                {
                    Billboard.Params data = new Billboard.Params(stateChange.resourceChange.ToString(), transform, Vector3.up * mBillboardHeight);
                    BillboardEmitter.Emitt(data, mBillboardDuration);
                }
                foreach (StatusEffect effect in stateChange.statusEffects)
                {
                    DisplayStatusApplication(effect);
                }
            }
        }

        public void DisplayStatusApplication(StatusEffect effect)
        {
            Billboard.Params data = new Billboard.Params("+" + effect.ToString().ToString(), transform, Vector3.up * mBillboardHeight);
            BillboardEmitter.Emitt(data, mBillboardDuration);
        }

        public void DisplayStatusRemoval(StatusEffect statusEffect)
        {
            Billboard.Params data = new Billboard.Params("-" + statusEffect.ToString().ToString(), transform, Vector3.up * mBillboardHeight);
            BillboardEmitter.Emitt(data, mBillboardDuration);
        }

        public Orientation GetOrientation()
        {
            Orientation orientation = Orientation.Forward;

            Vector3 forward = transform.forward;
            if (forward == Vector3.forward)
            {
                orientation = Orientation.Forward;
            }
            else if (forward == Vector3.back)
            {
                orientation = Orientation.Back;
            }
            else if (forward == Vector3.right)
            {
                orientation = Orientation.Right;
            }
            else if (forward == Vector3.left)
            {
                orientation = Orientation.Left;
            }
            else
            {
                Logger.Assert(false, LogTag.Character, "CharacterController", "Failed to calculate Orientation");
            }

            return orientation;
        }
    }
}



