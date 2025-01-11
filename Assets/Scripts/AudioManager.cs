using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private AudioSource audioSource;
    private AudioSource musicSource;

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        LoadAllAudioClips();
        LoadAllMusicClips();
    }

    private void LoadAllMusicClips()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Music");
        foreach (AudioClip clip in clips)
        {
            if (!musicClips.ContainsKey(clip.name))
            {
                musicClips.Add(clip.name, clip);
            }
        }
    }

    private void LoadAllAudioClips()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("SFX");
        foreach (AudioClip clip in clips)
        {
            if (!audioClips.ContainsKey(clip.name))
            {
                audioClips.Add(clip.name, clip);
            }
        }
    }

    public void PlaySound(string clipName)
    {
        if (audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }

    }

    public void PlaySound(string clipName, float volume)
    {
        if (audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayMusic(string clipName)
    {
        if (musicClips.TryGetValue(clipName, out AudioClip clip))
        {
            if (musicSource.clip == clip) return;

            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}
