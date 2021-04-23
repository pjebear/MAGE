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
        struct HeldApparelSlot
        {
            public GameObject InstantiatedObj;
            public HeldRegion HeldRegion;
            public HolsterRegion HolsterRegion;
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
        private HeldApparelSlot[] mHeldApparelSlots = new HeldApparelSlot[(int)HumanoidActorConstants.Hand.NUM];
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
            ApplyHeldApparel(appearance.LeftHeldAssetId, HumanoidActorConstants.Hand.Left);
            ApplyHeldApparel(appearance.RightHeldAssetId, HumanoidActorConstants.Hand.Right);
        }

        public void SetOutfitColorization(OutfitColorization outfitColorization)
        {
            mOutfitColorization = outfitColorization;
            if (mOutfit != null)
            {
                foreach (SkinnedMeshRenderer toColor in mOutfit.ColorableMeshes)
                {
                    toColor.materials[0].color = GetColorFromColorization(mOutfitColorization);
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

        public void ApplyHeldApparel(ApparelAssetId heldApparel, HumanoidActorConstants.Hand side)
        {
            int slotIdx = (int)side;
            if (mHeldApparelSlots[slotIdx].InstantiatedObj != null 
                && mHeldApparelSlots[slotIdx].InstantiatedObj.name != heldApparel.ToString())
            {
                Destroy(mHeldApparelSlots[slotIdx].InstantiatedObj);
                mHeldApparelSlots[slotIdx].InstantiatedObj = null;
            }

            if (mHeldApparelSlots[slotIdx].InstantiatedObj == null
                && heldApparel != ApparelAssetId.NONE)
            {
                HeldApparel apparelInfo = ResourceLoader.Load<HeldApparel, ApparelAssetId>(ResourceConstants.HeldApparelPath, heldApparel);
                mHeldApparelSlots[slotIdx].HeldRegion = apparelInfo.HeldRegion;
                mHeldApparelSlots[slotIdx].HolsterRegion = apparelInfo.HolsterRegion;

                Transform apparelParent = GetTransformForHeldItem(mHeldApparelSlots[slotIdx], side);
                GameObject apparelObj = Instantiate(apparelInfo.ApparelObj, apparelParent, false);
                mHeldApparelSlots[slotIdx].InstantiatedObj = apparelObj;

                if (mHeldApparelState == HumanoidActorConstants.HeldApparelState.Held
                    && apparelInfo.HeldRegion == HeldRegion.Hand)
                {
                    rActorAnimator.SetHandGraspState(side, true);
                }
            }
        }

        public void UpdateHeldApparelState(HumanoidActorConstants.HeldApparelState state)
        {
            mHeldApparelState = state;
            StartCoroutine(_DelayedApparelUpdate());
        }

        private IEnumerator _DelayedApparelUpdate()
        {
            if (rActorAnimator != null)
            {
                rActorAnimator.AnimateHoldItem(HumanoidActorConstants.Hand.Right, mHeldApparelState == HumanoidActorConstants.HeldApparelState.Held);
                rActorAnimator.SetHandGraspState(HumanoidActorConstants.Hand.Left, mHeldApparelState == HumanoidActorConstants.HeldApparelState.Held);
            }
            
            yield return new WaitForSeconds(1);

            UpdateHeldApparelLocation();
        }

        public void UpdateHeldApparelLocation()
        {
            for (int i = 0; i < mHeldApparelSlots.Length; ++i)
            {
                if (mHeldApparelSlots[i].InstantiatedObj != null)
                {
                    Transform updatedTransform = GetTransformForHeldItem(mHeldApparelSlots[i], (HumanoidActorConstants.Hand)i);
                    mHeldApparelSlots[i].InstantiatedObj.transform.SetParent(updatedTransform, false);
                }
            }
        }

        private Transform GetTransformForHeldItem(HeldApparelSlot apparelSlot, HumanoidActorConstants.Hand side)
        {
            Transform transform = null;

            switch (mHeldApparelState)
            {
                case HumanoidActorConstants.HeldApparelState.Held:
                {
                    switch (apparelSlot.HeldRegion)
                    {
                        case HeldRegion.Hand: transform = side == HumanoidActorConstants.Hand.Left ? rBody.LeftHandTransform : rBody.RightHandTransform; break;
                        case HeldRegion.Arm: transform = side == HumanoidActorConstants.Hand.Left ? rBody.LeftShieldTransform : rBody.RightSheidlTransform; break;
                    }
                }
                break;

                case HumanoidActorConstants.HeldApparelState.Holstered:
                {
                    switch (apparelSlot.HolsterRegion)
                    {
                        case HolsterRegion.BackSide: transform = side == HumanoidActorConstants.Hand.Left ? rBody.BackHolsterLTransform : rBody.BackHolsterRTransform; break;
                        case HolsterRegion.BackCenter: transform = rBody.ShieldHolsterTransform; break;
                    }
                }
                break;
            }

            return transform;
        }
    }
}


