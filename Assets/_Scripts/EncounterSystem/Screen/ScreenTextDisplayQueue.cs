using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EncounterSystem.Screen
{
    class ScreenTextDisplayQueue : MonoBehaviour
    {
        public float VerticalOffset = .5f;
        public Text TextPrefab = null;
        private Text mOnScreenText = null;
        private Queue<ScreenTextElement> mScreenTextQueue = null;
        float mTextDuration = -1f;

        void Awake()
        {
            mScreenTextQueue = new Queue<ScreenTextElement>();
            mOnScreenText = Instantiate(TextPrefab).GetComponent<Text>();
            mOnScreenText.transform.SetParent(GameObject.Find("Canvas").gameObject.transform); // gameObject = unit
        }

        private void SetOnScreenText(ScreenTextElement toSet)
        {
            mOnScreenText.text = toSet.Text;
            mOnScreenText.fontSize = toSet.FontSize;
            mOnScreenText.color = toSet.Color;
            mTextDuration = toSet.Duration;
        }

        // Update is called once per frame
        void Update()
        {
            if (mOnScreenText.IsActive())
            {
                mTextDuration -= Time.deltaTime;
                if (mTextDuration <= 0)
                {
                    if (mScreenTextQueue.Count > 0)
                    {
                        SetOnScreenText(mScreenTextQueue.Dequeue());
                    }
                    else
                    {
                        mOnScreenText.gameObject.SetActive(false);
                    }
                }
                mOnScreenText.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + Vector3.up * VerticalOffset);
            }
        }

        public void QueueOnScreenText(string text, Color color, float duration = 1f )
        {
            ScreenTextElement toQueue = new ScreenTextElement(duration, color, text);

            if (mOnScreenText.IsActive()) // if already active, there is atleast already 1 text being displayed. Enqueue
            {
                mScreenTextQueue.Enqueue(toQueue);
            }
            else // no other text is active. Directly display text
            {
                mOnScreenText.gameObject.SetActive(true);
                SetOnScreenText(toQueue);
            }  
        }

        private struct ScreenTextElement 
        {
            public float Duration;
            public Color Color;
            public string Text;
            public int FontSize;

            public ScreenTextElement(float duration, Color color, string text, int fontSize = 15)
            {
                Duration = duration;
                Color = color;
                Text = text;
                FontSize = fontSize;
            }
        }
    }
}

