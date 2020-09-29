using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Encounter
{
    abstract class EncounterCharacterControl 
        : MonoBehaviour
        , ActorSpawner.IRefreshListener
    {
        protected ActorSpawner ActorSpawner;

        private void Awake()
        {
            ActorSpawner = GetComponentInChildren<ActorSpawner>(true);
            Debug.Assert(ActorSpawner != null);
            if(ActorSpawner != null)
            {
                ActorSpawner.RefreshOnStart = false;
            }

            Init();
        }
        private void Start()
        {
            if (ActorSpawner != null)
            {
                ActorSpawner.RegisterListener(this);
                ActorSpawner.CharacterPicker.RootCharacterId = GetCharacterId();
                ActorSpawner.Refresh();
            }
        }
        protected abstract void Init();

        private void OnDestroy()
        {
            if (ActorSpawner != null)
            {
                ActorSpawner.UnRegisterListener(this);
            }
            Cleanup();
        }
        protected abstract void Cleanup();

        public abstract int GetCharacterId();

        public void OnActorRefresh()
        {
            // empty
        }
    }
}
