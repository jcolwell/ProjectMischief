using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsUIControl : UIControl 
{
    // public
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle fixedAspectToggle;
    public Toggle fogOfWarToggle;

    public Toggle tuneViewConeUpdateToggle;
    public Slider ticksBetweenFrameSlider;
    public Text tickText;

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
        fogOfWarToggle.isOn = settingsData.fogOfWarOn;
        fixedAspectToggle.isOn = settingsData.fixedAspectRatio;

        if( tuneViewConeUpdateToggle != null )
        {
            tuneViewConeUpdateToggle.isOn = PersistentSceneData.GetPersistentData().tuneViewConeUpdate;
        }

        if( ticksBetweenFrameSlider != null && tickText != null )
        {
            ticksBetweenFrameSlider.value = PersistentSceneData.GetPersistentData().ticksBetweenFrames;
            tickText.text = ticksBetweenFrameSlider.value.ToString();
        }

        AudioListener.volume = 0.0f;
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

    public void ChangeFogOfWarOn()
    {
        settingsData.fogOfWarOn = fogOfWarToggle.isOn;
    }

    public void ChangeFixedAspectRatio()
    {
        settingsData.fixedAspectRatio = fixedAspectToggle.isOn;
    }

    public void ChangeTuneViewCone()
    {
        PersistentSceneData.GetPersistentData().tuneViewConeUpdate = tuneViewConeUpdateToggle.isOn;
    }

    public void ChangeTicks()
    {
        PersistentSceneData.GetPersistentData().ticksBetweenFrames = (uint)ticksBetweenFrameSlider.value;
        tickText.text = ticksBetweenFrameSlider.value.ToString();
    }

    protected override void DurringCloseUI()
    {
        SettingsInitializer.InitializeSettings();
    }
}
