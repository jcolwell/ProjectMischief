using UnityEngine;
using System.Collections;

public class Sensor : MonoBehaviour 
{

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("picture")  == true)
		{
			other.gameObject.GetComponent<ArtPiece>().playerIsInRange =  true;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("picture")  == true)
		{
			other.gameObject.GetComponent<ArtPiece>().playerIsInRange =  false;
		}
	}
}
