using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsUIControl : UIControl 
{
    // public
    public Slider musicSlider;
    public Slider sfxSlider;

    public GameObject IntroButton = null;

    SettingsData settingsData;

    // Private
    SettingsUIControl() : base(UITypes.settings, 5)
    { }

    void Start()
    {
        settingsData = PersistentSceneData.GetPersistentData().GetSettingsData();

        // Set sliders and Toggles
        musicSlider.value = settingsData.musicSoundLevel;
        sfxSlider.value = settingsData.sfxSoundLevel;
        AudioListener.volume = 0.0f;

        if(UIManager.instance.IsUIActive(UITypes.frontEnd))
        {
            IntroButton.SetActive(true);
        }
        else
        {
            IntroButton.SetActive(false);
        }
    }

    // Public
        // Functions for buttons

    public void ResetData()
    {
        PersistentSceneData.GetPersistentData().ResetData();
    }

    public void ChangeMusicSoundLevel()
    {
        settingsData.musicSoundLevel = musicSlider.value;
        GameObject musicObj = GameObject.Find( "BackgroundMusic" );
        if( musicObj != null )
        {
            BackgroundMusicManager musicSource = musicObj.GetComponent<BackgroundMusicManager>();
            musicSource.setVolume( settingsData.musicSoundLevel * 0.01f );
        }
    }

    public void ChangeSFXSoundLevel()
    {
        settingsData.sfxSoundLevel = sfxSlider.value;
    }

    protected override void DurringCloseUI()
    {
        SettingsInitializer.InitializeSettings();
    }

    public void TurnOnIntro()
    {
        UIManager.instance.ActivateIntroObject();
        IntroControl.TurnOnIntro();
        IntroControl.SetIntroToNotLoadLevelWhenDone();
        CloseUI();
    } 
}
