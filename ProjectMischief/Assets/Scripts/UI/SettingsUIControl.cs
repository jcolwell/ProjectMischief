using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsUIControl : UIControl 
{
    // Private
    Slider musicSlider;
    Slider sfxSlider;
    Slider masterSlider;
    Toggle fixedAspectToggle;
    Toggle fogOfWarToggle;

    SettingsData settingsData;

    // Private
    SettingsUIControl() : base(UITypes.settings, 5)
    { }

    void Start()
    {
        // Grab Relvent Objects
        GameObject temp;

        temp = GameObject.Find("SoundMaster");
        masterSlider = temp.GetComponent<Slider>();

        temp = GameObject.Find("SFX");
        sfxSlider = temp.GetComponent<Slider>();

        temp = GameObject.Find("Music");
        musicSlider = temp.GetComponent<Slider>();

        temp = GameObject.Find("FogOfWarToggle");
        fogOfWarToggle = temp.GetComponent<Toggle>();

        temp = GameObject.Find("FixedAspectRatioToggle");
        fixedAspectToggle = temp.GetComponent<Toggle>();

        temp = null;

        settingsData = PersistentSceneData.GetPersistentData().GetSettingsData();

        // Set sliders and Toggles
        masterSlider.value = settingsData.masterSoundLevel;
        musicSlider.value = settingsData.musicSoundLevel;
        sfxSlider.value = settingsData.sfxSoundLevel;
        fogOfWarToggle.isOn = settingsData.fogOfWarOn;
        fixedAspectToggle.isOn = settingsData.fixedAspectRatio;
    }

    // Public
        // Functions for buttons
    public void ResetData()
    {
        PersistentSceneData.GetPersistentData().ResetData();
    }

    public void ChangeMasterSoundLevel()
    {
        
        settingsData.masterSoundLevel = masterSlider.value;
    }

    public void ChangeMusicSoundLevel()
    {
        settingsData.musicSoundLevel = masterSlider.value;
    }

    public void ChangeSFXSoundLevel()
    {
        settingsData.sfxSoundLevel = sfxSlider.value;
    }

    public void ChangeFogOfWarOn()
    {
        settingsData.fogOfWarOn = fogOfWarToggle.isOn;
        UIManager.instance.SetFogOfWar();
    }

    public void ChangeFixedAspectRatio()
    {
        settingsData.fixedAspectRatio = fixedAspectToggle.isOn;
        UIManager.instance.ResetAllUICanvas();
    }
}
