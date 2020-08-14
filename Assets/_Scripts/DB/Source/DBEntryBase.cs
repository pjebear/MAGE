using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    abstract class DBEntryBase
    {
        public abstract void Copy(DBEntryBase _from, DBEntryBase _to);

        public void Set(DBEntryBase entry)
        {
            Copy(entry, this);
        }

        public void CopyTo(DBEntryBase to)
        {
            Copy(this, to);
        }
    }
}



