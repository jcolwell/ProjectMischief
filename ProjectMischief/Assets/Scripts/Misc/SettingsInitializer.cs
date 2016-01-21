using UnityEngine;
using System.Collections;

public class SettingsInitializer : MonoBehaviour 
{
    GameObject fogOfWar = null;

    static public void InitializeSettings()
    {
        GameObject settingsInit = new GameObject();
        settingsInit.AddComponent<SettingsInitializer>();
        settingsInit.name = "SettingsInit";
    }

	void Start () 
    {

        UpdateViewCones();
        SetFogOfWar();
        UIManager.instance.ResetAllUICanvas();
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
            SettingsData settingData = PersistentSceneData.GetPersistentData().GetSettingsData();
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
        GameObject player = GameObject.Find( "Actor" );

        if( player == null )
        {
            player = GameObject.Find( "Actor(Clone) " );
        }

        if (player != null)
        {
            AudioListener listner = player.GetComponent<AudioListener>();
            //listner.
        }
    }
}
