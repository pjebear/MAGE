using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class EncounterContext
{
    public EncounterType EncounterType;
    public int CurrencyReward;
    public List<ItemId> ItemRewards = new List<ItemId>();

    public LevelId LevelId;
    public TileIdx BottomLeft;
    public TileIdx TopRight;

    public List<EncounterCondition> WinConditions = new List<EncounterCondition>();
    public List<EncounterCondition> LoseConditions = new List<EncounterCondition>();

    public EncounterContext()
    {

    }
}

