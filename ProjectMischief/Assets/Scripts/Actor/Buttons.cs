using UnityEngine;
using System.Collections;

public class Buttons : MonoBehaviour 
{
    GameObject player;
    int curSpeed = 1;
    int speed = 1;
    float rad = 0.01f;
    float curRad = 0.01f;

	void Start () 
    {
        player = GameObject.Find( "Actor" );

        if(player == null)
        {
            player = GameObject.Find( "Actor(Clone) " );
        }
	}
	
	void Update () 
    {
        if( Input.GetKeyDown( KeyCode.S) )
        {
            player.GetComponent<Moving>().SetSpeed(curSpeed);
            curSpeed += speed;
        }

        else if( Input.GetKeyDown( KeyCode.A ) )
        {
            player.GetComponent<Moving>().SetSpeed( curSpeed );
            curSpeed -= speed;
        }

        else if( Input.GetKeyDown( KeyCode.F ) )
        {
            player.GetComponent<FogOfWar>().ChangeRadius( curRad );
            curRad += rad;
        }

        else if( Input.GetKeyDown( KeyCode.D ) )
        {
            player.GetComponent<FogOfWar>().ChangeRadius( curRad );
            curRad -= rad;
        }
	}
}
