using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

interface ICharacterOutfiter
{
    void BeginOutfitting(DB.DBCharacter character, UnityAction characterUpdated);
    void Cleanup();
    //bool AreChangesPending();
    //void DiscardChanges();
    //void ApplyChanges();
}

