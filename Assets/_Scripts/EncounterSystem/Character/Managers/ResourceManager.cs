using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.AttributeTypes;
using Common.AttributeEnums;
using UnityEngine;
using Common.ActionTypes;

namespace EncounterSystem
{
    namespace Character
    {
        namespace Managers
        {
            class ResourceManager
            {
                private float[] rBaseResources;
                private float[] rStats;
                public float[] EncounterResources { get; private set; }

                public float ClockGuague { get; private set; }
                public bool IsAlive { get { return EncounterResources[(int)Resource.Health] >= 1; } }

                public ResourceManager()
                {
                    ClockGuague = 0f;               
                }

                public void Initialize(AttributeContainer attributes)
                {
                    rStats = attributes[AttributeType.Stat];
                    rBaseResources = attributes[AttributeType.Resource];
                    EncounterResources = new float[rBaseResources.Length];
                    rBaseResources.CopyTo(EncounterResources, 0);
                }

                public void ModifyCurrentResource(ResourceChange change)
                {
                    int resourceIndex = (int)change.Resource;
                    float amount = change.Value;
                    EncounterResources[resourceIndex] += amount;
                    // Update base character 'current' resources to match the encounter resource change

                    if (EncounterResources[resourceIndex] >= EncounterResources[resourceIndex + 3])
                    {
                        // Set current and base resources to be max
                        EncounterResources[resourceIndex] = EncounterResources[resourceIndex + 3];
                        rBaseResources[resourceIndex] = rBaseResources[resourceIndex + 3];
                    } 
                    else if (EncounterResources[resourceIndex] <= 0)
                    {
                        EncounterResources[resourceIndex] = 0;
                        rBaseResources[resourceIndex] = 0;
                    }
                    else
                    {
                        float currentToBaseScalar = rBaseResources[resourceIndex + 3] / EncounterResources[resourceIndex + 3] ;
                        rBaseResources[resourceIndex] += amount * currentToBaseScalar;
                    }
                }

                public void ProgressCharacterClock(float modifier = 1)
                {
                    ClockGuague = Mathf.Min(ClockGuague + rStats[(int)TertiaryStat.Speed] * modifier, 100);
                }

                public void DecrementClockGuage(float reduction)
                {
                    ClockGuague -= reduction;
                }

                public void OnTurnStart()
                {
                    EncounterResources[(int)Resource.Endurance] +=
                        rStats[(int)TertiaryStat.EnduranceRecovery] * EncounterResources[(int)Resource.MaxEndurance];
                    EncounterResources[(int)Resource.Endurance] =
                        Mathf.Min(EncounterResources[(int)Resource.Endurance], EncounterResources[(int)Resource.MaxEndurance]);
                }
            }
        }
    }
}