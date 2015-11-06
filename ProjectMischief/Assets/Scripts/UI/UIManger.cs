using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManger : MonoBehaviour 
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

	void Awake () 
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

    void OnDestroy()
    {
        --numOfUIOpen;
    }
}
