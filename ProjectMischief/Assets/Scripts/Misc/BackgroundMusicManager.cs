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
}
