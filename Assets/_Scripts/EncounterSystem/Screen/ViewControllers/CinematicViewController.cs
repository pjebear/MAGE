using Common.CharacterEnums;
using Common.CinematicEnums;
using Common.EncounterEnums;
using Common.EncounterTypes;
using EncounterSystem.Character;
using EncounterSystem.EncounterFlow;
using EncounterSystem.EncounterFlow.Mediator;
using EncounterSystem.Map;
using EncounterSystem.MapTypes;
using Screens.Payloads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EncounterSystem.Screen
{
    class CinematicViewController : MonoBehaviour
    {
        public DialogueControl BottomDialogueControl;
        public DialogueControl TopDialogueControl;
        private InputManager rInputManager;
        private MovementMediator rMovmentMediator;
        private CameraManager rCameraManager;
        private EncounterState rEncounterState;
        private Queue<CinematicComponent> mCinematicQueue;

        private MapTile mHighlightedTile;

        private bool mProgressEvent;

        private void Awake()
        {
            mCinematicQueue = new Queue<CinematicComponent>();
            Debug.Assert(BottomDialogueControl != null);
            Debug.Assert(TopDialogueControl != null);
            BottomDialogueControl.gameObject.SetActive(false);
            TopDialogueControl.gameObject.SetActive(false);
        }

        // Use this for initialization
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                mProgressEvent = true;
            }
        }

        public void Initialize(InputManager manager, MovementMediator movementMediator, EncounterState state)
        {
            rInputManager = manager;
            rMovmentMediator = movementMediator;
            rEncounterState = state;
            rCameraManager = Camera.main.gameObject.GetComponent<CameraManager>();
        }

        public void BeginCinematicEvent(CinematicBlueprint cinematic)
        {
            Debug.Assert(cinematic.CinematicComponents.Count > 0);
            mCinematicQueue = cinematic.CinematicComponents;

            ProgressCinematicEvent();
        }

        private void ProgressCinematicEvent()
        {
            if (mCinematicQueue.Count == 0)
            {
                rInputManager.CinematicFinished();
            }
            else
            {
                CinematicComponent component = mCinematicQueue.Dequeue();
                switch (component.ComponentType)
                {
                    case (CinematicComponentType.UnitReveal):
                        StartCoroutine(_ProgressRevealEvent(component as UnitRevealCinematic));
                        break;

                    case (CinematicComponentType.Movement):
                        StartCoroutine(_ProgressMovementEvent(component as MovementCinematic));
                        break;

                    case (CinematicComponentType.Dialogue):
                        StartCoroutine(_ProgressDialogueEvent(component as DialogueCinematic));
                        break;

                    default:
                        Debug.Log("No Cinematic handler hooked up for " + component.ComponentType.ToString());
                        rInputManager.CinematicFinished();
                        break;
                }
            }
        }

        private IEnumerator _ProgressMovementEvent(MovementCinematic movement)
        {
            HashSet<CharacterManager> toMove = new HashSet<CharacterManager>();
            foreach (var pair in movement.CharactersToMove)
            {
                CharacterManager character = rEncounterState.GetCharacter(pair.Key);
                MapTile destination = MapManager.Instance.GetTileAt(pair.Value);
                if (character != null  && destination != null && destination.GetCharacterOnTile() == null)
                {
                    toMove.Add(character);
                    rMovmentMediator.MediateMovement(character, destination, () => { toMove.Remove(character); }, true);
                }
                else
                {
                    Debug.Log("Invalid Movement Event for character " + pair.Key + " to tile " + pair.Value);
                    toMove.Clear();
                    break;
                }
            }

            yield return new WaitUntil(() => { return toMove.Count == 0; });
            yield return new WaitForSeconds(.1f);
            ProgressCinematicEvent();
        }

        private IEnumerator _ProgressRevealEvent(UnitRevealCinematic unitReveal)
        {
            foreach (var toReveal in unitReveal.UnitsToReveal)
            {
                if (toReveal.IsReveal) // show
                {
                    // if showing, character should be in the cinematic pool 
                    CharacterManager character = rEncounterState.CinematicUnits[toReveal.CharacterId];
                    character.gameObject.SetActive(true);
                    if (toReveal.AddToEncounter)
                    {
                        rEncounterState.TransferCinematicUnitToBattle(toReveal.CharacterId);
                        if (rEncounterState.FlowState > EncounterFlowState.EncounterBegun)
                        {
                            character.BeginEncounter();
                        }
                        else
                        {
                            // character will have been initialized in the unit initialization step in encounter flow
                        }
                       
                    }
                    MapManager.Instance.AddCharacter(character);
                    character.UpdateCurrentTile(MapManager.Instance.GetTileAt(toReveal.TileId), true);
                }
                else
                {
                    CharacterManager toRemove = null;
                    if (rEncounterState.CinematicUnits.ContainsKey(toReveal.CharacterId))
                    {
                        toRemove = rEncounterState.CinematicUnits[toReveal.CharacterId];
                    }
                    else
                    {
                        toRemove = rEncounterState.GetCharacter(toReveal.CharacterId);
                    }
                    if (toRemove != null)
                    {
                        toRemove.GetCurrentTile().RemoveCharacterFromTile();
                        toRemove.gameObject.SetActive(false);
                        rEncounterState.RemoveUnitFromEncounterLineup(toRemove);
                    }
                }
            }

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(.1f);
            ProgressCinematicEvent();
        }

        private IEnumerator _ProgressDialogueEvent(DialogueCinematic dialogue)
        {
            Queue<Dialogue> conversation = dialogue.DialogueExchange;

            while (conversation.Count > 0)
            {
                Dialogue nextDialogueEvent = conversation.Dequeue();
                CharacterManager speaker = rEncounterState.GetCharacter(nextDialogueEvent.SpeakerId);
                CharacterManager listener = rEncounterState.GetCharacter(nextDialogueEvent.ListenerId);
                Quaternion currentRotation = speaker.gameObject.transform.localRotation;
                speaker.GetComponent<MovementController>().LookAt(listener.gameObject.transform.localPosition);
                rCameraManager.PanToTarget(speaker.gameObject);

                if (mHighlightedTile != null)
                {
                    mHighlightedTile.SetTileColor(Color.clear);
                }
                mHighlightedTile = speaker.GetCurrentTile();
                mHighlightedTile.SetTileColor(MapEnums.MapConstants.CursorTileColor);
                UnitPanelPayload unitpayload = speaker.GetUnitPanelPayload();
                Vector3 positionOnScreen = Camera.main.WorldToScreenPoint(speaker.gameObject.transform.position);
                Sprite characterImage = Resources.Load<Sprite>(unitpayload.ImageAssetPath);
                if (positionOnScreen.y < 200)
                {
                    TopDialogueControl.gameObject.SetActive(true);
                    TopDialogueControl.DisplayDialogue(characterImage, unitpayload.UnitName, nextDialogueEvent.Content);
                    BottomDialogueControl.gameObject.SetActive(false);
                }
                else
                {
                    BottomDialogueControl.gameObject.SetActive(true);
                    BottomDialogueControl.DisplayDialogue(characterImage, unitpayload.UnitName, nextDialogueEvent.Content);
                    TopDialogueControl.gameObject.SetActive(false);
                }
                mProgressEvent = false;
                yield return new WaitUntil(() => { return mProgressEvent; });
                speaker.transform.localRotation = currentRotation;
            }

            TopDialogueControl.gameObject.SetActive(false);
            BottomDialogueControl.gameObject.SetActive(false);
            mHighlightedTile.SetTileColor(Color.clear);
            mHighlightedTile = null;
            yield return new WaitForSeconds(.1f);
            ProgressCinematicEvent();
        }
    }
}

