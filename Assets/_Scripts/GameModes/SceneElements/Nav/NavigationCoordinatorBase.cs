using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Navigation
{
    abstract class NavigationCoordinatorBase : MonoBehaviour
    {
        public abstract Vector3 GetNextNavigationPoint(Vector3 currentPosition);
    }
}
