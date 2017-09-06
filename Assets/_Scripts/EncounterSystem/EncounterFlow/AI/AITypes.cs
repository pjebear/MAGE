using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EncounterSystem.AITypes
{
    using Action;
    using Character;
    using MapTypes;
    // Used to record previous turn actions
    struct TurnIntentionRecord
    {
        public CharacterManager Target;
        public List<MapTile> TargetPath;
    }

    struct PotentialTurnAction
    {
        public MapTile MovementLocation;
        public ActionBase Action;
        public MapTile ActionTarget;
        public float Priority;
        public bool NeedsMovementForAction;

        public PotentialTurnAction(MapTile moveLocation, ActionBase action, MapTile actionTarget, float priority, bool needsMovement)
        {
            MovementLocation = moveLocation;
            Action = action;
            ActionTarget = actionTarget;
            Priority = priority;
            NeedsMovementForAction = needsMovement;
        } 
    }
}
