using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Encounters
{
    [RequireComponent(typeof(ActorSpawner))]
    [RequireComponent(typeof(CharacterPickerControl))]
    abstract class CharacterControl : MonoBehaviour
    {
        protected ActorSpawner ActorSpawner;

        private void Start()
        {
            Init();
            
            GetComponent<CharacterPickerControl>().CharacterPicker.Reset();
            GetComponent<CharacterPickerControl>().CharacterPicker.RootCharacterId = GetCharacterId();
            GetComponent<ActorSpawner>().Refresh();
        }

        protected abstract void Init();

        private void OnDestroy()
        {
            Cleanup();
        }
        protected abstract void Cleanup();

        public abstract int GetCharacterId();
    }
}
