using UnityEngine;
using System.Collections;

public class EndOfLevel : MonoBehaviour 
{
    public string playerTag = "Player";
    public string nextLevel;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(playerTag))
        {
            UIOverLord.instance.EndLevel(nextLevel);
        }
    }
	
}
