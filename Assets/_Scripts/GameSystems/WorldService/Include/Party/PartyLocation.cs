using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.World
{
    enum PartyPositionType
    {
        SpawnPoint,
        Position,

        NUM
    }

    struct PartyLocation
    {
        public LevelId Level;

        public PartyPositionType PositionType;
        public int SpawnPoint;
        public Vector3 Position;

        public void SetLevel(LevelId levelId)
        {
            Level = levelId;
        }

        public void SetPosition(Vector3 position)
        {
            PositionType = PartyPositionType.Position;
            Position = position;
        }

        public void SetPosition(int spawnPoint)
        {
            PositionType = PartyPositionType.SpawnPoint;
            SpawnPoint = spawnPoint;
        }
    }
}
