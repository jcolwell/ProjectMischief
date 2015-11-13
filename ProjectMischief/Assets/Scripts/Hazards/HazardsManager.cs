using UnityEngine;
using System.Collections;

public class HazardsManager : MonoBehaviour 
{
    public GameObject[] cameras;
    public GameObject[] lasers;
    bool isTurn = true;

	void Update () 
    {
        laser laserTemp;

        if(isTurn)
        {

            if(cameras[0].gameObject.transform.rotation.eulerAngles.y >= 240 )
            {
                laserTemp = lasers[0].GetComponent<laser>();
                laserTemp.ToggleLazer(false);
                StartCoroutine(pause(false));
            }
            else
            {
                Vector3 turn = new Vector3( 0, 100 * Time.deltaTime, 0 );
                cameras[0].gameObject.transform.Rotate( turn );
                laserTemp = lasers[0].GetComponent<laser>();
                laserTemp.ToggleLazer(true);

            }
        }
        else
        {

            if( cameras[0].gameObject.transform.rotation.eulerAngles.y <= 90 )
            {
                laserTemp = lasers[0].GetComponent<laser>();
                laserTemp.ToggleLazer(false);
                StartCoroutine( pause(true) );
            }
            else
            {
                Vector3 turn = new Vector3( 0, -100 * Time.deltaTime, 0 );
                cameras[0].gameObject.transform.Rotate( turn );
                laserTemp = lasers[0].GetComponent<laser>();
                laserTemp.ToggleLazer(true);
            }
        }
     
	}

    IEnumerator pause(bool pause)
    {
        yield return new WaitForSeconds( 5 );
        isTurn = pause;
    }

}
