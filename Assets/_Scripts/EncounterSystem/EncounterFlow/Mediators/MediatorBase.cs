using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EncounterSystem
{
    using Screen;
    using Map;
    namespace EncounterFlow
    {
        namespace Mediator
        {
            abstract class MediatorBase : MonoBehaviour
            {
                protected PlayerInputViewController rViewController = null; // Get a action specific one? 
                protected CameraManager rCameraManager = null;
                protected MapManager rMapManager = null;
                protected EncounterFlowManager rFlowManager = null;

                public void Initialize(PlayerInputViewController viewController, CameraManager cameraManager, MapManager mapManager, EncounterFlowManager flowManager)
                {
                    rViewController = viewController;
                    rCameraManager = cameraManager;
                    rMapManager = mapManager;
                    rFlowManager = flowManager;
                }
            }
        }
    }
}

