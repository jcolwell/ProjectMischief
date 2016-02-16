//======================================================
// File:            GuardDispatchManager.cs
// Discription:     This script will dispatch the closest guard
//                  to investigate a point of interest
//======================================================

//======================================================
// Includes
//======================================================

using System;
using UnityEngine;
using System.Collections;
//======================================================

//======================================================
// GuardDispatchManager
//======================================================
public class GuardDispatchManager : MonoBehaviour
{
    //======================================================
    // Public Variables
    //======================================================
    public GuardAI[] guards = new GuardAI[0];
    public AlertLightManager lights = null;
    //======================================================

    //======================================================
    // Dispatch the "closest" guard to investigate a point of interest
    //======================================================
    public void DispatchGuard( Vector3 position )
    {
        //This is going to send the closest guard based on world position
        //Completely ignoring any and all obstacles...

        //Debug.Log( "PLAYER HAS BEEN SEEN! GO GET HIM!" );
        if( lights )
        {
            lights.ToggleLightsOn();
        }
            
        Array.Sort( guards, ( guard1, guard2 ) =>
        {
            float guard1Dist = Vector3.Distance( guard1.transform.position, position);
            float guard2Dist = Vector3.Distance( guard2.transform.position, position);
            return guard1Dist.CompareTo( guard2Dist );
        });

        guards[0].Investigate( position );
    }

    //======================================================

    void ReportIntruder()
    {
        if( lights )
        {
            lights.ToggleLightsOn();
        }
    }

    void ReportInteruterNeutralized()
    {
        if( lights )
        {
            lights.ToggleLightsOff();
        }
    }


    //======================================================
 }

//======================================================
