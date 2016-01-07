using UnityEngine;
using System.Collections;

public class FrontEnd : MonoBehaviour 
{
    void OnEnable()
    {
        GameObject canvasObject = transform.FindDeepChild("Canvas").gameObject;
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        if (true)// place for settings check
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1.0f;
        }
        //else
        //{
        //    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //}
    }

    public void LoadLevel(string level)
    {
        Application.LoadLevel( level );
    }

    public void LoadLevel( int level )
    {
        Application.LoadLevel( level );
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
