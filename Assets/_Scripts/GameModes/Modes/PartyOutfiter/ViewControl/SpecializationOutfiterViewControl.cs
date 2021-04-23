using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MAGE.GameModes.FlowControl
{

    class SpecializationOutfiterViewControl
        : UIContainerControl
        , ICharacterOutfiter
    {
        private int mCharacterId = -1;
        private List<int> mTalentIds;
        private List<SpecializationType> mSpecializationIds;
        //! ICharacterOutfiter
        public void BeginOutfitting()
        {
            UIManager.Instance.PostContainer(UIContainerId.SpecializationOutfiterView, this);
        }

        public void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.SpecializationOutfiterView);
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch (containerId)
            {
                case (int)UIContainerId.SpecializationOutfiterView:
                {
                    if (interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        switch (interactionInfo.ComponentId)
                        {
                            case (int)SpecializationOutfiterView.ComponentId.ResetTalentsBtn:
                            {
                                ResetTalentPoints();
                                UIManager.Instance.Publish(UIContainerId.SpecializationOutfiterView);
                            }
                            break;

                            case (int)SpecializationOutfiterView.ComponentId.TalentBtns:
                            {
                                ListInteractionInfo buttonListInteractionInfo = interactionInfo as ListInteractionInfo;
                                AssignTalentPoint(mTalentIds[buttonListInteractionInfo.ListIdx]);

                                UIManager.Instance.Publish(UIContainerId.SpecializationOutfiterView);
                            }
                            break;

                            case (int)SpecializationOutfiterView.ComponentId.SpecializationBtns:
                            {
                                ListInteractionInfo buttonListInteractionInfo = interactionInfo as ListInteractionInfo;

                                SpecializationType toChangeTo = mSpecializationIds[buttonListInteractionInfo.ListIdx];
                                MAGE.GameSystems.WorldService.Get().ChangeSpecialization(mCharacterId, toChangeTo);

                                UIManager.Instance.Publish(UIContainerId.SpecializationOutfiterView);
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
            return "SpecializationOutfiterViewControl";
        }

        public IDataProvider Publish(int containerId)
        {
            SpecializationOutfiterView.DataProvider dataProvider = new SpecializationOutfiterView.DataProvider();

            Character outfitingCharacter = CharacterService.Get().GetCharacter(mCharacterId);

            mSpecializationIds = new List<SpecializationType>();
            dataProvider.SpecializationDPS = new List<UIButton.DataProvider>();
            foreach (Specialization specialization in outfitingCharacter.Specializations.Values)
            {
                mSpecializationIds.Add(specialization.SpecializationType);

                UIButton.DataProvider buttonDp = new UIButton.DataProvider(
                    specialization.SpecializationType.ToString(),
                    specialization.SpecializationType != outfitingCharacter.CurrentSpecialization.SpecializationType);

                dataProvider.SpecializationDPS.Add(buttonDp);
            }
           
            MAGE.GameSystems.Characters.Specialization currentSpecialization = outfitingCharacter.CurrentSpecialization;

            dataProvider.AvailableTalentPts = currentSpecialization.NumUnassignedTalentPoints();
            dataProvider.SpecializationName = currentSpecialization.SpecializationType.ToString();

            mTalentIds = new List<int>();
            foreach (var keyValuePair in currentSpecialization.Talents)
            {
                SpecializationOutfiterView.TalentDP talentDP = new SpecializationOutfiterView.TalentDP();

                mTalentIds.Add((int)keyValuePair.Key);
                MAGE.GameSystems.Characters.Talent talent = keyValuePair.Value;
                int maxPoints = talent.MaxPoints;

                talentDP.TalentName = talent.TalentId.ToString();
                talentDP.AssignedPoints = talent.PointsAssigned;
                talentDP.MaxPoints = talent.MaxPoints;
                talentDP.IsSelectable = talent.PointsAssigned < talent.MaxPoints && currentSpecialization.NumUnassignedTalentPoints() > 0;


                dataProvider.TalentDPs.Add(talentDP);
            }

            return dataProvider;
        }

        public void Refresh()
        {
            UIManager.Instance.Publish(UIContainerId.SpecializationOutfiterView);
        }

        public void SetCharacter(int characterId)
        {
            mCharacterId = characterId;
            Refresh();
        }

        private void AssignTalentPoint(int talentId)
        {
            MAGE.GameSystems.WorldService.Get().AssignTalentPoint(mCharacterId, (MAGE.GameSystems.Characters.TalentId)talentId);
        }

        private void ResetTalentPoints()
        {
            MAGE.GameSystems.WorldService.Get().ResetTalentPoints(mCharacterId);
        }
    }
}


