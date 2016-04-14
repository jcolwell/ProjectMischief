//=============================================================
// FILE:            Rotation.cs
// DESCRIPTION:     To rotate the object this script is attached to
//=============================================================

//=============================================================
// INCLUDES
//=============================================================
using UnityEngine;
using System.Collections;
//=============================================================

//=============================================================
// ROTATION
//=============================================================
public class Rotation : MonoBehaviour 
{

    //=============================================================
    // PUBLIC
    //=============================================================
    public bool isContinuous = false;
    public float viewArc = 90.0f ;
    public float startingAngleDegree = 0.0f;
    public float rotationDuration = 6.0f;
    public float swingDelay = 3.0f;
    //=============================================================

    //=============================================================
    // Private
    //=============================================================
    float currentAngle;
    float targetAngle;
    float minAngle;
    float maxAngle;
    float currentTime;
    float angleInc;
    float angleDir = 1.0f;
    float intenralRotDuration; //used to prevent the script form going hay wire when rotation durtion is being modifed at runtime
    float currentDelayTime;
    float veiwAngle;
    //=============================================================
	
    void Start()
    {
        minAngle = transform.eulerAngles.y - ( viewArc * 0.5f );
        maxAngle = transform.eulerAngles.y + ( viewArc * 0.5f );

        angleInc = viewArc / rotationDuration;

        veiwAngle = angleInc;

        currentAngle = minAngle + startingAngleDegree;
        targetAngle = ( currentAngle < maxAngle )? maxAngle : minAngle ;
        currentTime = ( startingAngleDegree / viewArc ); //* rotationDuration;

        intenralRotDuration = rotationDuration;

        this.transform.eulerAngles = new Vector3( 0.0f, currentAngle, 0.0f );

        if(isContinuous)
        {
            swingDelay = 0.0f;
        }

        //sound = gameObject.GetComponent<AudioSource>();
        //sound.Play();
    }

	// Update is called once per frame
	void Update () 
    {
        currentTime += Time.deltaTime;
        currentAngle = transform.eulerAngles.y + angleInc * Time.deltaTime;      
        transform.eulerAngles = new Vector3( 0.0f, currentAngle, 0.0f );

        //Swing back
        if( currentTime >= intenralRotDuration )
        {
            currentDelayTime += Time.deltaTime;
            angleInc = 0;
            
            if( currentDelayTime >= swingDelay )
            {
                if( !isContinuous )
                {
                    angleDir = -angleDir;
                }
                angleInc = veiwAngle * angleDir;
                targetAngle = ( targetAngle >= maxAngle ) ? minAngle : maxAngle;
                intenralRotDuration = rotationDuration;
                currentTime = 0.0f;
                currentDelayTime = 0.0f;
            }
             
        }
        //sound.Stop();

	}

}
//=============================================================

//=============================================================
