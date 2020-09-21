using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MAGE.GameModes.SceneElements
{
    [System.Serializable]
    [TrackClipType(typeof(CinematicDialogueClip))]
    [TrackBindingType(typeof(ActorSpawner))]
    class CinematicDialogueTrack : TrackAsset
    {
        public List<ActorSpawner> Dialogue = new List<ActorSpawner>();// new List<KeyValuePair<ActorSpawner, string>>();
    }
}

