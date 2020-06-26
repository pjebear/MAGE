using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class DBUtil
{
    public static RangeInfo FromDB(DB.DBRangeInfo dbEntry)
    {
        RangeInfo rangeInfo = new RangeInfo();

        rangeInfo.AreaType = (AreaType)dbEntry.Type;
        rangeInfo.MinRange = dbEntry.Min;
        rangeInfo.MaxRange = dbEntry.Max;
        rangeInfo.MaxElevationChange = dbEntry.Elevation;

        return rangeInfo;
    }
}

