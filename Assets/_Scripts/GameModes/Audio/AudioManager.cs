using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

enum SFXId
{

}

enum TrackId
{
    City,
    Encounter,
    Explore
}

class AudioManager : MonoBehaviour
{
    private AssetLoader<AudioClip> mAudioLoader = null;

    private void Awake()
    {
        mAudioLoader = new AssetLoader<AudioClip>("Audio");
        mAudioLoader.LoadAssets("SFX");
        mAudioLoader.LoadAssets("Tracks");
    }

    public AudioClip GetSFXClip(SFXId sFXId)
    {
        return mAudioLoader.GetAsset(sFXId.ToString());
    }

    public AudioClip GetTrack(TrackId trackId)
    {
        return mAudioLoader.GetAsset(trackId.ToString());
    }
}

