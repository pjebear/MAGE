using MAGE.GameModes.Encounter;
using MAGE.GameModes.SceneElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.Projectiles
{
    class TestCase : MonoBehaviour
    {
        private ProjectileDirector ProjectileDirector;

        public TileControl Shooter;
        public TileControl Target;

        public void Setup(ProjectileDirector director)
        {
            ProjectileDirector = director;
        }

        public void Run()
        {
            var param = ProjectileUtil.GenerateSpawnParams(Shooter, Target, ProjectilePathType.Arc, ProjectileId.Arrow);
            ProjectileDirector.SpawnProjectile(param);
        }
    }
}
