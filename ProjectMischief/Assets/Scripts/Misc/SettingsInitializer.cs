using UnityEngine;
using System.Collections;

public class SettingsInitializer : MonoBehaviour 
{
    public GameObject fogOfWar = null;
    public SettingsData settingData = null;

    static public void InitializeSettings()
    {
        GameObject settingsInit = new GameObject();
        settingsInit.AddComponent<SettingsInitializer>();
        settingsInit.name = "SettingsInit";
    }

	void Start () 
    {
        PersistentSceneData sceneData = PersistentSceneData.GetPersistentData();
        settingData = sceneData.GetSettingsData();
        UpdateViewCones();
        SetFogOfWar();
        SetVolume();
        UIManager.instance.ResetAllUICanvas();
        Destroy( gameObject );
	}

    public void UpdateViewCones()
    {
        if(!PersistentSceneData.GetPersistentData().tuneViewConeUpdate)
        {
            return;
        }

        VisionCone[] viewCones = GameObject.FindObjectsOfType<VisionCone>();

        if( viewCones == null )
        {
            return;
        }

        uint ticksBetweenUpdate = PersistentSceneData.GetPersistentData().ticksBetweenFrames;

        for( uint i = 0; i < viewCones.Length; ++i )
        {
            viewCones[i].ticksBetweenUpdate = ticksBetweenUpdate;
        }

    }

    public void SetFogOfWar()
    {
        fogOfWar = UIManager.instance.GetFogOfWar();
        if( fogOfWar == null )
        {
            fogOfWar = GameObject.Find( "Fow" );
            if( fogOfWar == null )
            {
                fogOfWar = GameObject.FindGameObjectWithTag( "Fow" );
            }
        }

        if( fogOfWar != null )
        {     
            fogOfWar.SetActive( settingData.fogOfWarOn );
            GameObject player = GameObject.Find( "Actor" );

            if( player == null )
            {
                player = GameObject.Find( "Actor(Clone)" );
            }

            if( player != null )
            {
                player.GetComponent<FogOfWar>().Initialize();
            }
        }
    }

    public void SetVolume()
    {
        AudioListener.volume = settingData.sfxSoundLevel * 0.01f;

        GameObject musicObj = GameObject.Find( "BackgroundMusic" );
        if( musicObj != null )
        {
            BackgroundMusicManager musicSource = musicObj.GetComponent<BackgroundMusicManager>();
            musicSource.setVolume( settingData.musicSoundLevel * 0.01f );

            if(!musicSource.isPlaying())
            {
                musicSource.Play();
            }
        }
    }
}
