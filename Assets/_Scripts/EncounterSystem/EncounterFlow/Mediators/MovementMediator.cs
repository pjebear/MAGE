using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EncounterSystem.EncounterFlow.Mediator
{
    using Character;
    using MapTypes;
    class MovementMediator : MediatorBase
    {
        public void MediateMovement(CharacterManager toMove, MapTile destination, System.Action callback, bool forCinematic = false)
        {
            StartCoroutine(_MediateMovement(toMove, destination, callback, forCinematic));
        }

        private IEnumerator _MediateMovement(CharacterManager toMove, MapTile destination, System.Action callback, bool forCinematic)
        {
            toMove.HasMoved = true;
            toMove.GetCurrentTile().RemoveCharacterFromTile();

            if (forCinematic)
            {
                rMapManager.CalculatePathTiles(toMove, forCinematic);
            }// otherwise will have been calculated and cached
           
            Stack<MapTile> movementList = rMapManager.GetMovementPathTo(destination);
            MovementController controller = toMove.GetComponent<MovementController>();
            MapTile moveTo = null;
            while (movementList.Count > 0)
            {
                moveTo = movementList.Pop();
                CharacterManager onNextTile = moveTo.GetCharacterOnTile();
                if (onNextTile != null)
                {
                    Debug.Assert(movementList.Count > 0, "Attempting to finish turn on occupied tile");
                    Debug.Assert(!(onNextTile.IsPlayerControlled ^ toMove.IsPlayerControlled), "Attempting to move through enemy tile");
                    // move unit out of the way
                }
                controller.MoveTo(moveTo.GetTileCenter());
                yield return new WaitUntil(controller.Arrived);
            }
            // @moveTo will be the last tile moved to by the character 

            toMove.UpdateCurrentTile(moveTo, true);
            controller.CleanUp();
            callback();
        }
    }


}

