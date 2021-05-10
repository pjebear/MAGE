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
    INVALID = -1,

    City,
    Encounter,
    Forest,
    Explore
}

class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private bool mMuteMusic = false;

    private AssetLoader<AudioClip> mAudioLoader = null;

    private float mVolume = .25f;

    private AudioSource rAudioSource = null;
    private List<TrackId> mTrackStack = new List<TrackId>();
    private TrackId CurrentTrack { get { return (mTrackStack.Count > 0 ? mTrackStack[mTrackStack.Count - 1] : TrackId.INVALID); } }
    private Coroutine mFadeCoroutine = null;

    private void Awake()
    {
        if (Instance == null)
        {
            mAudioLoader = new AssetLoader<AudioClip>("Audio");
            mAudioLoader.LoadAssets("SFX");
            mAudioLoader.LoadAssets("Tracks");

            DebugSettings settings = FindObjectOfType<DebugSettings>();
            mMuteMusic = settings.MuteMusic;

            rAudioSource = gameObject.AddComponent<AudioSource>();
            rAudioSource.loop = true;
            rAudioSource.spatialBlend = 0; // global volume

            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    private void Update()
    {
        if (rAudioSource.isPlaying && mFadeCoroutine == null)
        {
            float remainingSeconds = rAudioSource.clip.length - rAudioSource.time;
            if (remainingSeconds < 5 && remainingSeconds != 0)
            {
                rAudioSource.volume = remainingSeconds / 5;
            }
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

    public void PlayTrack(TrackId trackId)
    {
        Debug.LogFormat("AudioManager() - PlayTrack [{0}] CurrentTrack [{1}]", trackId.ToString(), CurrentTrack.ToString());

        if (mMuteMusic) return;

        StopAllCoroutines();

        if (CurrentTrack != TrackId.INVALID)
        {
            StartCoroutine(FadeOut(2, () =>
            {
                rAudioSource.Stop();

                rAudioSource.clip = GetTrack(trackId);
                rAudioSource.Play();
                StartCoroutine(FadeIn(2, null));
            }));
        }
        else
        {
            rAudioSource.clip = GetTrack(trackId);
            rAudioSource.Play();
            StartCoroutine(FadeIn(2, null));
        }

        mTrackStack.Add(trackId);
    }

    public void StopTrack(TrackId trackId)
    {
        Debug.Assert(trackId == CurrentTrack);
        if (CurrentTrack == trackId)
        {
            StopTrack();
        }
    }

    public void StopTrack()
    {
        StopAllCoroutines();

        if (CurrentTrack != TrackId.INVALID)
        {
            mTrackStack.Remove(CurrentTrack);
            StartCoroutine(FadeOut(2, () =>
            {
                rAudioSource.Stop();

                if (CurrentTrack != TrackId.INVALID)
                {
                    rAudioSource.clip = GetTrack(CurrentTrack);
                    rAudioSource.Play();
                    StartCoroutine(FadeIn(2, null));
                }
            }));
        }
    }

    public void StopAllTracks()
    {
        StopTrack();

        mTrackStack.Clear();
    }

    private IEnumerator FadeOut(float duration, Action onFadeComplete)
    {
        float fadeIncrement = 1 / duration;

        while (rAudioSource.volume > 0)
        {
            rAudioSource.volume -= fadeIncrement * Time.deltaTime;
            if (rAudioSource.volume <= 0)
            {
                rAudioSource.volume = 0;
            }
            yield return null;
        }

        if (onFadeComplete != null)
        {
            Action cb = onFadeComplete;
            onFadeComplete = null;
            cb();
        }
    }

    private IEnumerator FadeIn(float duration, Action onFadeComplete)
    {
        float fadeIncrement = 1 / duration;

        while (rAudioSource.volume < mVolume)
        {
            rAudioSource.volume += fadeIncrement * Time.deltaTime;
            if (rAudioSource.volume >= mVolume)
            {
                rAudioSource.volume = mVolume;
            }
            yield return null;
        }

        if (onFadeComplete != null)
        {
            Action cb = onFadeComplete;
            onFadeComplete = null;
            cb();
        }
    }
}

