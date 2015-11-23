using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIControl : MonoBehaviour 
{
    public bool pauseTimeWhenLoaded = true;

	public void CloseUI()
	{
        if (pauseTimeWhenLoaded)
        {
            Time.timeScale = 1.0f;
        }
		Destroy(this.gameObject);
	}

    // http://answers.unity3d.com/questions/147276/why-is-start-from-my-base-class-ignored-in-my-sub.html
    // Using portected new to allow child class to use this function
	protected new void Awake () 
	{
        ++numOfUIOpen;
        if (pauseTimeWhenLoaded)
        {
            Time.timeScale = 0.0f;
        }
	}

    //statics
    static int numOfUIOpen = 0;

    static public int GetNumOfUIOpen()
    {
        return numOfUIOpen;
    }

    // Using portected new to allow child class to use this function
    protected new void OnDestroy()
    {
        --numOfUIOpen;
    }
}
