using MAGE.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class AudioTriggerVolume : TriggerVolumeBase<ThirdPersonActorController>
    {
        [SerializeField] string _TrackId;
        public TrackId TrackId { get { return EnumUtil.StringToEnum<TrackId>(_TrackId); } }

        protected override int GetLayer()
        {
            return (int)Layer.Default;
        }

        protected override void HandleTriggerEntered(ThirdPersonActorController partyAvatar)
        {
            TrackId trackId = TrackId;
            Debug.Assert(trackId != TrackId.INVALID);
            if (trackId  != TrackId.INVALID)
            {
                AudioManager.Instance.PlayTrack(trackId);
            }
        }

        protected override void HandleTriggerExited(ThirdPersonActorController partyAvatar)
        {
            TrackId trackId = TrackId;
            Debug.Assert(trackId != TrackId.INVALID);
            if (trackId != TrackId.INVALID)
            {
                AudioManager.Instance.StopTrack(trackId);
            }
        }
    }
}
