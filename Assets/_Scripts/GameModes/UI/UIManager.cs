using MAGE.GameModes;
using MAGE.GameModes.FlowControl;
using MAGE.UI.Views;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MAGE.UI
{
    public enum UIContainerId
    {
        ActorActionsView,
        ConversationView,
        ContainerInspectView,
        EncounterCharacterInfoLeftView,
        EncounterCharacterInfoRightView,
        EncounterIntroView,
        EncounterUnitPlacementView,
        EncounterStatusView,
        EquipmentOutfiterView,
        ExplorationMenuView,
        MainMenuView,
        MapView,
        NPCActionSelectView,
        OutfiterSelectView,
        QuestLogView,
        SpecializationOutfiterView,
        VendorView,

        NUM
    }


    class UIManager : MonoBehaviour
        , Messaging.IMessageHandler
    {
        private string TAG = "UIManager";

        public static UIManager Instance;

        public Transform OverlayParent;
        public Image FadeSkrim;

        private Dictionary<UIContainerId, KeyValuePair<UIContainer, UIContainerControl>> mContainerControlPairs;
        private HashSet<UIContainerId> mContainersToPublish;
        private AssetLoader<UIContainer> mViewLoader = null;
        private AssetLoader<Sprite> mSpriteLoader = null;
        private AudioSource mUIAudioSource = null;
        private CursorControl mCursorControl = null;

        private Coroutine mFadeCoroutine;

        public void Initialize()
        {
            Logger.Log(LogTag.UI, TAG, "::Initialize()");
            Logger.Assert(Instance == null, LogTag.UI, TAG, "::Initialize() - Already initialized!");

            Instance = this;

            mContainerControlPairs = new Dictionary<UIContainerId, KeyValuePair<UIContainer, UIContainerControl>>();
            mContainersToPublish = new HashSet<UIContainerId>();

            mViewLoader = new AssetLoader<UIContainer>(Path.Combine("UI", "Views"));
            mViewLoader.LoadAssets();

            mSpriteLoader = new AssetLoader<Sprite>(Path.Combine("UI", "Sprites"));
            mSpriteLoader.LoadAssets();

            mCursorControl = new CursorControl();
            mCursorControl.SetCursorState(CursorControl.CursorType.Default);

            mUIAudioSource = gameObject.AddComponent<AudioSource>();
            mUIAudioSource.spatialBlend = 0; // no fall off
            mUIAudioSource.volume = .2f;

            Messaging.MessageRouter.Instance.RegisterHandler(this);
        }

        private void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        private void LateUpdate()
        {
            if (mContainersToPublish.Count > 0)
            {
                foreach (UIContainerId id in mContainersToPublish)
                {
                    Debug.Assert(mContainerControlPairs.ContainsKey(id));
                    IDataProvider publish = mContainerControlPairs[id].Value.Publish((int)id);

                    //Logger.Log(LogTag.UI, TAG, string.Format("Publishing from {0} -> {1}: {2}",
                    //    mContainerControlPairs[id].Value.Name(), mContainerControlPairs[id].Key.Name(), publish.ToString()));

                    mContainerControlPairs[id].Key.Publish(publish);
                }

                mContainersToPublish.Clear();
            }
        }

        public void PlaySFX(SFXId sFXId)
        {
            mUIAudioSource.PlayOneShot(AudioManager.Instance.GetSFXClip(sFXId));
        }

        public void SetCursor(CursorControl.CursorType cursorType)
        {
            mCursorControl.SetCursorState(cursorType);
        }

        public Sprite LoadSprite(string assetName)
        {
            return mSpriteLoader.GetAsset(assetName);
        }

        public void PostContainer(UIContainerId containerId, UIContainerControl provider)
        {
            UIContainer toPost = null;
            Logger.Assert(!mContainerControlPairs.ContainsKey(containerId), LogTag.UI, TAG, string.Format("PostContainer() - Container [{0}] already active!", containerId.ToString()));
            if (!mContainerControlPairs.ContainsKey(containerId))
            {
                toPost = Instantiate(mViewLoader.GetAsset(containerId.ToString()), OverlayParent).GetComponent<UIContainer>();
                toPost.Init((int)containerId, null);
                mContainerControlPairs.Add(containerId, new KeyValuePair<UIContainer, UIContainerControl>(toPost, provider));

                //Logger.Log(LogTag.UI, TAG, string.Format("{0} Posting Container {1}",provider.Name(), toPost.Name()));

                mContainersToPublish.Add(containerId);
            }
        }

        public void RemoveOverlay(UIContainerId containerId)
        {
            Logger.Assert(mContainerControlPairs.ContainsKey(containerId), LogTag.UI, TAG, string.Format("RemoveOverlay() - Overlay [{0}] wasn't active", containerId.ToString()));
            if (mContainerControlPairs.ContainsKey(containerId))
            {
                UIContainer overlay = mContainerControlPairs[containerId].Key;
                mContainerControlPairs.Remove(containerId);

                //Logger.Log(LogTag.UI, TAG, string.Format("Removing Container {0}", overlay.Name()));

                overlay.gameObject.SetActive(false);
                Destroy(overlay.gameObject);
            }
        }

        public void Publish(UIContainerId containerId)
        {
            //Logger.Log(LogTag.UI, TAG, string.Format("Publish Queued for Container {0}", mContainerControlPairs[containerId].Key.Name()));

            mContainersToPublish.Add(containerId);
        }

        public void ComponentInteracted(int containerId, UIInteractionInfo interactionInfo)
        {
            UIContainerId id = (UIContainerId)containerId;
            Debug.Assert(mContainerControlPairs.ContainsKey(id), string.Format("UIContainer {0} interacted with but is not being managed by UIManager", id.ToString()));

            Logger.Log(LogTag.UI, TAG, string.Format("Container Interacted {0}", id.ToString()));

            mContainerControlPairs[id].Value.HandleComponentInteraction(containerId, interactionInfo);
        }

        public void HandleMessage(Messaging.MessageInfoBase messageInfoBase)
        {
            switch (messageInfoBase.MessageId)
            {
                case GameModeMessage.Id:
                {
                    GameModeMessage gameModeMessage = messageInfoBase as GameModeMessage;

                    switch (gameModeMessage.Type)
                    {
                        case GameModeMessage.EventType.FadeIn:
                        {
                            Fade(true);
                        }
                        break;

                        case GameModeMessage.EventType.FadeOut:
                        {
                            Fade(false);
                        }
                        break;

                        case GameModeMessage.EventType.UISetup_Begin:
                        {
                            Logger.Log(LogTag.UI, TAG, "::HandleMessage() - " + gameModeMessage.Type.ToString());

                            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.UISetup_Complete));
                        }
                        break;

                        case GameModeMessage.EventType.UITakedown_Begin:
                        {
                            Logger.Log(LogTag.UI, TAG, "::HandleMessage() - " + gameModeMessage.Type.ToString());

                            Logger.Assert(OverlayParent.childCount == 0, LogTag.UI, TAG, "Left over UI elements on UITakedown", LogLevel.Warning);
                            for (int i = 0; i < OverlayParent.childCount; ++i)
                            {
                                Logger.Log(LogTag.UI, TAG, string.Format("    {0}. {1}", i, OverlayParent.GetChild(i).name), LogLevel.Warning);
                                Destroy(OverlayParent.GetChild(i).gameObject);
                            }
                            mContainerControlPairs.Clear();

                            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.UITakedown_Complete));
                        }
                        break;

                        case GameModeMessage.EventType.ModeStart:
                        {
                            if (mFadeCoroutine != null)
                            {
                                StopCoroutine(mFadeCoroutine);
                            }
                            mFadeCoroutine = StartCoroutine(_FadeInOut(true, .5f));
                        }
                        break;

                        case GameModeMessage.EventType.ModeEnd:
                        {
                            if (mFadeCoroutine != null)
                            {
                                StopCoroutine(mFadeCoroutine);
                            }
                            mFadeCoroutine = StartCoroutine(_FadeInOut(false, .5f));
                        }
                        break;
                    }
                }
                break;
                case FlowMessage.Id:
                {
                    FlowMessage flowMessage = messageInfoBase as FlowMessage;

                    switch (flowMessage.Type)
                    {
                        case FlowMessage.EventType.Notify:
                        {
                            string notifyArg = flowMessage.Arg<string>();
                            switch (notifyArg)
                            {
                                case "fadeIn":
                                {
                                    Fade(true);
                                }
                                break;
                                case "fadeOut":
                                {
                                    Fade(false);
                                }
                                break;
                            }
                            
                        }
                        break;
                    }
                }
                break;
            }
        }

        public void Fade(bool fadeIn, float overSeconds = 0.5f)
        {
            Logger.Log(LogTag.UI, TAG, string.Format("Fade() - [{0}]", fadeIn ? "In" : "Out"));
            Logger.Assert(mFadeCoroutine != null, LogTag.UI, TAG, "FadeIn() - Fade coroutine was already active");
            if (mFadeCoroutine != null)
            {
                StopCoroutine(mFadeCoroutine);
            }

            mFadeCoroutine = StartCoroutine(_FadeInOut(fadeIn, overSeconds));
        }

        private IEnumerator _FadeInOut(bool fadeIn, float overSeconds)
        {
            float duration = 0;

            while (duration < overSeconds)
            {
                duration += Time.deltaTime;

                float progress = duration / overSeconds;
                if (fadeIn)
                {
                    progress = 1 - progress;
                }

                Color updatedColor = FadeSkrim.color;
                updatedColor.a = progress;
                FadeSkrim.color = updatedColor;

                yield return new WaitForFixedUpdate();
            }

            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.FadeComplete));
            Messaging.MessageRouter.Instance.NotifyMessage(new FlowMessage(FlowMessage.EventType.FlowEvent, "fadeComplete"));
        }
    }

}

