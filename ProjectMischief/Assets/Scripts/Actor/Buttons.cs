using UnityEngine;
using System.Collections;

public class Buttons : MonoBehaviour 
{
    GameObject player;
    int speed = 1;
    float rad = 0.01f;

	void Start () 
    {
        player = GameObject.Find( "Actor" );
	}
	
	void Update () 
    {
        if( Input.GetKeyDown( KeyCode.S) )
        {
            player.GetComponent<Moving>().SetSpeed(speed);
        }

        else if( Input.GetKeyDown( KeyCode.A ) )
        {
            player.GetComponent<Moving>().SetSpeed( -speed );
        }

        else if( Input.GetKeyDown( KeyCode.F ) )
        {
            player.GetComponent<FogOfWar>().ChangeRadius( rad );
        }

        else if( Input.GetKeyDown( KeyCode.D ) )
        {
            player.GetComponent<FogOfWar>().ChangeRadius( -rad );
        }
	}
}
