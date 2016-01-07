using UnityEngine;
using System.Collections;

public class FrontEnd : MonoBehaviour 
{
    void Start()
    {
        GameObject canvasObject = transform.FindDeepChild("Canvas").gameObject;
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        SettingsData settingData = PersistentSceneData.GetPersistentData().GetSettingsData();

        if (settingData.fixedAspectRatio)// place for settings check
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1.0f;
        }
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadShop()
    {
        Application.LoadLevelAdditive( "UIStore" );
    }

	public void LoadLevelSelect()
	{
		UIManager.instance.LoadLevelSelect();
	}

    public void BringUpSettings()
    {
        UIManager.instance.LoadSettings();
    }
}
