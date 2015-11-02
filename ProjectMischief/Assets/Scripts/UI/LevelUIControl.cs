using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUIControl : MonoBehaviour 
{
    float timeElapsed;

    float deltaTime = 0;
    float lastFramesTime;

    GameObject timer;
    GameObject menu;
    Text timerText;

	void Start () 
    {
        UIOverLord.instance.RegisterUI(gameObject, UITypes.level);

        // Grab relvent objects
        menu = GameObject.Find( "MenuLevel" );
        timer = GameObject.Find("Timer");
        GameObject temp = GameObject.Find( "TimerText" );
        // TODO: add asserts
        timerText = temp.GetComponent<Text>();

        // itailize varibles
        timeElapsed = 0.0f;

        lastFramesTime = Time.realtimeSinceStartup;
	}
	
	void Update () 
    {
        CalculateDeltaTime();

        if( !UIOverLord.gameIsPaused)
        {
            timeElapsed += deltaTime;
        }

        const int kSec = 60; // num of seconds per minute;
        string minSec = string.Format( "{0}:{1:00}", (int)(timeElapsed / kSec), (int)(timeElapsed % kSec) );
        timerText.text = "Time " + minSec;

        // if any other ui is open make this ui invisble
        int uiOpen = UIManger.GetNumOfUIOpen();
        menu.SetActive( uiOpen == 1 );

        
	}

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }

    void OnDestroy()
    {
        UIOverLord.instance.UnRegisterUI(UITypes.level);
    }

    public float GetTimeElapsed()
    {
        return timeElapsed;
    }

    public void TurnTimerOff()
    {
        UIOverLord.gameIsPaused = true;
        timer.SetActive(false);
    }

    public void TurnTimerOn()
    {
        UIOverLord.gameIsPaused = false;
        timer.SetActive(true);
    }

    public void ToMenu()
    {
        Application.LoadLevel( "FrontEnd" );
    }
}

