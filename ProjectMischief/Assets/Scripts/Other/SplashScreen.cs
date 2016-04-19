//==================================================================
// File:            SplashScreen.cs
// Description:     Fade In/Out our splash screen image and then
//                  load the main menu
// Author:          Josh Colwell
//==================================================================

//==================================================================
// INCLUDES
//==================================================================
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
//==================================================================

//==================================================================
// SplashScreen
//==================================================================
public class SplashScreen : MonoBehaviour 
{

    //==============================================================
    // PUBLIC
    //==============================================================   
    public Image splashScreen;
    public Image backgroundScreen;
    public float delay = 1.0f;
    public float fadeInRate = 0.5f;
    public float fadeOutRate = 0.5f;
    //==============================================================

    //==============================================================
    // PUBLIC
    //==============================================================
    private enum State
    {
        FadeIn,
        FadeOut,
        SmartFade
    }

    private State state = State.FadeIn;
    private Color color;
    public float alpha = 0.0f;
    private bool isFrontEndLoaded = false;
    //==============================================================

    //==============================================================
    // METHODS
    //==============================================================
   
    void Start()
    {
        Time.timeScale = 1.0f;
        color = splashScreen.color;
        
        color.a = 0.0f;
        splashScreen.color = color;
    }

    //==============================================================

	void Update () 
    {
        delay -= Time.deltaTime;

        if( delay <= 0.0f )
        {
            UpdateSplashScreen();
        }
	}

    //==============================================================

    private void UpdateSplashScreen()
    {
        switch( state )
        {
            case State.FadeIn:
            {
                alpha += fadeInRate * Time.deltaTime;
                Mathf.Clamp( alpha, 0.0f, 1.0f );

                color.a = alpha;
                splashScreen.color = color;

                if( alpha >= 1.0f )
                {
                    state = State.FadeOut;
                }
                break;
            }

            case State.FadeOut:
            {
                //LoadLevel();
                if( !isFrontEndLoaded )
                {
                        //Debug.Log( "[SplashScreen] Loading the FrontEnd " );
                        SceneManager.LoadScene("FrontEnd", LoadSceneMode.Additive);

                    isFrontEndLoaded = true;
                }
                Time.timeScale = 1.0f;

                alpha -= fadeOutRate * Time.deltaTime;
                Mathf.Clamp( alpha, 0.0f, 1.0f );

                color.a = alpha;
                splashScreen.color = color;
                
                if( alpha <= 0.0f )
                {
                    color = backgroundScreen.color;
                    alpha = backgroundScreen.color.a;
                    state = State.SmartFade;
                }
                break;
            }

            case State.SmartFade:
            {

                alpha -= fadeOutRate * Time.deltaTime;
                Mathf.Clamp( alpha, 0.0f, 1.0f );

                color.a = alpha;
                backgroundScreen.color = color;

                if( alpha <= 0.0f )
                {
                    IntroControl.TurnOnIntro();
                    Destroy( this.gameObject );
                }
                break;
            }
        }
    }

    //==============================================================
    
    void LoadLevel()
    {

        if( !isFrontEndLoaded )
        {
            //Debug.Log( "[SplashScreen] Loading the FrontEnd " );
            SceneManager.LoadScene("FrontEnd", LoadSceneMode.Additive);

            isFrontEndLoaded = true;
        }
        return;
    }

    //==============================================================
}
//==================================================================