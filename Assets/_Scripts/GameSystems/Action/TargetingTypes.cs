using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace MAGE.GameSystems.Actions
{
    struct ActionTargetDetails
    {
        public RangeInfo CastDetails;
        public RangeInfo SelectionDetails;
        public bool IsSelfCast;

        public ActionTargetDetails(RangeInfo castDetails, RangeInfo selectionDetails, bool isSelfCast)
        {
            CastDetails = castDetails;
            SelectionDetails = selectionDetails;
            IsSelfCast = isSelfCast;
        }
    }

    struct TargetSelection
    {
        public Target FocalTarget;
        public RangeInfo SelectionRange;
        public TargetSelection(Target focalTarget, RangeInfo range)
        {
            FocalTarget = focalTarget;
            SelectionRange = range;
        }

        public TargetSelection(Target target)
            : this(target, RangeInfo.Unit)
        {
        }
    }


    enum TargetSelectionType
    {
        Tile,
        Character,

        NUM
    }

    struct Target
    {
        public TileIdx TileTarget;
        public Character CharacterTarget;
        public TargetSelectionType TargetType;

        public Target(TileIdx tileTarget)
        {
            TargetType = TargetSelectionType.Tile;
            TileTarget = tileTarget;
            CharacterTarget = null;
        }

        public Target(Character actorTarget)
        {
            TargetType = TargetSelectionType.Character;
            CharacterTarget = actorTarget;
            TileTarget = new TileIdx();
        }
    }
}



