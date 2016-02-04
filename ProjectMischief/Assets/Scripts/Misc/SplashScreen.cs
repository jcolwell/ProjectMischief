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
        FadeOut
    }

    private State state = State.FadeIn;
    private Image splashScreen;
    private Color color;
    private float alpha = 0.0f;
    //==============================================================

    //==============================================================
    // METHODS
    //==============================================================
   
    void Start()
    {
        splashScreen = GetComponent<Image>();
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
        Debug.Log( color.a );
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
                alpha -= fadeOutRate * Time.deltaTime;
                Mathf.Clamp( alpha, 0.0f, 1.0f );

                color.a = alpha;
                splashScreen.color = color;
                
                if( alpha <= 0.0f )
                {
                    Application.LoadLevel( "FrontEnd" );
                }
                break;
            }
        }
    }
    
    //==============================================================
}
//==================================================================