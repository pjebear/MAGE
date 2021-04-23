using MAGE.GameModes.DebugFlow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

enum SFXId
{
    INVALID = -1,

    ArrowDraw,
    ArrowRelease,
    Cancel,
    Cast,
    Confirm,
    Dodge,
    Heal,
    MaleDeath,
    MenuHover,
    Parry,
    ShieldBlock,
    Slash,
    WeaponSwing
}

enum TrackId
{
    City,
    Encounter,
    Explore
}

class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private bool mMuteMusic = false;

    private AssetLoader<AudioClip> mAudioLoader = null;

    private void Awake()
    {
        if (Instance == null)
        {
            mAudioLoader = new AssetLoader<AudioClip>("Audio");
            mAudioLoader.LoadAssets("SFX");
            mAudioLoader.LoadAssets("Tracks");

            DebugSettings settings = FindObjectOfType<DebugSettings>();
            mMuteMusic = settings.MuteMusic;

            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public AudioClip GetSFXClip(SFXId sFXId)
    {
        return mAudioLoader.GetAsset(sFXId.ToString());
    }

    public AudioClip GetTrack(TrackId trackId)
    {
        return mAudioLoader.GetAsset(trackId.ToString());
    }

    public void FadeInTrack(AudioSource source, float fadeInDuration, float maxVolume = 1.0f)
    {
        if (mMuteMusic) return;

        StartCoroutine(FadeAudio(source, fadeInDuration, 0, maxVolume));
    }

    public void FadeOutTrack(AudioSource source, float fadeOutDuration)
    {
        StartCoroutine(FadeAudio(source, fadeOutDuration, source.volume, 0));
    }

    private IEnumerator FadeAudio(AudioSource source, float duration, float startVolume, float endVolume)
    {
        source.volume = startVolume;
        source.Play();

        float fadeIncrement = (endVolume - startVolume) / duration;

        while (source != null && source.volume < 1)
        {
            source.volume += fadeIncrement * Time.deltaTime;
            if (source.volume > endVolume)
            {
                source.volume = endVolume;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}

