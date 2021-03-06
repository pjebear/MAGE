﻿using MAGE.GameModes.SceneElements;
using MAGE.Messaging;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.FlowControl
{
    class CinematicFlowControl
        : FlowControlBase
    {
        private CinematicMoment mCinematicMoment;

        // Flow Control Base
        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Cinematic;
        }

        protected override void Cleanup()
        {
            
        }

        protected override void Setup()
        {
            List<CinematicMoment> activeCinematics = LevelManagementService.Get().GetLoadedLevel().GetActiveCinematics();
            Logger.Assert(activeCinematics.Count > 0, LogTag.FlowControl, GetFlowControlId().ToString(), "Setup() - No active cinematics present in level");
            if (activeCinematics.Count > 0)
            {
                mCinematicMoment = activeCinematics[0];
                mCinematicMoment.Play();
            }
            else
            {
                OnCinematicComplete();
            }
        }

        public override void HandleMessage(MessageInfoBase eventInfoBase)
        {
            switch (eventInfoBase.MessageId)
            {
                case LevelManagement.LevelMessage.Id:
                {
                    LevelManagement.LevelMessage levelMessage = eventInfoBase as LevelManagement.LevelMessage;
                    if (levelMessage.Type == LevelManagement.MessageType.CinematicComplete)
                    {
                        OnCinematicComplete();
                    }
                }
                break;
            }
        }

        public override void OnKeyPressed(InputSource source, int key, InputState state)
        {
            if (source == InputSource.Keyboard
                && state == InputState.Down
                && key == (int)KeyCode.Escape)
            {
                if (mCinematicMoment != null)
                {
                    mCinematicMoment.Skip();
                }
            }
        }

        private void OnCinematicComplete()
        {
            if (mCinematicMoment != null)
            {
                Transform partyAvatar = mCinematicMoment.GetPartyAvatarInScene();
                if (partyAvatar != null)
                {
                    GameSystems.World.PartyLocation partyLocation = GameSystems.WorldService.Get().GetPartyLocation();
                    partyLocation.SetPosition(partyAvatar.position);
                    GameSystems.WorldService.Get().UpdatePartyLocation(partyLocation);
                }
            }

            SendFlowMessage(FlowAction.advance.ToString());
        }
    }
}
