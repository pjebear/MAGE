using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    [System.Serializable]
    class DBDialogue : DBEntryBase
    {
        public int SpeakerIdx;
        public string Content;
       
        public override void Copy(DBEntryBase _from, DBEntryBase _to)
        {
            DBDialogue from = _from as DBDialogue;
            DBDialogue to = _to as DBDialogue;

            to.SpeakerIdx = from.SpeakerIdx;
            to.Content = from.Content;
        }
    }
    
}
