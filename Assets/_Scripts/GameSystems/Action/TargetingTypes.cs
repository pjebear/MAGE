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

        //public TargetSelection(Target target)
        //    : this(target, RangeInfo.Unit)
        //{
        //}
    }


    enum TargetSelectionType
    {
        Empty,
        Point,
        Focal,

        NUM
    }

    struct Target
    {
        public Vector3 PointTarget;
        public CombatTarget FocalTarget;
        public TargetSelectionType TargetType;

        public static Target Empty
        {
            get
            {
                Target target = new Target();
                target.TargetType = TargetSelectionType.Empty;
                target.PointTarget = Vector3.zero;
                target.FocalTarget = null;

                return target;
            }
        }

        public Target(Vector3 pointTarget)
        {
            TargetType = TargetSelectionType.Point;
            PointTarget = pointTarget;
            FocalTarget = null;
        }

        public Target(CombatTarget focalTarget)
        {
            TargetType = TargetSelectionType.Focal;

            PointTarget = Vector3.zero;
            FocalTarget = focalTarget;
        }

        public Vector3 GetTargetPoint()
        {
            Vector3 point = Vector3.zero;

            switch (TargetType)
            {
                case TargetSelectionType.Point:
                    point = PointTarget;
                    break;
                case TargetSelectionType.Focal:
                    point = FocalTarget.transform.position;
                    break;
                case TargetSelectionType.Empty:
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            return point;
        }
    }
}