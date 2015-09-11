using UnityEngine;
using System.Collections;

public class Sensor : MonoBehaviour 
{

	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("I'M FUCKING HITING IT, FUCK YOU");
		if(other.CompareTag("picture")  == true)
		{
			Debug.Log("I'm in Range!");
			other.gameObject.GetComponent<ArtPiece>().playerIsInRange =  true;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("picture")  == true)
		{
			Debug.Log("I'm not in Range!");
			other.gameObject.GetComponent<ArtPiece>().playerIsInRange =  false;
		}
	}
}
