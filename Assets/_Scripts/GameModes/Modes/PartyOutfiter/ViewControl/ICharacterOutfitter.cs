using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

interface ICharacterOutfiter
{
    void SetCharacter(int characterId);
    void Refresh();
    void BeginOutfitting();
    void Cleanup();
    //bool AreChangesPending();
    //void DiscardChanges();
    //void ApplyChanges();
}

