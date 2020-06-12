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
    private List<int> mTalentIds;
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

        DB.Character.DBSpecializationInfo specializationInfo = mOutfitingCharacter.Specializations[mOutfitingCharacter.CharacterInfo.CurrentSpecialization];

        Specialization specialization = SpecializationFactory.CheckoutSpecialization((SpecializationType)mOutfitingCharacter.CharacterInfo.CurrentSpecialization, specializationInfo);

        dataProvider.AvailableTalentPts = specialization.NumUnassignedTalentPoints();
        dataProvider.SpecializationName = specialization.SpecializationType.ToString();

        mTalentIds = new List<int>();
        foreach (var keyValuePair in specialization.Talents)
        {
            SpecializationOutfiterView.TalentDP talentDP = new SpecializationOutfiterView.TalentDP();

            mTalentIds.Add((int)keyValuePair.Key);
            Talent talent = keyValuePair.Value;
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
        DB.Character.DBSpecializationInfo info = mOutfitingCharacter.Specializations[mOutfitingCharacter.CharacterInfo.CurrentSpecialization];
        DB.Character.Talent dbTalent = info.Talents.Find(x => x.TalentId == talentId);
        if (dbTalent != null)
        {
            dbTalent.AssignedPoints++;
        }
        else
        {
            info.Talents.Add(new DB.Character.Talent() { TalentId = talentId, AssignedPoints = 1 });
        }

        DBHelper.WriteCharacter(mOutfitingCharacter);
        mOnUpdatedCB();
    }

    private void ResetTalentPoints()
    {
        DB.Character.DBSpecializationInfo toReset = mOutfitingCharacter.Specializations[mOutfitingCharacter.CharacterInfo.CurrentSpecialization];
        for (int i = 0; i < toReset.Talents.Count; ++i)
        {
            toReset.Talents[i].AssignedPoints = 0;
        }

        DBHelper.WriteCharacter(mOutfitingCharacter);
        mOnUpdatedCB();
    }
}

