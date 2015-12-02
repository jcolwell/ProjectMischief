using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIControl : MonoBehaviour 
{
    //public
    public bool pauseTimeWhenLoaded = true;

    // Protected
    protected UITypes uiType;

    //public
    public UIControl(UITypes type)
    {
        uiType = type;
    }

	public void CloseUI()
	{
        if (pauseTimeWhenLoaded)
        {
            Time.timeScale = 1.0f;
        }
		Destroy(this.gameObject);
	}

    //private
    void OnEnable() 
	{
        UIManager.instance.RegisterUI(gameObject, uiType);

        DurringOnEnable();
        if (pauseTimeWhenLoaded)
        {
            Time.timeScale = 0.0f;
        }
	}

    protected virtual void DurringOnEnable()
    {
    }



	void OnDestroy()
    {
        UIManager.instance.UnRegisterUI(uiType);
        DurringDestroy();
    }

    protected virtual void DurringDestroy()
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