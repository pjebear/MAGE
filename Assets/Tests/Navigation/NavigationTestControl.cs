using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems;
using MAGE.GameSystems.World.Internal;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems.Appearances;
using MAGE.DB.Internal;
using MAGE.GameSystems.Characters.Internal;
using CharacterInfo = MAGE.GameSystems.Characters.CharacterInfo;

namespace MAGE.GameModes.Tests
{
    class NavigationTestControl : MonoBehaviour
    {
        public Button ThirdPersonBtn;
        public Button TopDownBtn;
        public Button ReinhardPicker;
        public Button AsmundPicker;
        //public ActorController Rheinhardt;
        //public ActorController Asmund;

        //private ActorController.ControllerState mControllerState;
        //private ActorController mSelectedActor;

        private void Awake()
        {
            
        }

        //private void Start()
        //{
        //    //mSelectedActor = Rheinhardt;
        //    // = ActorController.ControllerState.TopDown;

        //    ThirdPersonBtn.onClick.AddListener(     ()=> { UpdateSelection(mSelectedActor, ActorController.ControllerState.ThirdPerson); });
        //    TopDownBtn.onClick.AddListener(         ()=> { UpdateSelection(mSelectedActor, ActorController.ControllerState.TopDown); });
        //    ReinhardPicker.onClick.AddListener(         ()=> { UpdateSelection(Rheinhardt, mControllerState); });
        //    AsmundPicker.onClick.AddListener(         ()=> { UpdateSelection(Asmund, mControllerState); });
        //}

        //private void UpdateSelection(ActorController actorController, ActorController.ControllerState controllerState)
        //{
        //    if (mSelectedActor != actorController)
        //    {
        //        mSelectedActor.SetControllerState(ActorController.ControllerState.None);
        //    }
        //    mSelectedActor = actorController;
        //    mControllerState = controllerState;

        //    mSelectedActor.SetControllerState(controllerState);
        //}
    }
}



