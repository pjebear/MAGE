using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MAGE.GameModes.SceneElements
{
    class CinematicDialogueClip : PlayableAsset, ITimelineClipAsset
    {
        public Animator ActorSpawner;
        public CinematicDialogueBehaviour template = new CinematicDialogueBehaviour();

        public ClipCaps clipCaps { get { return ClipCaps.None; } }
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<CinematicDialogueBehaviour>.Create(graph, template);
        }
    }
}
