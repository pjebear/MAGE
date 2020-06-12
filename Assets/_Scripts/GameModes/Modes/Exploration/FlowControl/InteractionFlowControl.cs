using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class InteractionFlowControl 
    : MonoBehaviour,
    IInputHandler,
    UIContainerControl
{
    private string TAG = "InteractionFlowControl";
    private float mMinInteractionDistance = 2;

    private float mDistanceToHovered = 0;
    private ThirdPersonActorController mExplorationActor = null;
    private IInteractable mHoveredInteractable = null;
    private IInteractable mInteractingWith = null;

    private DB.DBConversation mConversation;
    private int mConversationIdx = 0;

    public void Init(ThirdPersonActorController exploring)
    {
        mExplorationActor = exploring;
        InputManager.Instance.RegisterHandler(this, false);
    }

    public void CleanUp()
    {
        InputManager.Instance.ReleaseHandler(this);
        mHoveredInteractable = null;
    }

    void Update()
    {
        if (mHoveredInteractable != null)
        {
            bool wasPreviousInRange = IsInRange();
            mDistanceToHovered = DistanceToHoverTarget();
            bool isNowInRange = IsInRange();

            if (wasPreviousInRange != isNowInRange)
            {
                UpdateHoverTargetCursor();
            }
        }
    }

    #region UIContainerControl
    public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
    {
        switch ((UIContainerId)containerId)
        {
            case UIContainerId.NPCActionSelectView:
            {
                if (interactionInfo.InteractionType == UIInteractionType.Click)
                {
                    if (interactionInfo.ComponentId == (int)NPCActionSelectView.ComponentId.Exit)
                    {
                        mInteractingWith = null;
                        mExplorationActor.Enable(true);
                        Camera.main.gameObject.GetComponent<ThirdPersonCamera>().Follow(mExplorationActor.transform);
                        ExplorationEventRouter.Instance.NotifyEvent(new ExplorationEvent(ExplorationEvent.EventType.InteractionEnd));
                        UIManager.Instance.RemoveOverlay(UIContainerId.NPCActionSelectView);
                    }
                    else if (interactionInfo.ComponentId == (int)NPCActionSelectView.ComponentId.ActionSelect)
                    {
                        ListInteractionInfo listInfo = interactionInfo as ListInteractionInfo;
                        if (listInfo.ListIdx == 0)
                        {
                            UIManager.Instance.RemoveOverlay(UIContainerId.NPCActionSelectView);

                            mConversation = DB.DBHelper.LoadConversation((int)ConversationId.BanditsInTheHills);
                            mConversationIdx = 0;
                            UIManager.Instance.PostContainer(UIContainerId.ConversationView, this);
                        }
                    }
                }
            }
            break;
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
                            UIManager.Instance.PostContainer(UIContainerId.NPCActionSelectView, this);
                            GameModesModule.LevelManager.GetLoadedLevel().Scenarios[ScenarioId.TheGreatHoldUp].gameObject.SetActive(true);
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
            case UIContainerId.NPCActionSelectView:
            {
                dataProvider = PublishNPCActions();
            }
            break;
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

        string assetName = (currentDialogue.SpeakerIdx == 0 ? PortraitSpriteId.Vendor : PortraitSpriteId.Rheinhardt).ToString();
        string speakerName = currentDialogue.SpeakerIdx == 0 ? "John" : StoryCharacterId.Rheinhardt.ToString();

        dataProvider.Name = speakerName;
        dataProvider.PortraitAssetName = assetName;
        dataProvider.Content = currentDialogue.Content;

        return dataProvider;
    }

    IDataProvider PublishNPCActions()
    {
        NPCActionSelectView.DataProvider dataProvider = new NPCActionSelectView.DataProvider();
        dataProvider.Name = "John";
        dataProvider.PortraitAssetName = PortraitSpriteId.Vendor.ToString();

        Conversation conversation = ConversationUtil.Load(ConversationId.BanditsInTheHills, (int)StoryCharacterId.Asmund);
        dataProvider.NPCActions = new List<string>() { conversation.Header, "Vendor" };

        return dataProvider;
    }

    #endregion // UIContainerControl

    //! IInputHandler
    public void OnKeyPressed(InputSource source, int key, InputState state)
    {
        if (!Interacting())
        {
            switch (source)
            {
                case InputSource.Mouse:
                {
                    if (key == (int)MouseKey.Right
                        && state == InputState.Down
                        && mHoveredInteractable != null
                        && IsInRange())
                    {
                        Interact(mHoveredInteractable);
                    }
                }
                break;
            }
        }
    }

    public void OnMouseHoverChange(GameObject mouseHover)
    {
        if (!Interacting())
        {
            if (mouseHover != null)
            {
                mHoveredInteractable = mouseHover.GetComponentInParent<IInteractable>();
                mDistanceToHovered = DistanceToHoverTarget();
            }
            else
            {
                mHoveredInteractable = null;
            }

            UpdateHoverTargetCursor();
        }
    }

    public void OnMouseScrolled(float scrollDelta)
    {
        // empty
    }

    private void Interact(IInteractable interactable)
    {
        mInteractingWith = interactable;
        ExplorationEventRouter.Instance.NotifyEvent(new ExplorationEvent(ExplorationEvent.EventType.InteractionStart, mInteractingWith));
        ExplorationModule.Instance.MovementDirector.RotateActor(mExplorationActor.ActorController, mInteractingWith.transform, null);
        mExplorationActor.Enable(false);
        Camera.main.gameObject.GetComponent<ThirdPersonCamera>().Interact(mExplorationActor.transform, mInteractingWith.transform);
        UIManager.Instance.SetCursor(CursorControl.CursorType.Default);
        UIManager.Instance.PostContainer(UIContainerId.NPCActionSelectView, this);
    }

    private void UpdateHoverTargetCursor()
    {
        if (mHoveredInteractable != null)
        {
            float distanceTo = DistanceToHoverTarget();
            if (distanceTo < mMinInteractionDistance)
            {
                UIManager.Instance.SetCursor(CursorControl.CursorType.Interact_Near);
            }
            else
            {
                UIManager.Instance.SetCursor(CursorControl.CursorType.Interact_Far);
            }
        }
        else
        {
            UIManager.Instance.SetCursor(CursorControl.CursorType.Default);
        }
       
    }

    float DistanceToHoverTarget()
    {
        return (mExplorationActor.transform.position - mHoveredInteractable.gameObject.transform.position).magnitude;
    }

    bool IsInRange()
    {
        return mDistanceToHovered < mMinInteractionDistance;
    }
    bool Interacting()
    {
        return mInteractingWith != null;
    }
}

