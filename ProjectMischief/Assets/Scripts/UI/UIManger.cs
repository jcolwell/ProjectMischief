using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManger : MonoBehaviour 
{
	
	static bool isRunning = false;

	public void CloseUI()
	{
		Time.timeScale = 1.0f;
		Destroy(this.gameObject);
		isRunning = false;
	}

	void Awake () 
	{
		if(isRunning == true)
		{
            Destroy( this.gameObject );
		}
	}
}
