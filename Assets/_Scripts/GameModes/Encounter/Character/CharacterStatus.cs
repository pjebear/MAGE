using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


enum CharacterStatusType
{
    HasActed,
    HasMoved,

    NUM
}


class CharacterStatus
{
    private int[] mStatus;
    public CharacterStatus()
    {
        mStatus = new int[(int)CharacterStatusType.NUM];
    }

    public int this[CharacterStatusType idx]
    {
        get { return mStatus[(int)idx]; }
        set { mStatus[(int)idx] = value; }
    }
}



