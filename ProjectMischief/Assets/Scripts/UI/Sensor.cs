using UnityEngine;
using System.Collections;

public class Sensor : MonoBehaviour 
{

	void OnTriggerEnter(Collider other)
	{
		//if(other.CompareTag("picture")  == true)
		//{
		//	other.gameObject.GetComponent<ArtPiece>().playerIsInRange =  true;
		//}
	}

    public void CheckIfInRange(Collider other)
    {
        Ray ray = new Ray(gameObject.transform.position, (other.transform.position - gameObject.transform.position));
        float raduis = gameObject.GetComponentInChildren<SphereCollider>().radius;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raduis) && hit.collider.CompareTag(other.tag))
        {
            other.gameObject.GetComponent<ArtPiece>().playerIsInRange = true;
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
