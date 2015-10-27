using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManger : MonoBehaviour 
{
	public void CloseUI()
	{
		Time.timeScale = 1.0f;
		Destroy(this.gameObject);
	}

	void Awake () 
	{
        ++numOfUIOpen;
        Time.timeScale = 0.0f;
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
