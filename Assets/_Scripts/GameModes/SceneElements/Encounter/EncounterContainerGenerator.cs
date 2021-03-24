using MAGE.GameSystems.Mobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Encounters
{
    static class EncounterContainerGenerator
    {
        public static void GenerateEncounterContainer(ThirdPersonActorController partyAvatar, List<ActorSpawner> enemyUnits, Vector3 encounterCenter)
        {
            Level currentLevel = LevelManagementService.Get().GetLoadedLevel();

            Vector3 bottomLeft = encounterCenter;
            Vector3 topRight = encounterCenter;

            List<Transform> allUnits = new List<Transform>();
            allUnits.Add(partyAvatar.transform);
            allUnits.AddRange(enemyUnits.Select(x=>x.transform).ToList());

            foreach (Transform unit in allUnits)
            {
                if (unit.transform.position.x < bottomLeft.x)
                {
                    bottomLeft.x = unit.transform.position.x;
                }
                if (unit.transform.position.z < bottomLeft.z)
                {
                    bottomLeft.z = unit.transform.position.z;
                }
                if (unit.transform.position.x > topRight.x)
                {
                    topRight.x = unit.transform.position.x;
                }
                if (unit.transform.position.z > topRight.z)
                {
                    topRight.z = unit.transform.position.z;
                }
            }

            Vector3 enemyPosition = Vector3.zero;
            foreach (ActorSpawner actorSpawner in enemyUnits)
            {
                enemyPosition += actorSpawner.transform.position;
            }
            enemyPosition /= enemyUnits.Count;

            Vector3 partyPosition = partyAvatar.transform.position;

            Vector3 displacementFromEnemy = partyPosition - enemyPosition;

            PlacementRegion partyPlacementRegion = PlacementRegion.Left;

            // Party is to the left/right of the enemies
            if (Mathf.Abs(displacementFromEnemy.x) > Mathf.Abs(displacementFromEnemy.z))
            {
                if (displacementFromEnemy.x < 0)
                {
                    partyPlacementRegion = PlacementRegion.Left;
                }
                else
                {
                    partyPlacementRegion = PlacementRegion.Right;
                }
            }
            else
            {
                if (displacementFromEnemy.z < 0)
                {
                    partyPlacementRegion = PlacementRegion.Bottom;
                }
                else
                {
                    partyPlacementRegion = PlacementRegion.Top;
                }
            }

            GameObject go = new GameObject("RandomEncounter");
            go.transform.SetParent(currentLevel.EncounterContainer);
            go.transform.position = encounterCenter;
            EncounterContainer_Deprecated container = go.AddComponent<EncounterContainer_Deprecated>();

            container.WinConditions.Add(new GameSystems.World.EncounterConditionParams()
            {
                ConditionType = GameSystems.World.EncounterConditionType.TeamDefeatedCondition,
                Param1 = (int)TeamSide.EnemyAI
            });
            container.LoseConditions.Add(new GameSystems.World.EncounterConditionParams()
            {
                ConditionType = GameSystems.World.EncounterConditionType.TeamDefeatedCondition,
                Param1 = (int)TeamSide.AllyHuman
            });

            container.AlliesContainer = new GameObject("Allies").transform;
            container.AlliesContainer.transform.SetParent(container.transform);
            container.AlliesContainer.transform.localPosition = Vector3.zero;
            container.EnemiesContainer = new GameObject("Enemies").transform;
            container.EnemiesContainer.transform.SetParent(container.transform);
            container.EnemiesContainer.transform.localPosition = Vector3.zero;
            container.EncounterScenarioId = EncounterScenarioId.Random;
            container.PlacementInfo.PlacementRegion = partyPlacementRegion;
            int width = Mathf.Abs(Mathf.CeilToInt(topRight.x - bottomLeft.x));
            if (width < 8)
            {
                int difference = 8 - width;
                bottomLeft.x -= difference / 2;
                topRight.x += difference / 2;

                width = Mathf.Abs(Mathf.CeilToInt(topRight.x - bottomLeft.x));
            }
            int length = Mathf.Abs(Mathf.CeilToInt(topRight.z - bottomLeft.z));
            if (length < 8)
            {
                int difference = 8 - length;
                bottomLeft.z -= difference / 2;
                topRight.z += difference / 2;

                length = Mathf.Abs(Mathf.CeilToInt(topRight.z - bottomLeft.z));
            }
            Vector3 newCenter = bottomLeft + (topRight - bottomLeft) / 2;
            container.Tiles = currentLevel.GenerateTiles(newCenter, width, length, container.transform).transform;

            foreach (ActorSpawner enemy in enemyUnits)
            {
                if (enemy.GetComponent<MobCharacterControl>() != null)
                {
                    MobId mob = enemy.GetComponent<MobCharacterControl>().MobId;
                    container.MobsInEncounter.Add(mob);
                }

                ActorSpawner spawner = ActorLoader.Instance.CreateActorSpawner();

                Vector3 mapPos = RaycastUtil.GetRayCastHit(enemy.transform.position + Vector3.up * 100, Vector3.down, 500, new List<RayCastLayer>() { RayCastLayer.Terrain });
                spawner.transform.SetPositionAndRotation(mapPos, enemy.transform.rotation);
                spawner.transform.SetParent(container.EnemiesContainer);
                spawner.GetComponent<CharacterPickerControl>().CharacterPicker.RootCharacterId = enemy.GetComponent<CharacterPickerControl>().CharacterPicker.GetCharacterId();
            }
        }
    }
}
