using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ActionResponseInfo
{
    public static int INFINITE_RANGE = -1;
    public int PercentChance;
    public int Range;

    public ActionResponseInfo(int percentChange, int range)
    {
        PercentChance = percentChange;
        Range = range;
    }
}