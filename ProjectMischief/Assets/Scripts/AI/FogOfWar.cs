﻿using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour
{
    FOV2DEyes eyes;
    FOV2DVisionCone visionCone;
    //float speed = -5;

    void Start()
    {
        eyes = GetComponentInChildren<FOV2DEyes>();
        visionCone = GetComponentInChildren<FOV2DVisionCone>();
    }

    //void FixedUpdate()
    //{
    //    if (transform.position.x < -10 || transform.position.x > 10)
    //    {
    //        speed *= -1;
    //    }

    //    transform.position = new Vector3(transform.position.x + speed * Time.fixedDeltaTime, transform.position.y, transform.position.z);
    //}

    void Update()
    {
        bool playerInView = false;

        foreach( RaycastHit hit in eyes.hits )
        {
            if( hit.transform && hit.transform.tag == "fog" )
            {
                hit.collider.gameObject.SetActive( false );
            }
        }

        if( playerInView )
        {
            visionCone.status = FOV2DVisionCone.Status.Alert;
        }
        else
        {
            visionCone.status = FOV2DVisionCone.Status.Idle;
        }
    }
}