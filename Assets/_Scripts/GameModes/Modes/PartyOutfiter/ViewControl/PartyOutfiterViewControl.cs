using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.FlowControl
{
    class PartyOutfiterViewControl : UIContainerControl
    {
        private Transform mCharacterSpawnPoint;

        private ICharacterOutfiter mSpecOutfiter = new SpecializationOutfiterViewControl();
        private ICharacterOutfiter mEquipmentOutfiter = new EquipmentOutfiterViewControl();

        private ICharacterOutfiter mOutfiter;
        public int mOutfiterIdx = 0;

        private List<int> mCharacterIds = new List<int>();
        private int mCharacterIdx = 0;
        private MAGE.GameSystems.Characters.Character mOutfitingCharacter = null;

        public void Init(Transform characterSpawnPoint)
        {
            mCharacterSpawnPoint = characterSpawnPoint;
        }

        public void Start()
        {
            mCharacterIds = MAGE.GameSystems.WorldService.Get().GetCharactersInParty();
            mCharacterIdx = 0;

            mOutfitingCharacter = MAGE.GameSystems.CharacterService.Get().GetCharacter(mCharacterIds[mCharacterIdx]);
            SpawnCharacter();

            SetOutfiter(mEquipmentOutfiter);

            UIManager.Instance.PostContainer(UIContainerId.OutfiterSelectView, this);
        }

        public void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.OutfiterSelectView);
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch (containerId)
            {
                case (int)UIContainerId.OutfiterSelectView:
                {
                    if (interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        switch (interactionInfo.ComponentId)
                        {
                            case (int)OutfiterSelectView.ComponentId.CharacterSelectLeftBtn:
                            {
                                CycleCharacter(-1);
                            }
                            break;

                            case (int)OutfiterSelectView.ComponentId.CharacterSelectRightBtn:
                            {
                                CycleCharacter(1);
                            }
                            break;
                            case (int)OutfiterSelectView.ComponentId.ExitBtn:
                            {
                                //GameModesModule.Instance.Explore();
                            }
                            break;
                            case (int)OutfiterSelectView.ComponentId.EquipBtn:
                            {
                                if (mOutfiter != mEquipmentOutfiter)
                                {
                                    SetOutfiter(mEquipmentOutfiter);
                                }
                            }
                            break;

                            case (int)OutfiterSelectView.ComponentId.SpecBtn:
                            {
                                if (mOutfiter != mSpecOutfiter)
                                {
                                    SetOutfiter(mSpecOutfiter);
                                }
                            }
                            break;
                        }
                    }
                }
                break;
            }
        }

        public string Name()
        {
            return "PartyOutfiterViewControl";
        }

        public IDataProvider Publish(int containerId)
        {
            OutfiterSelectView.DataProvider dataProvider = new OutfiterSelectView.DataProvider();

            dataProvider.character = mOutfitingCharacter.Name;

            return dataProvider;
        }

        private void SetOutfiter(ICharacterOutfiter outfiter)
        {
            if (mOutfiter != null)
            {
                mOutfiter.Cleanup();
            }

            mOutfiter = outfiter;

            mOutfiter.BeginOutfitting(mOutfitingCharacter, () => { SpawnCharacter(); });
        }

        private void CycleCharacter(int direction)
        {
            int newIdx = mCharacterIdx + direction;
            if (newIdx < 0) newIdx = mCharacterIds.Count - 1;
            if (newIdx == mCharacterIds.Count) newIdx = 0;

            if (newIdx != mCharacterIdx)
            {
                mCharacterIdx = newIdx;
                mOutfitingCharacter = MAGE.GameSystems.CharacterService.Get().GetCharacter(mCharacterIds[mCharacterIdx]);

                mOutfiter.Cleanup();

                mOutfiter.BeginOutfitting(mOutfitingCharacter, () => { SpawnCharacter(); });

                SpawnCharacter();
            }

            UIManager.Instance.Publish(UIContainerId.OutfiterSelectView);
        }

        private void SpawnCharacter()
        {
            if (mCharacterSpawnPoint.childCount > 0)
            {
                GameObject.Destroy(mCharacterSpawnPoint.GetChild(0).gameObject);
            }

            //ActorLoader.Instance.LoadActor(mOutfitingCharacter.GetAppearance(), mCharacterSpawnPoint);
        }
    }
}
