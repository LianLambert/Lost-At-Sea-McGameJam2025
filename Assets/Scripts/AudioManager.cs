using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    private AudioSource audioSource;
    private AudioSource musicSource;

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();

    private HashSet<string> playingSounds = new HashSet<string>();

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        LoadAllAudioClips();
        LoadAllMusicClips();

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            PlayMusic("Menu Theme - Guiding Light");
            Debug.Log("menu music!");


        }

        if (SceneManager.GetActiveScene().name == "SampleScene 1")
        {
            PlayMusic("Level Theme - Lost at Sea");
            Debug.Log("level music!");
        }
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
            if (playingSounds.Contains(clipName)) return;

            audioSource.PlayOneShot(clip);
            playingSounds.Add(clipName);

            // Remove clip from list of playing clips
            StartCoroutine(RemoveFromPlayingSounds(clipName, clip.length));

        }

    }

    public void PlaySound(string clipName, float volume)
    {
        if (audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            if (playingSounds.Contains(clipName)) return;

            audioSource.PlayOneShot(clip, volume);
            playingSounds.Add(clipName);

            // Remove clip from list of playing clips
            StartCoroutine(RemoveFromPlayingSounds(clipName, clip.length));


        }
    }

    private IEnumerator RemoveFromPlayingSounds(string clipName, float delay)
    {
        yield return new WaitForSeconds(delay);
        playingSounds.Remove(clipName);
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
