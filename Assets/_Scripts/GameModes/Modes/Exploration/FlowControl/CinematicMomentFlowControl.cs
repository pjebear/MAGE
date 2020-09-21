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

namespace MAGE.GameModes.FlowControl
{
    //class ScenarioFlowControl
    //    : MonoBehaviour,
    //    IInputHandler,
    //    UIContainerControl,
    //    Messaging.IMessageHandler
    //{
    //    private string TAG = "ScenarioFlowControl";

    //    private ThirdPersonActorController mExplorationActor = null;
    //    private Scenario mScenario;

    //    private MAGE.DB.DBConversation mConversation;
    //    private int mConversationIdx = 0;

    //    public void Init(ThirdPersonActorController exploring)
    //    {
    //        mExplorationActor = exploring;
    //        Input.InputManager.Instance.RegisterHandler(this, false);
    //        Messaging.MessageRouter.Instance.RegisterHandler(this);
    //    }

    //    public void CleanUp()
    //    {
    //        Messaging.MessageRouter.Instance.UnRegisterHandler(this);
    //        Input.InputManager.Instance.ReleaseHandler(this);
    //    }

    //    #region UIContainerControl
    //    public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
    //    {
    //        switch ((UIContainerId)containerId)
    //        {
    //            case UIContainerId.ConversationView:
    //            {
    //                if (interactionInfo.InteractionType == UIInteractionType.Click)
    //                {
    //                    if (interactionInfo.ComponentId == (int)ConversationView.ComponentId.ContinueBtn)
    //                    {
    //                        if (mConversationIdx < mConversation.Conversation.Count - 1)
    //                        {
    //                            mConversationIdx++;
    //                            UIManager.Instance.Publish(UIContainerId.ConversationView);
    //                        }
    //                        else
    //                        {
    //                            UIManager.Instance.RemoveOverlay(UIContainerId.ConversationView);
    //                            ScenarioComplete();
    //                        }
    //                    }
    //                }

    //            }
    //            break;
    //        }
    //    }

    //    public string Name()
    //    {
    //        return TAG;
    //    }

    //    public IDataProvider Publish(int containerId)
    //    {
    //        IDataProvider dataProvider = null;

    //        switch ((UIContainerId)containerId)
    //        {
    //            case UIContainerId.ConversationView:
    //            {
    //                dataProvider = PublishConversation();
    //            }
    //            break;
    //        }

    //        return dataProvider;
    //    }

    //    IDataProvider PublishConversation()
    //    {
    //        ConversationView.DataProvider dataProvider = new ConversationView.DataProvider();

    //        MAGE.DB.DBDialogue currentDialogue = mConversation.Conversation[mConversationIdx];

    //        int speakerId = mConversation.Members[currentDialogue.SpeakerIdx];

    //        MAGE.GameSystems.Characters.Character character = MAGE.GameSystems.CharacterService.Get().GetCharacter(speakerId);

    //        dataProvider.Name = character.Name;
    //        dataProvider.PortraitAssetName = character.GetAppearance().PortraitSpriteId.ToString();
    //        dataProvider.Content = currentDialogue.Content;

    //        return dataProvider;
    //    }

    //    #endregion // UIContainerControl

    //    //! IInputHandler
    //    public void OnKeyPressed(InputSource source, int key, InputState state)
    //    {
    //        // empty
    //    }

    //    public void OnMouseHoverChange(GameObject mouseHover)
    //    {
    //        // empty
    //    }

    //    public void OnMouseScrolled(float scrollDelta)
    //    {
    //        // empty
    //    }

    //    public void HandleMessage(Messaging.MessageInfoBase messageInfoBase)
    //    {
    //        switch (messageInfoBase.MessageId)
    //        {
    //            case Exploration.ExplorationMessage.Id:
    //            {
    //                Exploration.ExplorationMessage message = messageInfoBase as Exploration.ExplorationMessage;

    //                switch (message.Type)
    //                {
    //                    case (Exploration.ExplorationMessage.EventType.ScenarioTriggered):
    //                    {
    //                        BeginScenario(message.Arg<Scenario>());
    //                    }
    //                    break;
    //                }
    //            }
    //            break;
    //        }
    //    }

    //    private void BeginScenario(Scenario scenario)
    //    {
    //        mScenario = scenario;
    //        mConversation = MAGE.GameSystems.DBService.Get().LoadConversation((int)ConversationId.LotharInTrouble);
    //        mConversationIdx = 0;
    //        UIManager.Instance.PostContainer(UIContainerId.ConversationView, this);

    //        mExplorationActor.Enable(false);
    //    }

    //    private void ScenarioComplete()
    //    {
    //        mExplorationActor.Enable(true);
    //        MAGE.GameSystems.WorldService.Get().PrepareEncounter(new EncounterCreateParams() { ScenarioId = EncounterScenarioId.Scenario });
    //        GameModesModule.Instance.Encounter();
    //    }
    //}
}


