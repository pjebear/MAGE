using MAGE.GameModes.Combat;
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
        Point,
        Focal,

        NUM
    }

    struct Target
    {
        public Transform PointTarget;
        public CombatTarget FocalTarget;
        public TileIdx TileTarget;
        public Character CharacterTarget;
        public TargetSelectionType TargetType;

        public Target(TileIdx tileTarget)
        {
            TargetType = TargetSelectionType.Tile;
            TileTarget = tileTarget;
            CharacterTarget = null;
            PointTarget = null;
            FocalTarget = null;
        }

        public Target(Character actorTarget)
        {
            TargetType = TargetSelectionType.Character;
            CharacterTarget = actorTarget;
            TileTarget = new TileIdx();
            PointTarget = null;
            FocalTarget = null;
        }

        public Target(Transform pointTarget)
        {
            TargetType = TargetSelectionType.Point;
            CharacterTarget = null;
            TileTarget = new TileIdx();
            PointTarget = pointTarget;
            FocalTarget = null;
        }

        public Target(CombatTarget focalTarget)
        {
            TargetType = TargetSelectionType.Focal;
            CharacterTarget = null;
            TileTarget = new TileIdx();
            PointTarget = null;
            FocalTarget = focalTarget;
        }

        public Transform GetTargetPoint()
        {
            Transform transform = null;

            switch (TargetType)
            {
                case TargetSelectionType.Point:
                    transform = PointTarget;
                    break;
                case TargetSelectionType.Focal:
                    transform = FocalTarget.transform;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            return transform;
        }
    }
}



