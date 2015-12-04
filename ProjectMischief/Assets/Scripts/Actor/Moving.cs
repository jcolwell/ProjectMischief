﻿//======================================================
// File: GuardAI.cs
// Description:    This Script will drive Guard AI
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using System.Collections;
//======================================================


//======================================================
// Class Moving
//======================================================
public class Moving : MonoBehaviour 
{
    //======================================================
    // Public
    //======================================================
    bool leftClickFlag = true;
    public string floorTag;
    public string PictureTag;
    public Quaternion lookRotation;
    public LayerMask cullingMask;
    public int speed = 3;
    public bool use2DReticle = false;
    public GameObject movementReticle;
    //======================================================

    //======================================================
    // Private
    //======================================================
    Vector3 pos;
    Vector3 Target;
    RaycastHit hit;
    NavMeshAgent agent;
    //======================================================



    enum State
    {
        Idle,
        Stealth,
    };

    State state;

    void Awake ()
    {
    }

    void Start()
    {
        pos = gameObject.transform.position;
        agent = GetComponent<NavMeshAgent>();
    }


	void Update ()
    {

        if (!agent.enabled)
        {
            return;
        }

#if UNITY_ANDROID

        if( Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began )
        {
            Ray ray = Camera.main.ScreenPointToRay( Input.GetTouch(0).position);
            if( Physics.Raycast( ray, out hit, 100 ) && Time.timeScale != 0.0f )
            {
                if( hit.transform.tag == floorTag )
                {
                    if(movementReticle != null)
                    {
                        Instantiate( movementReticle, hit.point, Quaternion.identity);
                    }

                    float X = hit.point.x;
                    float Z = hit.point.z;
                    Target = new Vector3( X, gameObject.transform.position.y, Z );
                    Vector3 direction = (hit.transform.position - gameObject.transform.position).normalized;
                    lookRotation = Quaternion.LookRotation( direction );

                    StartCoroutine( RotateAgent( transform.rotation, lookRotation ) );

                    state = State.Stealth;
                }

                if( hit.transform.tag == PictureTag )
                {
                    ArtPiece art = hit.collider.gameObject.GetComponent<ArtPiece>();
                    if( art.playerIsInRange == true )
                    {
                        art.LoadMenu();
                        PlayerCheckPoint playerCheckPoint = gameObject.GetComponent<PlayerCheckPoint>();
                        if( playerCheckPoint != null )
                        {
                            playerCheckPoint.SetCheckPoint( gameObject.transform.position );
                        }
                    }
                }
            }
        }
#else
        if ( Input.GetKey( KeyCode.Mouse0 ) && leftClickFlag )
        {
            leftClickFlag = false;
        }

        if( !Input.GetKey( KeyCode.Mouse0 ) && !leftClickFlag )
        {
            leftClickFlag = true;
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

            if( Physics.Raycast( ray, out hit, 100, cullingMask ) && Time.timeScale != 0.0f )
            {
                if( hit.transform.tag == floorTag )
                {
                    if (movementReticle != null && !use2DReticle)
                    {
                        Instantiate(movementReticle, hit.point, Quaternion.identity);
                    }
                    else if (use2DReticle)
                    {
                        UIManager.instance.Spawn2DReticle(Camera.main, hit.point);
                    }

                    float X = hit.point.x;
                    float Z = hit.point.z;
                    Target = new Vector3( X, gameObject.transform.position.y, Z );
                }

                if( hit.transform.tag == PictureTag )
                {
                    ArtPiece art = hit.collider.gameObject.GetComponent<ArtPiece>();
                    GetComponentInChildren<Sensor>().CheckIfInRange(hit.collider);
                    if( art.playerIsInRange == true )
                    {
                        art.LoadMenu();
                        PlayerCheckPoint playerCheckPoint = gameObject.GetComponent<PlayerCheckPoint>();
                        if( playerCheckPoint != null )
                        {
                            playerCheckPoint.SetCheckPoint( gameObject.transform.position );
                        }
                    }
                }
            }
        }
#endif


        UpdateStealth(speed);
	}

    void UpdateStealth(int speed)
    {
        agent.SetDestination(Target);

        agent.speed = speed;

        if (pos == Target)
        {
            state = State.Idle;
        }
    }

    //For Equipment Stats
    public void SetSpeed(int s)
    {
        speed = s;
    }

    public void setTarget(Vector3 t)
    {
        Target = t;
        agent.ResetPath();
    }

    public void Reset()
    {
        agent.ResetPath();
        agent.velocity = new Vector3();
        //agent.Stop();
    }

    public void ToggleNavMeshAgent()
    {
        agent.enabled = !agent.enabled;
    }

}

