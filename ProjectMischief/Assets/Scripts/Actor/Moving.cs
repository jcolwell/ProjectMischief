using UnityEngine;
using System.Collections;

public class Moving : MonoBehaviour 
{
    static public Moving instance;
    Vector3 pos;
    Vector3 Target;
    RaycastHit hit;
    NavMeshAgent agent;
    bool leftClickFlag = true;
    public string floorTag;
    public string PictureTag;
    public Quaternion lookRotation;
    public LayerMask cullingMask;
    int speed = 3;

    public bool use2DReticle = false;
    public GameObject movementReticle;

    enum State
    {
        Idle,
        Stealth,
    };

    State state;

    void Awake ()
    {
        pos = new Vector3(-2.0f, 0.25f, -6.35f);
    }

    void Start()
    {
        instance = this;
        agent = GetComponent<NavMeshAgent>();
    }


	void Update ()
    {

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
                    //Vector3 direction = (hit.transform.position - gameObject.transform.position).normalized;

                    state = State.Stealth;


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



        switch( state )
        {
        case State.Stealth:
        UpdateStealth(speed);
        break;
        case State.Idle:
        UpdateIdle();
        break;
        }
	}

    void changePos( Vector3 p )
    {
        pos = p;
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

    void UpdateIdle()
    {

    }

    public void SetSpeed(int s)
    {
        speed = s;
    }

    public void SetTarget(Vector3 tar)
    {
        Target = tar;
    }
}
