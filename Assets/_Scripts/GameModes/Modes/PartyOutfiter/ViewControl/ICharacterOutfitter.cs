using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

interface ICharacterOutfiter
{
    void BeginOutfitting(MAGE.GameSystems.Characters.Character character, UnityAction characterUpdated);
    void Cleanup();
    //bool AreChangesPending();
    //void DiscardChanges();
    //void ApplyChanges();
}

