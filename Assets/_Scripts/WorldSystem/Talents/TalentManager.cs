using Common.ProfessionEnums;
using Screens.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Character;

namespace WorldSystem
{
    namespace Talents
    {
        
        class TalentTree
        {
            public Dictionary<TalentIndex, TalentBase> Talents;
            private Dictionary<TalentIndex, int> mDormantPoints;
            public int AvailablePoints;
            private int mTotalPoints;
            
            public TalentTree()
            {
                Talents = new Dictionary<TalentIndex, TalentBase>();
                mDormantPoints = new Dictionary<TalentIndex, int>();
                AvailablePoints = mTotalPoints = 0;
            }

            public void AddPointToTalent(TalentIndex talent)
            {
                UnityEngine.Debug.Assert(AvailablePoints > 0);
                UnityEngine.Debug.Assert(Talents.ContainsKey(talent));
                AvailablePoints--;
                Talents[talent].ApplyTalentPoints(1);
            }

            public void RemovePointFromTalent(TalentIndex talent)
            {
                UnityEngine.Debug.Assert(Talents.ContainsKey(talent));
                UnityEngine.Debug.Assert(Talents[talent].CurrentPoints > 0);
                AvailablePoints++;
                Talents[talent].RemoveTalentPoints(1);
            }

            public void ResetTalent(TalentIndex talent)
            {
                UnityEngine.Debug.Assert(Talents.ContainsKey(talent));
                AvailablePoints += Talents[talent].ResetTalentPoints();
            }

            public void ResetAllTalents()
            {
                foreach (var talent in Talents.Keys)
                {
                    ResetTalent(talent);
                }
            }

            public void DeactivateTree()
            {
                foreach (var talentPair in Talents)
                {
                    int pointsInTalent = talentPair.Value.ResetTalentPoints();
                    if (pointsInTalent > 0)
                    {
                        mDormantPoints.Add(talentPair.Key, pointsInTalent);
                    }
                }
            }

            public void ActivateTree()
            {
                foreach (var pointsPair in mDormantPoints)
                {
                    Talents[pointsPair.Key].ApplyTalentPoints(pointsPair.Value);
                }
                mDormantPoints.Clear();
            }

            public Dictionary<TalentIndex, TalentPayload> GetTalentPayloads()
            {
                Dictionary<TalentIndex, TalentPayload> talentPayloads = new Dictionary<TalentIndex, TalentPayload>();
                foreach (var talentpair in Talents)
                {
                    talentPayloads.Add(talentpair.Key, talentpair.Value.GetPayload());
                }
                return talentPayloads;
            }
        }

        class TalentManager
        {
            private Dictionary<ProfessionType, TalentTree> mTalentTrees;

            public TalentManager()
            {
                mTalentTrees = new Dictionary<ProfessionType, TalentTree>();
            } 

            public void Initialize(Dictionary<ProfessionType, Dictionary<TalentIndex, List<TalentIndex>>> talentTrees, CharacterBase character)
            {
                foreach (var professionTreePair in talentTrees)
                {

                    Dictionary<TalentIndex, List<TalentIndex>> treeSkeleton = professionTreePair.Value;
                    TalentTree talentTree = new TalentTree();
                   
                    // Create all talents in the tree
                    foreach (TalentIndex talent in treeSkeleton.Keys)
                    {
                        talentTree.Talents.Add(talent, TalentFactory.TalentFactory.CheckoutTalent(talent, character));
                    }

                    // Add the prerequisites for each ability
                    foreach (var talentPair in talentTree.Talents)
                    {
                        foreach (var prereqIndex in treeSkeleton[talentPair.Key])
                        {
                            UnityEngine.Debug.Assert(talentTree.Talents.ContainsKey(prereqIndex));
                            talentPair.Value.PrerequisiteTalents.Add(talentTree.Talents[prereqIndex]);
                        }
                    }

                    mTalentTrees.Add(professionTreePair.Key, talentTree);
                }
                
            }

            public TalentTreePayload GetTalentTreePayload(ProfessionType professionType)
            {
                return new TalentTreePayload(mTalentTrees[professionType].GetTalentPayloads(), mTalentTrees[professionType].AvailablePoints, professionType);
            }

            public void AddTalentPoint(ProfessionType profession)
            {
                mTalentTrees[profession].AvailablePoints++;
            }

            public void ApplyTalentPoint(ProfessionType profession, TalentIndex index)
            {
                mTalentTrees[profession].AddPointToTalent(index);
            }

            public void ActivateTree(ProfessionType profession)
            {
                mTalentTrees[profession].ActivateTree();
            }

            public void DeactivateTree(ProfessionType profession)
            {
                mTalentTrees[profession].DeactivateTree();
            }

            public void TEMP_UnlockAllTalents(ProfessionType profession)
            {
                mTalentTrees[profession].AvailablePoints += 100;
                foreach (TalentBase talent in mTalentTrees[profession].Talents.Values)
                {
                    //talent.ApplyTalentPoints(talent.MaxPoints);
                }
            }
        }
    }
}
