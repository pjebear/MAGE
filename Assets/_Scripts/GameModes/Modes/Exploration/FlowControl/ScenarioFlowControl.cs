using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ScenarioFlowControl 
    : MonoBehaviour,
    IInputHandler,
    UIContainerControl,
    IEventHandler<ExplorationEvent>
{
    private string TAG = "ScenarioFlowControl";
   
    private ThirdPersonActorController mExplorationActor = null;
    private Scenario mScenario;

    private DB.DBConversation mConversation;
    private int mConversationIdx = 0;

    public void Init(ThirdPersonActorController exploring)
    {
        mExplorationActor = exploring;
        InputManager.Instance.RegisterHandler(this, false);
        ExplorationEventRouter.Instance.RegisterHandler(this);
    }

    public void CleanUp()
    {
        ExplorationEventRouter.Instance.UnRegisterListener(this);
        InputManager.Instance.ReleaseHandler(this);
    }

    #region UIContainerControl
    public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
    {
        switch ((UIContainerId)containerId)
        {
            case UIContainerId.ConversationView:
            {
                if (interactionInfo.InteractionType == UIInteractionType.Click)
                {
                    if (interactionInfo.ComponentId == (int)ConversationView.ComponentId.ContinueBtn)
                    {
                        if (mConversationIdx < mConversation.Conversation.Count - 1)
                        {
                            mConversationIdx++;
                            UIManager.Instance.Publish(UIContainerId.ConversationView);
                        }
                        else
                        {
                            UIManager.Instance.RemoveOverlay(UIContainerId.ConversationView);
                            ScenarioComplete();
                        }
                    }
                }

            }
            break;
        }
    }

    public string Name()
    {
        return TAG;
    }

    public IDataProvider Publish(int containerId)
    {
        IDataProvider dataProvider = null;

        switch ((UIContainerId)containerId)
        {
            case UIContainerId.ConversationView:
            {
                dataProvider = PublishConversation();
            }
            break;
        }

        return dataProvider;
    }

    IDataProvider PublishConversation()
    {
        ConversationView.DataProvider dataProvider = new ConversationView.DataProvider();

        DB.DBDialogue currentDialogue = mConversation.Conversation[mConversationIdx];

        int speakerId = mConversation.Members[currentDialogue.SpeakerIdx];

        CharacterInfo character = DB.CharacterHelper.FromDB(speakerId);

        dataProvider.Name = character.Name;
        dataProvider.PortraitAssetName = character.Appearance.PortraitSpriteId.ToString();
        dataProvider.Content = currentDialogue.Content;

        return dataProvider;
    }

    #endregion // UIContainerControl

    //! IInputHandler
    public void OnKeyPressed(InputSource source, int key, InputState state)
    {
        // empty
    }

    public void OnMouseHoverChange(GameObject mouseHover)
    {
        // empty
    }

    public void OnMouseScrolled(float scrollDelta)
    {
        // empty
    }

    public void HandleEvent(ExplorationEvent eventInfo)
    {
        switch (eventInfo.Type)
        {
            case (ExplorationEvent.EventType.ScenarioTriggered):
            {
                BeginScenario(eventInfo.Arg<Scenario>());
            }
            break;
        }
    }

    private void BeginScenario(Scenario scenario)
    {
        mScenario = scenario;
        mConversation = DB.DBHelper.LoadConversation((int)ConversationId.LotharInTrouble);
        mConversationIdx = 0;
        UIManager.Instance.PostContainer(UIContainerId.ConversationView, this);

        mExplorationActor.Enable(false);
    }

    private void ScenarioComplete()
    {
        mExplorationActor.Enable(true);
        GameSystemModule.Instance.PrepareEncounter(new EncounterCreateParams() { ScenarioId = EncounterScenarioId.Scenario });
        GameModesModule.Instance.Encounter();
    }
}

