using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices
{
    class Inventory
    {
        private string TAG = "Inventory";

        public Dictionary<int, int> Items = new Dictionary<int, int>();

        public void Add(int itemId, int count = 1)
        {
            Logger.Log(LogTag.GameSystems, TAG, string.Format("Add() - {0}", itemId));

            if (!Items.ContainsKey(itemId))
            {
                Items.Add(itemId, count);
            }
            else
            {
                Items[itemId]++;
            }
        }

        public bool Contains(int itemId)
        {
            return Items.ContainsKey(itemId);
        }

        public void Remove(int itemId)
        {
            Logger.Log(LogTag.GameSystems, TAG, string.Format("Remove() - {0}", itemId));

            Logger.Assert(Contains(itemId), LogTag.GameSystems, TAG, string.Format("Remove() Failed to find {0}", itemId), LogLevel.Warning);

            if (Contains(itemId))
            {
                Items[itemId]--;

                if (Items[itemId] == 0)
                {
                    Items.Remove(itemId);
                }
            }
        }
    }


}
