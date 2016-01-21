using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour 
{
    AudioSource backgroundMusic;

    void Awake()
    {
        backgroundMusic = GetComponent<AudioSource>();

        backgroundMusic.ignoreListenerPause = true;
        backgroundMusic.ignoreListenerVolume = true;
    }

    void setVolume(float v)
    {
        backgroundMusic.volume = v;
    }

    void Mute()
    {
        backgroundMusic.mute = true;
    }

    void Pause()
    {
        backgroundMusic.Pause();
    }

    void Play()
    {
        backgroundMusic.Play();
    }
}
