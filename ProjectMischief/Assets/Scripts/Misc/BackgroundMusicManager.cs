using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour 
{
    AudioSource backgroundMusic;
    public AudioClip main;

    void Awake()
    {
        backgroundMusic = GetComponent<AudioSource>();
        backgroundMusic.ignoreListenerPause = true;
        backgroundMusic.ignoreListenerVolume = true;

        backgroundMusic.clip = main;
        backgroundMusic.Play();
    }

    void Update()
    {
        if(!backgroundMusic.isPlaying)
        {
            backgroundMusic.clip = main;
            backgroundMusic.Play();
        }
    }

    public void setVolume(float v)
    {
        backgroundMusic.volume = v;
    }

    public void Mute()
    {
        backgroundMusic.mute = true;
    }

    public void Pause()
    {
        backgroundMusic.Pause();
    }

    public void Play()
    {
        backgroundMusic.Play();
    }

    public bool isPlaying()
    {
        return backgroundMusic.isPlaying;
    }

    public void ChangeSong(AudioClip song)
    {
        backgroundMusic.clip = song;
        backgroundMusic.Play();
    }
}
