// this spawns the player

using UnityEngine;
using System.Collections;

public class StartOfLevel : MonoBehaviour 
{
   

    public GameObject player;

	// Use this for initialization
	void Awake () 
    {
        if (player != null)
        {
            GameObject.Instantiate( player, transform.position, transform.rotation );
        }
        Destroy(gameObject);
	}
	
}
