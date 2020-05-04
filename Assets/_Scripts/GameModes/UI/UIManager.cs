using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum UIContainerId
{
    ActorActionsView,
    EncounterIntroView,
    EncounterUnitPlacementView,
    EncounterStatusView,
    EquipmentOutfiterView,
    ExplorationMenuView,
    MainMenuView,
    OutfiterSelectView,
    SpecializationOutfiterView,

    NUM
}

public enum UIInteractionType
{
    None,

    Click,
    MouseOver,
    MouseExit,

    NUM
}

class UIManager : MonoBehaviour
    , IEventHandler<GameModeEvent>
{
    private string TAG = "UIManager";

    public static UIManager Instance;

    private Dictionary<UIContainerId, KeyValuePair<UIContainer, UIContainerControl>> mContainerControlPairs;
    private HashSet<UIContainerId> mContainersToPublish;
    private AssetLoader<UIContainer> mViewLoader = null;
    private AudioSource mUIAudioSource = null;

    public void Initialize()
    {
        Logger.Log(LogTag.UI, TAG, "::Initialize()");
        Logger.Assert(Instance == null, LogTag.UI, TAG, "::Initialize() - Already initialized!");

        Instance = this;

        mContainerControlPairs = new Dictionary<UIContainerId, KeyValuePair<UIContainer, UIContainerControl>>();
        mContainersToPublish = new HashSet<UIContainerId>();

        mViewLoader = new AssetLoader<UIContainer>("UI");
        mViewLoader.LoadAssets("Views");

        mUIAudioSource = gameObject.AddComponent<AudioSource>();
        mUIAudioSource.spatialBlend = 0; // no fall off
        mUIAudioSource.volume = .2f;

        GameModeEventRouter.Instance.RegisterHandler(this);
    }

    private void OnDestroy()
    {
        GameModeEventRouter.Instance.UnRegisterListener(this);
    }

    private void LateUpdate()
    {
        if (mContainersToPublish.Count > 0)
        {
            foreach (UIContainerId id in mContainersToPublish)
            {
                Debug.Assert(mContainerControlPairs.ContainsKey(id));
                IDataProvider publish = mContainerControlPairs[id].Value.Publish();

                Logger.Log(LogTag.UI, TAG, string.Format("Publishing from {0} -> {1}: {2}",
                    mContainerControlPairs[id].Value.Name(), mContainerControlPairs[id].Key.Name(), publish.ToString()));

                mContainerControlPairs[id].Key.Publish(publish);
            }

            mContainersToPublish.Clear();
        }
    }

    public void PlaySFX(SFXId sFXId)
    {
        mUIAudioSource.PlayOneShot(GameModesModule.AudioManager.GetSFXClip(sFXId));
    }

    public void PostContainer(UIContainerId containerId, UIContainerControl provider)
    {
        UIContainer toPost = null;
        Debug.Assert(!mContainerControlPairs.ContainsKey(containerId));

        toPost = Instantiate(mViewLoader.GetAsset(containerId.ToString()), transform).GetComponent<UIContainer>();

        mContainerControlPairs.Add(containerId, new KeyValuePair<UIContainer, UIContainerControl>(toPost, provider));

        Logger.Log(LogTag.UI, TAG, string.Format("{0} Posting Container {1}",provider.Name(), toPost.Name()));

        mContainersToPublish.Add(containerId);
    }

    public void RemoveOverlay(UIContainerId containerId)
    {
        Debug.Assert(mContainerControlPairs.ContainsKey(containerId));
        
        UIContainer overlay = mContainerControlPairs[containerId].Key;
        mContainerControlPairs.Remove(containerId);

        Logger.Log(LogTag.UI, TAG, string.Format("Removing Container {0}", overlay.Name()));

        overlay.gameObject.SetActive(false);
        Destroy(overlay.gameObject);
    }

    public void Publish(UIContainerId containerId)
    {
        Logger.Log(LogTag.UI, TAG, string.Format("Publish Queued for Container {0}", mContainerControlPairs[containerId].Key.Name()));

        mContainersToPublish.Add(containerId);
    }

    public void ComponentInteracted(int containerId, IUIInteractionInfo interactionInfo)
    {
        UIContainerId id = (UIContainerId)containerId;
        Debug.Assert(mContainerControlPairs.ContainsKey(id), string.Format("UIContainer {0} interacted with but is not being managed by UIManager", id.ToString()));

        Logger.Log(LogTag.UI, TAG, string.Format("Container Interacted {0}", id.ToString()));

        mContainerControlPairs[id].Value.HandleComponentInteraction(containerId, interactionInfo);
    }

    public void HandleEvent(GameModeEvent eventInfo)
    {
        switch (eventInfo.Type)
        {
            case GameModeEvent.EventType.UISetup_Begin:
                {
                    Logger.Log(LogTag.UI, TAG, "::HandleEvent() - " + eventInfo.Type.ToString());

                    GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.UISetup_Complete));
                }
                break;

            case GameModeEvent.EventType.UITakedown_Begin:
                {
                    Logger.Log(LogTag.UI, TAG, "::HandleEvent() - " + eventInfo.Type.ToString());

                    Logger.Assert(transform.childCount == 0, LogTag.UI, TAG, "Left over UI elements on UITakedown", LogLevel.Warning);
                    for (int i = 0; i < transform.childCount; ++i)
                    {
                        Logger.Log(LogTag.UI, TAG, string.Format("    {0}. {1}", i, transform.GetChild(i).name), LogLevel.Warning);
                        Destroy(transform.GetChild(i).gameObject);
                    }
                    mContainerControlPairs.Clear();

                    GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.UITakedown_Complete));
                }
                break;
        }
    }
}
