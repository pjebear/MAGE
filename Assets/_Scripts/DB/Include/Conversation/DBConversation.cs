using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBConversation : Internal.DBEntryBase
    {
        public int Id;
        public string Name;
        public string Header;
        public List<int> Members = new List<int>();
        public List<DBDialogue> Conversation = new List<DBDialogue>();
        
        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBConversation from = _from as DBConversation;
            DBConversation to = _to as DBConversation;

            to.Id = from.Id;
            to.Name = from.Name;
            to.Header = from.Header;
            to.Members = new List<int>(from.Members);

            to.Conversation.Clear();
            foreach (DBDialogue dialogue in from.Conversation)
            {
                DBDialogue toAdd = new DBDialogue();
                toAdd.Set(dialogue);
                to.Conversation.Add(toAdd);
            }
        }
    }
}
