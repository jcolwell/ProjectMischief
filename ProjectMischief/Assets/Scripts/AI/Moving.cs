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
    bool IsRotating = true;
    public Quaternion lookRotation;
    float RotationSpeed = 5;
    
    enum State
    {
        Idle,
        Stealth,
    };

    State state;

    void Awake ()
    {
        //GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        pos = new Vector3(-2.0f, 0.25f, -6.35f);
    }

    void Start()
    {
        instance = this;
        agent = GetComponent<NavMeshAgent>();
    }


	void Update () 
    {
        if( Input.GetKey( KeyCode.Mouse0 ) && leftClickFlag )
        {
            leftClickFlag = false;
        }
        if( !Input.GetKey( KeyCode.Mouse0 ) && !leftClickFlag )
        {
            leftClickFlag = true;
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
            if( Physics.Raycast( ray, out hit, 100 ) )
            {
                if( hit.transform.tag == floorTag )
                {

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

        switch( state )
        {
        case State.Stealth:
        UpdateStealth();
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

    void UpdateStealth()
    {
        
        

        if(IsRotating == true)
        {
            agent.SetDestination(Target);
        }
   
            if (pos == Target)
            {
                state = State.Idle;
            }
    }

    void UpdateIdle()
    {

    }

    public void SetTarget(Vector3 tar)
    {
        Target = tar;
    }

    IEnumerator RotateAgent(Quaternion currentRotation, Quaternion targetRotation) 
    {
        IsRotating = true;
        while( currentRotation != targetRotation ) 
        {
             transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, RotationSpeed * Time.deltaTime);

             yield return 1;
         }
         IsRotating = false;
    }
}
