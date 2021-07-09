using MAGE.Common;
using MAGE.GameSystems;
using MAGE.GameSystems.Appearances;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace MAGE.GameModes.SceneElements
{
    class ActorOutfitter : MonoBehaviour
    {
        class HeldApparelSlot
        {
            public GameObject InstantiatedObj;
            public HeldRegion HeldRegion;
            public HolsterRegion HolsterRegion;
        }

        class HeldApparelSlots
        {
            private HeldApparelSlot[,] mHeldApparel;

            public HeldApparelSlots()
            {
                mHeldApparel = new HeldApparelSlot[(int)HumanoidActorConstants.HeldApparelType.NUM, (int)HumanoidActorConstants.Hand.NUM];
                for (int i = 0; i < (int)HumanoidActorConstants.HeldApparelType.NUM; ++i)
                {
                    for (int j = 0; j < (int)HumanoidActorConstants.Hand.NUM; ++j)
                    {
                        mHeldApparel[i, j] = new HeldApparelSlot();
                    }
                }
            }

            public HeldApparelSlot ApparelAt(HumanoidActorConstants.HeldApparelType type, HumanoidActorConstants.Hand hand)
            {
                return mHeldApparel[(int)type, (int)hand];
            }
        }

        private ActorAnimator rActorAnimator;
        private Body rBody;
        //private AssetLoader<Body> mBodyLoader = new AssetLoader<Body>("Props/Bodies");
        //private AssetLoader<SkinTone> mSkinToneLoader = new AssetLoader<SkinTone>("Props/Bodies/Humanoid/SkinMaterials");
        //private AssetLoader<OutfitArrangement> mOutfitLoader = new AssetLoader<OutfitArrangement>("Props/Apparel/Arrangements");
        //private AssetLoader<HeldApparel> mHeldItemLoader = new AssetLoader<HeldApparel>("Props/Apparel/Held");
        //private AssetLoader<GameObject> mHairLoader = new AssetLoader<GameObject>("Props/Apparel/Hair");

        private OutfitColorization mOutfitColorization = OutfitColorization.Allied;
        private HumanoidActorConstants.HeldApparelState mHeldApparelState = HumanoidActorConstants.HeldApparelState.Holstered;

        private HeldApparelSlots mHeldApparel = new HeldApparelSlots();
        private OutfitArrangement mOutfit = null;
        private GameObject mHair = null;
        private GameObject mFacialHair = null;

        private void Awake()
        {
            //LoadAssets();
            //mSkinToneLoader.LoadAssets();
            //mOutfitLoader.LoadAssets();
            //mHeldItemLoader.LoadAssets();
            //mHairLoader.LoadAssets();

            rActorAnimator = GetComponentInParent<ActorAnimator>();
            rBody = GetComponent<Body>();
        }

        public void UpdateAppearance(Appearance appearance)
        {
            // Outfit
            if (mOutfit != null && mOutfit.OutfitArrangementType != appearance.OutfitType)
            {
                Destroy(mOutfit.gameObject);
                mOutfit = null;
            }

            if (mOutfit == null)
            {
                if (appearance.OutfitType != ApparelAssetId.NONE)
                {
                    SetOutfit(appearance.OutfitType);
                }
                else
                {
                    SetOutfit(ApparelAssetId.Default);
                }
            }

            // Skin Tone
            SkinTone skinTone = ResourceLoader.Load<SkinTone>(ResourceConstants.HumanoidSkinTonePath, string.Format("SkinTone_{0}", (int)appearance.SkinToneType)); //mSkinToneLoader.GetAsset(string.Format("SkinTone_{0}", (int)appearance.SkinToneType));
            rBody.BodyMesh.materials[(int)HumanoidActorConstants.BodyMaterialSlot.Body].SetTexture("_MainTex", skinTone.BodyTexture);
            rBody.BodyMesh.materials[(int)HumanoidActorConstants.BodyMaterialSlot.Head].SetTexture("_MainTex", skinTone.HeadTexture);

            // Hair
            Color hairColor = HumanoidActorConstants.HairColors[(int)appearance.HairColor];

            // Head Hair
            if (mHair != null && mHair.name != appearance.HairType.ToString())
            {
                Destroy(mHair);
                mHair = null;
            }
            if (mHair == null)
            {
                mHair = Instantiate(ResourceLoader.Load<GameObject, HairType>(ResourceConstants.HairPath, appearance.HairType));
                mHair.transform.SetParent(rBody.BodyMesh.transform);
                SkinnedMeshRenderer hairMesh = mHair.GetComponentInChildren<SkinnedMeshRenderer>();
                ReskinMesh.Reskin(hairMesh, rBody.BodyMesh);
                hairMesh.materials[(int)HumanoidActorConstants.HairMaterialSlot.Color].color = hairColor;
            }
           
            // Facial Hair
            if (mFacialHair != null && mFacialHair.name != appearance.FacialHairType.ToString())
            {
                Destroy(mFacialHair);
                mFacialHair = null;
            }
            if (mFacialHair == null && appearance.FacialHairType != FacialHairType.None)
            {
                mFacialHair = Instantiate(ResourceLoader.Load<GameObject, FacialHairType>(ResourceConstants.HairPath, appearance.FacialHairType), rBody.FacialHairTransform);
                MeshRenderer facialHairMesh = mFacialHair.GetComponent<MeshRenderer>();
                facialHairMesh.materials[(int)HumanoidActorConstants.HairMaterialSlot.Color].color = hairColor;
            }

            // Eye brows
            rBody.BodyMesh.materials[(int)HumanoidActorConstants.BodyMaterialSlot.EyeBrow].color = hairColor;

            // Held Items
            ApplyHeldApparel(appearance.LeftHeldAssetId, HumanoidActorConstants.HeldApparelType.Melee, HumanoidActorConstants.Hand.Left);
            ApplyHeldApparel(appearance.RightHeldAssetId, HumanoidActorConstants.HeldApparelType.Melee, HumanoidActorConstants.Hand.Right);
            ApplyHeldApparel(appearance.RangedAssetId, HumanoidActorConstants.HeldApparelType.Ranged, HumanoidActorConstants.Hand.Right);
        }

        public void SetOutfitColorization(OutfitColorization outfitColorization)
        {
            mOutfitColorization = outfitColorization;
            if (mOutfit != null)
            {
                foreach (SkinnedMeshRenderer toColor in mOutfit.ColorableMeshes)
                {
                    toColor.materials[0].color = GetColorFromColorization(mOutfitColorization);
                    toColor.materials[0].SetColor("_EMISSION", GetColorFromColorization(mOutfitColorization));
                    toColor.materials[0].EnableKeyword("_EMISSION");
                }
            }
        }

        Color GetColorFromColorization(OutfitColorization colorization)
        {
            Color outfitColor = Color.white;
            switch (mOutfitColorization)
            {
                case OutfitColorization.Allied: outfitColor = Color.white; break;
                case OutfitColorization.Enemy: outfitColor = Color.red; break;
            }

            return outfitColor;
        }

        void SetOutfit(ApparelAssetId outfitAssetId)
        {
            mOutfit = Instantiate(ResourceLoader.Load<OutfitArrangement, ApparelAssetId>(ResourceConstants.OutfitsPath, outfitAssetId));

            mOutfit.transform.SetParent(rBody.transform);
            foreach (SkinnedMeshRenderer outfitItem in mOutfit.GetArrangement())
            {
                ReskinMesh.Reskin(outfitItem, rBody.BodyMesh);
            }

            SetOutfitColorization(mOutfitColorization);
        }

        public void ApplyHeldApparel(ApparelAssetId heldApparel, HumanoidActorConstants.HeldApparelType type, HumanoidActorConstants.Hand hand)
        {
            HeldApparelSlot heldApparelSlot = mHeldApparel.ApparelAt(type, hand);
            if (heldApparelSlot.InstantiatedObj != null 
                && heldApparelSlot.InstantiatedObj.name != heldApparel.ToString())
            {
                Destroy(heldApparelSlot.InstantiatedObj);
                heldApparelSlot.InstantiatedObj = null;
            }

            if (heldApparelSlot.InstantiatedObj == null
                && heldApparel != ApparelAssetId.NONE)
            {
                HeldApparel apparelInfo = ResourceLoader.Load<HeldApparel, ApparelAssetId>(ResourceConstants.HeldApparelPath, heldApparel);
                heldApparelSlot.HeldRegion = apparelInfo.HeldRegion;
                heldApparelSlot.HolsterRegion = apparelInfo.HolsterRegion;

                Transform apparelParent = GetTransformForHeldItem(heldApparelSlot, type, hand);
                GameObject apparelObj = Instantiate(apparelInfo.ApparelObj, apparelParent, false);
                heldApparelSlot.InstantiatedObj = apparelObj;

                bool isHandGrasped = false;

                if (type == HumanoidActorConstants.HeldApparelType.Melee)
                {
                    if (mHeldApparelState == HumanoidActorConstants.HeldApparelState.MeleeHeld
                        && apparelInfo.HeldRegion == HeldRegion.Hand)
                    {
                        isHandGrasped = true;
                    }
                }
                else
                {
                    if (mHeldApparelState == HumanoidActorConstants.HeldApparelState.RangedHeld
                        && apparelInfo.HeldRegion == HeldRegion.Hand)
                    {
                        isHandGrasped = true;
                    }
                }

                rActorAnimator.SetHandGraspState(hand, isHandGrasped);
            }
        }

        public void UpdateHeldApparelState(HumanoidActorConstants.HeldApparelState state, bool immediate)
        {
            mHeldApparelState = state;

            if (immediate)
            {
                UpdateHeldApparelLocation();
                rActorAnimator.SetHandGraspState(HumanoidActorConstants.Hand.Left, mHeldApparelState != HumanoidActorConstants.HeldApparelState.Holstered);
                rActorAnimator.SetHandGraspState(HumanoidActorConstants.Hand.Right, mHeldApparelState != HumanoidActorConstants.HeldApparelState.Holstered);
            }
            else
            {
                StartCoroutine(_DelayedApparelUpdate());
            }
        }

        private IEnumerator _DelayedApparelUpdate()
        {
            if (rActorAnimator != null)
            {
                rActorAnimator.AnimateHoldItem(HumanoidActorConstants.Hand.Right, mHeldApparelState != HumanoidActorConstants.HeldApparelState.Holstered);
                rActorAnimator.SetHandGraspState(HumanoidActorConstants.Hand.Left, mHeldApparelState != HumanoidActorConstants.HeldApparelState.Holstered);
            }
            
            yield return new WaitForSeconds(1);

            UpdateHeldApparelLocation();
        }

        public void UpdateHeldApparelLocation()
        {
            for (int i = 0; i < (int)HumanoidActorConstants.HeldApparelType.NUM; ++i)
            {
                for (int j = 0; j < (int)HumanoidActorConstants.Hand.NUM; ++j)
                {
                    HumanoidActorConstants.HeldApparelType type = (HumanoidActorConstants.HeldApparelType)i;
                    HumanoidActorConstants.Hand hand = (HumanoidActorConstants.Hand)j;
                    HeldApparelSlot slot = mHeldApparel.ApparelAt(type, hand);
                    if (slot.InstantiatedObj != null)
                    {
                        Transform updatedTransform = GetTransformForHeldItem(slot, type, hand);
                        slot.InstantiatedObj.transform.SetParent(updatedTransform, false);
                    }
                }
            }
            
        }

        private Transform GetTransformForHeldItem(HeldApparelSlot apparelSlot, HumanoidActorConstants.HeldApparelType type, HumanoidActorConstants.Hand hand)
        {
            Transform transform = null;

            if ((type == HumanoidActorConstants.HeldApparelType.Melee && mHeldApparelState == HumanoidActorConstants.HeldApparelState.RangedHeld) 
                || (type == HumanoidActorConstants.HeldApparelType.Ranged && mHeldApparelState == HumanoidActorConstants.HeldApparelState.MeleeHeld)
                || (mHeldApparelState == HumanoidActorConstants.HeldApparelState.Holstered))
            {
                switch (apparelSlot.HolsterRegion)
                {
                    case HolsterRegion.BackSide: transform = hand == HumanoidActorConstants.Hand.Left ? rBody.BackHolsterLTransform : rBody.BackHolsterRTransform; break;
                    case HolsterRegion.BackCenter: transform = rBody.ShieldHolsterTransform; break;
                }
            }
            else
            {
                switch (apparelSlot.HeldRegion)
                {
                    case HeldRegion.Hand: transform = hand == HumanoidActorConstants.Hand.Left ? rBody.LeftHandTransform : rBody.RightHandTransform; break;
                    case HeldRegion.Arm: transform = hand == HumanoidActorConstants.Hand.Left ? rBody.LeftShieldTransform : rBody.RightSheidlTransform; break;
                }
            }

                switch (mHeldApparelState)
            {
                case HumanoidActorConstants.HeldApparelState.RangedHeld:
                {

                }
                break;
                case HumanoidActorConstants.HeldApparelState.MeleeHeld:
                {
                    
                }
                break;

                case HumanoidActorConstants.HeldApparelState.Holstered:
                {
                    
                }
                break;
            }

            return transform;
        }
    }
}


