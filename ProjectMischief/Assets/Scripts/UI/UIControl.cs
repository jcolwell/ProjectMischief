using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIControl : MonoBehaviour 
{
    //public
    public bool pauseTimeWhenLoaded = true;

    //public
	public void CloseUI()
	{
        if (pauseTimeWhenLoaded)
        {
            Time.timeScale = 1.0f;
        }
		Destroy(this.gameObject);
	}

    //private
	void Awake () 
	{
        OnAwake();
        if (pauseTimeWhenLoaded)
        {
            Time.timeScale = 0.0f;
        }
	}

    protected virtual void OnAwake()
    {

    }
}
