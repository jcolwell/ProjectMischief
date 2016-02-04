//=================================================================
// File:            AlertLightController
// Discription:     
//=================================================================

//=================================================================
// INCLUDES
//=================================================================
using UnityEngine;
using System.Collections;
//=================================================================

//=================================================================
// ALERT LIGHT CONTROLLER
//=================================================================
public class AlertLightController : MonoBehaviour 
{

    //=============================================================
    // PUBLIC
    //=============================================================
    public float changeRate = 0.01f;
    public float minIntensity = 0.0f;
    public float maxIntensity = 3.0f;
    //=============================================================

    //=============================================================
    // PRIVATE
    //=============================================================
    private bool isActive = true;
    Light light;
    //=============================================================

    //=============================================================
    // METHODS
    //=============================================================
	
    void Start () 
    {
        light = GetComponent<Light>();
        light.intensity = 0.0f;
	}

    //=============================================================	

	void Update () 
    {
        if( isActive )
        {
            light.intensity += (changeRate * Time.deltaTime);
            //changeRate *= (light.intensity >= maxIntensity || light.intensity <= minIntensity) ? -1.0f : 1.0f;
            if(light.intensity >= maxIntensity && changeRate > 0.0f)
            {
                changeRate = -changeRate;
            }
            else if( light.intensity <= minIntensity && changeRate < 0.0f )
            {
                changeRate = -changeRate;
            }
        }
	}

    //=============================================================

    public void ToggleOn()
    {
        isActive = true;
        light.intensity = minIntensity;
    }

    public void ToggleOff()
    {
        isActive = false;
        light.intensity = 0.0f;
    }
}

//=================================================================