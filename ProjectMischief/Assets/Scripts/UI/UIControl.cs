﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIControl : MonoBehaviour 
{
    //public
    public bool pauseTimeWhenLoaded = true;

    // Protected
    protected UITypes uiType;

    // private 
    private int renderOrder = 1; // higher numbers are rendered first

    //public
    public UIControl(UITypes type)
    {
        uiType = type;
    }

    public UIControl(UITypes type, int order)
    {
        uiType = type;
        renderOrder = order;
    }

	public void CloseUI()
	{
        DurringCloseUI();
        if (pauseTimeWhenLoaded)
        {
            Time.timeScale = 1.0f;
        }
		Destroy(this.gameObject);
	}

    public void SetCanvas()
    {
        GameObject canvasObject = transform.FindDeepChild("Canvas").gameObject;
        Canvas canvas = canvasObject.GetComponent<Canvas>();

        SettingsData settingData = PersistentSceneData.GetPersistentData().GetSettingsData();

        if (settingData.fixedAspectRatio)// place for settings check
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1.0f;
            canvas.sortingOrder = renderOrder;
        }
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = renderOrder;
        }
    }

    //private
        // DO NOT OVERIDE THESE 2 FUNCTIONS IN THE CHILD CLASSES IF YOU DO DEATH ONTO YOU
    void OnEnable() 
	{
        UIManager.instance.RegisterUI(gameObject, uiType);
        SetCanvas();

        DurringOnEnable();
        if (pauseTimeWhenLoaded)
        {
            Time.timeScale = 0.0f;
        }
	}

    void OnDestroy()
    {
        UIManager.instance.UnRegisterUI( uiType );
        DurringDestroy();
    }

    // protected
    protected virtual void DurringOnEnable()
    {
    }

    protected virtual void DurringDestroy()
    {
    }

    protected virtual void DurringCloseUI()
    {
    }
}

// http://answers.unity3d.com/questions/799429/transformfindstring-no-longer-finds-grandchild.html
public static class TransformDeepChildExtension
{
	//Breadth-first search
	public static Transform FindDeepChild(this Transform aParent, string aName)
	{
		Transform result = aParent.Find(aName);
		if (result != null)
			return result;
		foreach(Transform child in aParent)
		{
			result = child.FindDeepChild(aName);
			if (result != null)
				return result;
		}
		return null;
	}
}