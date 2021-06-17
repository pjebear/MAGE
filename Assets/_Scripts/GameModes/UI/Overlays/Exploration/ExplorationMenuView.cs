using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MAGE.UI.Views
{
    class ExplorationMenuView
       : UIContainer
    {
        public enum ComponentId
        {
            OutfiterBtn,
            SaveBtn,
            ExitBtn,
            MapBtn,
            QuestLog,
        }
        public class DataProvider : IDataProvider
        {
            public bool HasUpdatedQuests = false;
            public List<KeyValuePair<string, string>> UpdateMessages = new List<KeyValuePair<string, string>>();
        }

        struct BannerMessage
        {
            public string message;
            public string info;
        }
        private Queue<BannerMessage> mUpdateMessages = new Queue<BannerMessage>();

        public UIButton OutfiterBtn;
        public UIButton SaveBtn;
        public UIButton ExitBtn;
        public UIButton MapBtn;
        public UIButton QuestLog;
        public GameObject QuestUpdatedDot;
        public GameObject UpdatedMessageBanner;
        public UIText UpdatedMessageTxt;
        public UIText UpdatedMessageInfoTxt;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = dataProvider as DataProvider;

            QuestUpdatedDot.SetActive(dp.HasUpdatedQuests);

            foreach (KeyValuePair<string,string> message in dp.UpdateMessages)
            {
                BannerMessage bannerMessage = new BannerMessage();
                bannerMessage.message = message.Key;    
                bannerMessage.info = message.Value;
                

                mUpdateMessages.Enqueue(bannerMessage);

                if (!UpdatedMessageBanner.gameObject.activeSelf)
                {
                    StartCoroutine(_DisplayBannerMessage());
                }
            }
        }

        protected override void InitChildren()
        {
            OutfiterBtn.Init((int)ComponentId.OutfiterBtn, this);
            SaveBtn.Init((int)ComponentId.SaveBtn, this);
            ExitBtn.Init((int)ComponentId.ExitBtn, this);
            MapBtn.Init((int)ComponentId.MapBtn, this);
            QuestLog.Init((int)ComponentId.QuestLog, this);
        }

        IEnumerator _DisplayBannerMessage()
        {
            BannerMessage bannerMessage = mUpdateMessages.Peek();
            mUpdateMessages.Dequeue();

            UpdatedMessageBanner.gameObject.SetActive(true);
            UpdatedMessageTxt.Publish(bannerMessage.message);
            UpdatedMessageInfoTxt.Publish(bannerMessage.info);

            yield return new WaitForSeconds(3);

            if (mUpdateMessages.Count > 0)
            {
                StartCoroutine(_DisplayBannerMessage());
            }
            else
            {
                UpdatedMessageBanner.gameObject.SetActive(false);
            }
        }
    }

}

