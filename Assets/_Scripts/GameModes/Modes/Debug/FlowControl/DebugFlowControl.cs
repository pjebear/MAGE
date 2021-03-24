using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.DebugFlow
{
    class DebugFlowControl
        : FlowControlBase
    {
        private string TAG = "DebugFlowControl";

        private DebugSettings mDebugSettings;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Debug;
        }

        protected override void Setup()
        {
            mDebugSettings = FindObjectOfType<DebugSettings>();

            if (mDebugSettings != null && mDebugSettings.OverrideLevelId != LevelId.INVALID)
            {
                GameSystems.World.PartyLocation partyLocation = new GameSystems.World.PartyLocation();
                partyLocation.Level = mDebugSettings.OverrideLevelId;
                WorldService.Get().UpdatePartyLocation(partyLocation);
            }            
        }

        protected override void Cleanup()
        {
            // empty
        }

        public override string Condition(string queryEvent)
        {
            string conditionResult = "";

            switch (queryEvent)
            {
                case "skipMainMenu": conditionResult = mDebugSettings.SkipMainMenu ? "true" : "false"; break;
            }

            return conditionResult;
        }

        public override bool Notify(string param)
        {
            bool result = false;

            switch (param)
            {
                case "debugInit":
                {
                    WorldService.Get().PrepareNewGame();
                    result = true;
                }
                break;
            }

            return result;
        }
    }
}


