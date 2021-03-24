using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Navigation
{
    class PathNodeTriggerVolume : TriggerVolumeBase<PathNavigator>
    {
        private HashSet<PathNavigator> mCollidingWith = new HashSet<PathNavigator>();

        protected override int GetLayer()
        {
            return (int)Layer.Navigation;
        }

        protected override void HandleTriggerEntered(PathNavigator entered)
        {
            if (mCollidingWith.Add(entered))
            {
                entered.NotifyArrivedAtNode(this);
            }
            
        }

        protected override void HandleTriggerExited(PathNavigator exited)
        {
            mCollidingWith.Remove(exited);
        }
    }
}
