using MAGE.GameModes.Exploration;
using MAGE.GameSystems;
using MAGE.GameSystems.Appearances;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MAGE.GameModes.SceneElements
{
    class CinematicDialogueControl
        : MonoBehaviour
        , UI.Views.UIContainerControl
    {
        [System.Serializable]
        public struct DialogueInfo
        {
            public int SpeakerIdx;
            public string Content;
        }

        int mDialogueIdx = -1;
        bool mIsDialoguePosted = false;

        public List<CharacterPickerControl> Speakers = new List<CharacterPickerControl>();

        public List<DialogueInfo> Dialogue = new List<DialogueInfo>();

        void Awake()
        {
            
        }

        void OnDisable()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.RemoveOverlay(UIContainerId.ConversationView);
            }
            
            mIsDialoguePosted = false;
        }

        public void ShowDialogue()
        {
            mDialogueIdx++;
            
            if (mIsDialoguePosted)
            {
                UIManager.Instance.Publish(UIContainerId.ConversationView);
            }
            else
            {
                mIsDialoguePosted = true;
                UIManager.Instance.PostContainer(UIContainerId.ConversationView, this);
            }
        }

        public void HideDialogue()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.ConversationView);
            mIsDialoguePosted = false;
        }

        public IDataProvider Publish(int containerId)
        {
            IDataProvider dataProvider = null;
            if (containerId == (int)UIContainerId.ConversationView)
            {
                ConversationView.DataProvider dp = new ConversationView.DataProvider();

                Debug.Assert(mDialogueIdx < Dialogue.Count);
                if (mDialogueIdx < Dialogue.Count)
                {
                    CharacterPickerControl characterPickerControl = Speakers[Dialogue[mDialogueIdx].SpeakerIdx];
                    dp.Name = "TODO";
                    dp.PortraitAssetName = characterPickerControl.Appearance.PortraitSpriteId.ToString();
                    dp.Content = Dialogue[mDialogueIdx].Content;
                }

                dataProvider = dp;
            }

            return dataProvider;
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            // empty
        }

        public string Name()
        {
            return "CinematicMoment";
        }
    }
}
