using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound{
    public string       name;
    public AudioClip    clip;

    [Range(0f, 1.0f)]
    public float        baseVolume;
    [Range(0f, 1.0f)]
    public float        basePitch;

    public bool         overlapAllowed = true;
    public bool         loop = false;

    private AudioSource source;

    public void SetAudioSource(AudioSource audioSource)
    {
        source = audioSource;
        source.loop = loop;
        source.clip = clip;
    }

    public void PlayWithDefaultSettings()
    {
        if (CheckForOverlap()) return;

        source.volume = baseVolume;
        source.pitch  = basePitch;
        source.Play();
    }

    public void PlayWithVolumeVariance(float volumeVariant)
    {
        if (CheckForOverlap()) return;

        source.volume = volumeVariant;
        source.Play();
    }

    public void PlayWithPitchVariance(float pitchVariant)
    {
        if (CheckForOverlap()) return;

        source.pitch = pitchVariant;
        source.Play();
    }

    public void PlayFullCustom(float volumeVariant, float pitchVariant)
    {
        if (CheckForOverlap()) return;

        source.volume = volumeVariant;
        source.pitch = pitchVariant;
        source.Play();
    }

    private bool CheckForOverlap()
    {
        return (source.isPlaying && overlapAllowed == false);
    }
}

public class SoundManager : MonoBehaviour {

    [SerializeField]
    Sound[] allSounds;

    private     static SoundManager _instance = null;
    public      static SoundManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            DestroyObject(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        foreach(Sound sound in allSounds)
        {
            GameObject newGameObject = new GameObject("Sound_" + sound.name);
            sound.SetAudioSource(newGameObject.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string soundName)
    {
        foreach(Sound sound in allSounds)
        {
            if(sound.name == soundName)
            {
                sound.PlayWithDefaultSettings();
                return;
            }
        }
    }

    public void PlaySound(string soundName, float volumeVariant, float pitchVariant)
    {
        foreach (Sound sound in allSounds)
        {
            if (sound.name == soundName)
            {
                sound.PlayFullCustom(volumeVariant, pitchVariant);
                return;
            }
        }
    }

    public void PlaySoundWithAlternateVolume(string soundName, float volumeVariant)
    {
        foreach (Sound sound in allSounds)
        {
            if (sound.name == soundName)
            {
                sound.PlayWithVolumeVariance(volumeVariant);
                return;
            }
        }
    }

    public void PlaySoundWithAlternatePitch(string soundName, float pitchVariant)
    {
        foreach (Sound sound in allSounds)
        {
            if (sound.name == soundName)
            {
                sound.PlayWithPitchVariance(pitchVariant);
                return;
            }
        }
    }
}
