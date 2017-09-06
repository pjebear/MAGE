using Common.StatusEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EncounterSystem.Screen
{
    using Screens.Payloads;
    class StatusEffectIconManager : MonoBehaviour
    {
        public StatusEffectIcon StatusEffectIconPrefab = null;
        public Transform BeneficialIconsOffset;
        public Transform HarmfulIconsOffset;

        public int NumIconsPerRow = 10;
        private StatusEffectIcon[] mBeneficialStatusEffectIcons;
        private StatusEffectIcon[] mHarmfulStatusEffectIcons;
        static Sprite[] mLoadedSpriteIcons;

        private string IconAssetPath = "StatusIcons/";
        private string IconAssetPrefix = "status_";


        
        void Awake()
        {
            Debug.Assert(StatusEffectIconPrefab != null);
            mBeneficialStatusEffectIcons = new StatusEffectIcon[NumIconsPerRow];
            mHarmfulStatusEffectIcons = new StatusEffectIcon[NumIconsPerRow];
        }

        public void Initialize(float iconSize)
        {
            if (mLoadedSpriteIcons == null)
            {
                mLoadedSpriteIcons = new Sprite[(int)StatusEffectIndex.NUM];
                LoadIconImages();
            }

            for (int i = 0; i < NumIconsPerRow; i++)
            {
                mBeneficialStatusEffectIcons[i] = Instantiate(StatusEffectIconPrefab, BeneficialIconsOffset);
                mBeneficialStatusEffectIcons[i].transform.localPosition = Vector3.right * (i * iconSize);
                mBeneficialStatusEffectIcons[i].Initialize(iconSize);
                mBeneficialStatusEffectIcons[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < NumIconsPerRow; i++)
            {
                mHarmfulStatusEffectIcons[i] = Instantiate(StatusEffectIconPrefab, HarmfulIconsOffset);
                mHarmfulStatusEffectIcons[i].transform.localPosition = Vector3.right * (i * iconSize);
                mBeneficialStatusEffectIcons[i].Initialize(iconSize);
                mHarmfulStatusEffectIcons[i].gameObject.SetActive(false);
            }
        }

        public void SetIcons(List<StatusEffectIconPayload> payloads)
        {
            int beneficialIconIdx = 0;
            int harmfulIconIdx = 0 ;
            foreach (StatusEffectIconPayload payload in payloads)
            {
                if (payload.IsBeneficial)
                {
                    if (beneficialIconIdx < NumIconsPerRow)
                    {
                        mBeneficialStatusEffectIcons[beneficialIconIdx++].SetIcon(mLoadedSpriteIcons[payload.AssetId], payload.ToolTip, payload.NumStacks, payload.IsBeneficial);
                    }
                }
                else
                {
                    if (harmfulIconIdx < NumIconsPerRow)
                    {
                        mHarmfulStatusEffectIcons[harmfulIconIdx++].SetIcon(mLoadedSpriteIcons[payload.AssetId], payload.ToolTip, payload.NumStacks, payload.IsBeneficial);
                    }
                }
            }
        }

        public void Hide()
        {
            for (int i = 0; i < NumIconsPerRow; ++i)
            {
                mBeneficialStatusEffectIcons[i].gameObject.SetActive(false);
                mHarmfulStatusEffectIcons[i].gameObject.SetActive(false);
            }
        }

        

        private void LoadIconImages()
        {
            Sprite defaultSprite = Resources.Load<Sprite>(IconAssetPath + "default");
            for (int i = 0; i < mLoadedSpriteIcons.Length; ++i)
            {
                Sprite iconSprite = Resources.Load<Sprite>(IconAssetPath + IconAssetPrefix + i);
                if (iconSprite != null)
                {
                    mLoadedSpriteIcons[i] = iconSprite;
                }
                else
                {
                    mLoadedSpriteIcons[i] = defaultSprite;
                }
            }
        }
    }
}
