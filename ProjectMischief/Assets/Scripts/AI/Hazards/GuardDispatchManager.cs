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
    //======================================================

    //======================================================
    // Dispatch the "closest" guard to investigate a pont of interest
    //======================================================
    void DispatchGuard( Vector3 position )
    {
        //This is going to send the closest guard based on world position
        //Completely ignoring any and all obstacles...
        Array.Sort( guards, (guard1, guard2)=>
        {
            float guard1Dist = Vector3.Distance(guard1.transform.position, position);
            float guard2Dist = Vector3.Distance(guard2.transform.position, position);
            return (guard1Dist < guard2Dist)? 0 : -1;
        });

        guards[0].Investigate( position );
    }

 }
//======================================================
