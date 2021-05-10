using MAGE.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    [RequireComponent(typeof(TrackPickerControl))]
    class AudioTriggerVolume : TriggerVolumeBase<ThirdPersonActorController>
    {
        protected override int GetLayer()
        {
            return (int)Layer.Default;
        }

        protected override void HandleTriggerEntered(ThirdPersonActorController partyAvatar)
        {
            TrackId trackId = EnumUtil.StringToEnum<TrackId>(GetComponent<TrackPickerControl>().TrackIdPicker.PickedOption);
            Debug.Assert(trackId != TrackId.INVALID);
            if (trackId  != TrackId.INVALID)
            {
                AudioManager.Instance.PlayTrack(trackId);
            }
        }

        protected override void HandleTriggerExited(ThirdPersonActorController partyAvatar)
        {
            TrackId trackId = EnumUtil.StringToEnum<TrackId>(GetComponent<TrackPickerControl>().TrackIdPicker.PickedOption);
            Debug.Assert(trackId != TrackId.INVALID);
            if (trackId != TrackId.INVALID)
            {
                AudioManager.Instance.StopTrack(trackId);
            }
        }
    }
}
