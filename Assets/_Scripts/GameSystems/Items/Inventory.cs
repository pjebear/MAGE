using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Inventory
{
    private string TAG = "Inventory";

    public Dictionary<int, int> Items = new Dictionary<int, int>();

    public void Add(int itemId)
    {
        Logger.Log(LogTag.GameSystems, TAG, string.Format("Add() - {0}", itemId));

        if (!Items.ContainsKey(itemId))
        {
            Items.Add(itemId, 1);
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

