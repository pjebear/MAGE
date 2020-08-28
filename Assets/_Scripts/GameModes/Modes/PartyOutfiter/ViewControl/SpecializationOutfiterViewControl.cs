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
        private MAGE.GameSystems.Characters.Character mOutfitingCharacter = null;
        private UnityAction mOnUpdatedCB;
        private List<int> mTalentIds;
        //! ICharacterOutfiter
        public void BeginOutfitting(MAGE.GameSystems.Characters.Character character, UnityAction characterUpdated)
        {
            mOutfitingCharacter = character;
            mOnUpdatedCB = characterUpdated;

            UIManager.Instance.PostContainer(UIContainerId.SpecializationOutfiterView, this);
        }

        public void Cleanup()
        {
            mOutfitingCharacter = null;
            mOnUpdatedCB = null;

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

            MAGE.GameSystems.Characters.Specialization specialization = mOutfitingCharacter.CurrentSpecialization;

            dataProvider.AvailableTalentPts = specialization.NumUnassignedTalentPoints();
            dataProvider.SpecializationName = specialization.SpecializationType.ToString();

            mTalentIds = new List<int>();
            foreach (var keyValuePair in specialization.Talents)
            {
                SpecializationOutfiterView.TalentDP talentDP = new SpecializationOutfiterView.TalentDP();

                mTalentIds.Add((int)keyValuePair.Key);
                MAGE.GameSystems.Characters.Talent talent = keyValuePair.Value;
                int maxPoints = talent.MaxPoints;

                talentDP.TalentName = talent.TalentId.ToString();
                talentDP.AssignedPoints = talent.PointsAssigned;
                talentDP.MaxPoints = talent.MaxPoints;
                talentDP.IsSelectable = talent.PointsAssigned < talent.MaxPoints && specialization.NumUnassignedTalentPoints() > 0;


                dataProvider.TalentDPs.Add(talentDP);
            }

            return dataProvider;
        }

        private void AssignTalentPoint(int talentId)
        {
            MAGE.GameSystems.WorldService.Get().AssignTalentPoint(mOutfitingCharacter.Id, (MAGE.GameSystems.Characters.TalentId)talentId);

            mOnUpdatedCB();
        }

        private void ResetTalentPoints()
        {
            MAGE.GameSystems.WorldService.Get().ResetTalentPoints(mOutfitingCharacter.Id);

            mOnUpdatedCB();
        }
    }
}


