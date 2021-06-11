using MAGE.GameModes.Combat;
using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounters;
using MAGE.GameModes.SceneElements.Navigation;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Loot;
using MAGE.GameSystems.Stats;
using MAGE.GameSystems.World;
using MAGE.UI;
using MAGE.UI.Views;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace MAGE.GameModes.Encounter
{
    class EncounterResultsFlowControl 
        : FlowControlBase
        , UIContainerControl
    {
        private string TAG = "EncounterResultsFlowControl";

        private EncounterEndInfo mEncounterEndInfo;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Encounter;
        }

        protected override void Setup()
        {
            Level level = LevelManagementService.Get().GetLoadedLevel();
            EncounterContainer activeEncounter = level.GetActiveEncounter();

            List<int> party = WorldService.Get().GetCharactersInParty();
            EncounterEndParams result = new EncounterEndParams();
            result.EncounterScenarioId = activeEncounter.EncounterScenarioId;
            result.PlayersInEncounter = GameModel.Encounter.Teams[TeamSide.AllyHuman]
                .Where(x => x.GetComponent<ControllableEntity>() != null)
                .Select(x => x.GetComponent<ControllableEntity>().Character.Id)
                .Where(x=> party.Contains(x))
                .ToList();

            result.DidUserWin = !GameModel.Encounter.IsEncounterLost();
            result.LootParams.LevelId = level.LevelId;
            result.LootParams.Mobs = activeEncounter.MobsInEncounter;
            result.LootParams.Items = activeEncounter.ItemRewards;
            result.LootParams.Coins = activeEncounter.CoinReward;

            mEncounterEndInfo = WorldService.Get().UpdateOnEncounterEnd(result);

            UIManager.Instance.PostContainer(UIContainerId.EncounterResultsView, this);
        }

        protected override void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterResultsView);
        }

        public override bool Notify(string notifyEvent)
        {
            bool handled = false;

            switch (notifyEvent)
            {

            }

            return handled;
        }

        public override string Query(string queryParam)
        {
            string result = "";

            switch (queryParam)
            {

            }

            return result;
        }

        public override string Condition(string condition)
        {
            string result = FlowConstants.CONDITION_EMPTY;

            switch (condition)
            {

            }

            return result;
        }

        public IDataProvider Publish(int containerId)
        {
            EncounterResultsView.DataProvider dataProvider = new EncounterResultsView.DataProvider();

            dataProvider.DidUserWin = mEncounterEndInfo.Won;

            if (mEncounterEndInfo.Won)
            {
                foreach (var growthInfoPair in mEncounterEndInfo.CharacterGrowth)
                {
                    CharacterGrowthInfo growth = growthInfoPair.Value;

                    Character character = GameModel.Encounter.Players[growthInfoPair.Key].GetComponent<ControllableEntity>().Character;
                    Character postCombatCharacter = CharacterService.Get().GetCharacter(growthInfoPair.Key);

                    CharacterGrowth.DataProvider growthDP = new CharacterGrowth.DataProvider();
                    growthDP.Name = character.Name;
                    growthDP.PortraitAsset = character.Appearance.PortraitSpriteId.ToString();

                    growthDP.LevelGrowth.Level = character.Level;
                    growthDP.LevelGrowth.Current = character.Experience;
                    growthDP.LevelGrowth.Max = CharacterConstants.LEVEL_UP_THRESHOLD;
                    growthDP.LevelGrowth.Growth = growth.Xp;

                    growthDP.Specialization = character.CurrentSpecializationType.ToString();
                    growthDP.SpecializationGrowth.Level = character.CurrentSpecialization.Level;
                    growthDP.SpecializationGrowth.Current = character.CurrentSpecialization.Experience;
                    growthDP.SpecializationGrowth.Max = SpecializationConstants.LEVEL_UP_THRESHOLD;
                    growthDP.SpecializationGrowth.Growth = growth.SpecializationXp;

                    growthDP.MightGrowth.Current = (int)character.CurrentAttributes[PrimaryStat.Might];
                    growthDP.MightGrowth.Growth = (int)postCombatCharacter.CurrentAttributes[PrimaryStat.Might] - (int)character.CurrentAttributes[PrimaryStat.Might];

                    growthDP.FineseGrowth.Current = (int)character.CurrentAttributes[PrimaryStat.Finese];
                    growthDP.FineseGrowth.Growth = (int)postCombatCharacter.CurrentAttributes[PrimaryStat.Finese] - (int)character.CurrentAttributes[PrimaryStat.Finese];

                    growthDP.MagicGrowth.Current = (int)character.CurrentAttributes[PrimaryStat.Magic];
                    growthDP.MagicGrowth.Growth = (int)postCombatCharacter.CurrentAttributes[PrimaryStat.Magic] - (int)character.CurrentAttributes[PrimaryStat.Magic];

                    growthDP.FortitudeGrowth.Current = (int)character.CurrentAttributes[SecondaryStat.Fortitude];
                    growthDP.FortitudeGrowth.Growth = (int)postCombatCharacter.CurrentAttributes[SecondaryStat.Fortitude] - (int)character.CurrentAttributes[SecondaryStat.Fortitude];

                    growthDP.AttunementGrowth.Current = (int)character.CurrentAttributes[SecondaryStat.Attunement];
                    growthDP.AttunementGrowth.Growth = (int)postCombatCharacter.CurrentAttributes[SecondaryStat.Attunement] - (int)character.CurrentAttributes[SecondaryStat.Attunement];

                    dataProvider.CharacterGrowths.Add(growthDP);
                }

                dataProvider.CurrentCoins = WorldService.Get().GetCurrency() - mEncounterEndInfo.Rewards.Currency;
                dataProvider.CoinRewards = mEncounterEndInfo.Rewards.Currency;

                foreach (var itemRewardPair in mEncounterEndInfo.Rewards.Items)
                {
                    Item itemReward = ItemFactory.LoadItem(itemRewardPair.Key);
                    IconTextBase.DataProvider itemRewardDp = new IconTextBase.DataProvider();
                    itemRewardDp.IconAssetName = itemReward.SpriteId.ToString();
                    itemRewardDp.Text = string.Format("{0} x {1}",itemReward.ItemTag.ItemId.ToString(), itemRewardPair.Value);

                    dataProvider.ItemRewards.Add(itemRewardDp);
                }
                
            }

            return dataProvider;
        }

        public string Name()
        {
            return "EncounterFlowControl";
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            if (interactionInfo.ComponentId == (int)EncounterResultsView.ComponentId.ContinueBtn
                && interactionInfo.InteractionType == UIInteractionType.Click)
            {
                SendFlowMessage("advance");
            }
        }
    }
}


