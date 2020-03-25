using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using UnityEngine;
using UnityEngine.Events;

class SpecializationOutfiterViewControl 
    : UIContainerControl
    , ICharacterOutfiter
{
    private DB.DBCharacter mOutfitingCharacter = null;
    private UnityAction mOnUpdatedCB;
    //! ICharacterOutfiter
    public void BeginOutfitting(DBCharacter character, UnityAction characterUpdated)
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

    public void HandleComponentInteraction(int containerId, IUIInteractionInfo interactionInfo)
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
                                    UIButtonList.ButtonListInteractionInfo buttonListInteractionInfo = interactionInfo as UIButtonList.ButtonListInteractionInfo;
                                    AssignTalentPoint(buttonListInteractionInfo.ButtonIdx);

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

    public IDataProvider Publish()
    {
        SpecializationOutfiterView.DataProvider dataProvider = new SpecializationOutfiterView.DataProvider();

        DB.SpecializationInfo specializationInfo = mOutfitingCharacter.Specializations.Specializations[(int)mOutfitingCharacter.CharacterInfo.CurrentSpecialization];

        dataProvider.AvailableTalentPts = specializationInfo.TalentPoints;
        dataProvider.SpecializationName = mOutfitingCharacter.CharacterInfo.CurrentSpecialization.ToString();

        SpecializationInfo info = SpecializationFactory.CheckoutSpecializationInfo(mOutfitingCharacter.CharacterInfo.CurrentSpecialization);
        for (int i = 0; i < info.Talents.Count; ++i)
        {
            SpecializationOutfiterView.TalentDP talentDP = new SpecializationOutfiterView.TalentDP();
            TalentId talentId = info.Talents[i];
            int spentPoints = specializationInfo.SpentTalentPoints[i];
            Talent talent = TalentFactory.CheckoutTalent(talentId, spentPoints);
            int maxPoints = talent.MaxPoints;

            talentDP.TalentName = info.Talents[i].ToString();
            talentDP.AssignedPoints = spentPoints;
            talentDP.MaxPoints = maxPoints;
            talentDP.IsSelectable = spentPoints < maxPoints;


            dataProvider.TalentDPs.Add(talentDP);
        }

        return dataProvider;
    }

    private void AssignTalentPoint(int talentIdx)
    {
        DB.SpecializationInfo info = mOutfitingCharacter.Specializations.Specializations[(int)mOutfitingCharacter.CharacterInfo.CurrentSpecialization];
        info.SpentTalentPoints[talentIdx]++;
        info.TalentPoints--;

        DBHelper.WriteCharacter(mOutfitingCharacter);
        mOnUpdatedCB();
    }

    private void ResetTalentPoints()
    {
        DB.SpecializationInfo toReset = mOutfitingCharacter.Specializations.Specializations[(int)mOutfitingCharacter.CharacterInfo.CurrentSpecialization];
        for (int i = 0; i < toReset.SpentTalentPoints.Count; ++i)
        {
            toReset.TalentPoints += toReset.SpentTalentPoints[i];
            toReset.SpentTalentPoints[i] = 0;
        }

        DBHelper.WriteCharacter(mOutfitingCharacter);
        mOnUpdatedCB();
    }
}

