using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAGE.GameSystems;
using MAGE.Messaging;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Scenarios
{
    class DemoScenario : Scenario, Messaging.IMessageHandler 
    {
        // Stage 1
        public CinematicMoment OpeningCinematic;

        // Stage 2
        public Transform TrainingGroundParent;

        // Stage 3

        int mCurrentStage = 0;
        int mNumStages = 5;

        public override void Init()
        {
            SetupStage();
        }

        private void SetupStage()
        {
            switch (mCurrentStage)
            {
                case 0: SetupStage0(); break;
                case 1: SetupStage1(); break;
                case 2: SetupStage2(); break;
                case 3: SetupStage3(); break;
            }
        }

        public void HandleMessage(MessageInfoBase eventInfoBase)
        {
            if (eventInfoBase.MessageId == (int)LevelManagement.LevelMessage.Id)
            {
                LevelManagement.LevelMessage levelMessage = eventInfoBase as LevelManagement.LevelMessage;
                if (levelMessage.Type == LevelManagement.MessageType.CinematicComplete)
                {
                    if (mCurrentStage == 0)
                    {
                        mCurrentStage++;
                    }
                    if (mCurrentStage == 1)
                    {
                        mCurrentStage++;

                        PropInfo propInfo = LevelManagementService.Get().GetPropInfo((int)NPCPropId.DemoLevel_Captain);
                        

                        Conversation conversation = new Conversation();
                        conversation.Header = "Ready to train?";
                        propInfo.Conversations.Add(conversation);
                        Dialogue dialogue = new Dialogue();
                        dialogue.SpeakerIdx = ConversationConstants.PARTY_AVATAR_ID;

                        LevelManagementService.Get().UpdatePropInfo(propInfo);
                    }
                }
            }
            
        }

        public void SetupStage0()
        {
            //OpeningCinematic.gameObject.SetActive(true);
        }

        public void SetupStage1()
        {
            //TrainingGroundParent.gameObject.SetActive(true);
        }

        public void SetupStage2()
        {

        }

        public void SetupStage3()
        {

        }
    }
}
