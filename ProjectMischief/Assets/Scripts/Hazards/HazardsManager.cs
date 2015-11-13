using UnityEngine;
using System.Collections;

public class HazardsManager : MonoBehaviour 
{
    public GameObject[] cameras;
    public GameObject[] lasers;
    bool isTurn = true;

	void Update () 
    {
        if(isTurn)
        {

            if(cameras[0].gameObject.transform.rotation.eulerAngles.y >= 240 )
            {
                lasers[0].SetActive( false );
                StartCoroutine(pause(false));
            }
            else
            {
                Vector3 turn = new Vector3( 0, 100 * Time.deltaTime, 0 );
                cameras[0].gameObject.transform.Rotate( turn );
                lasers[0].SetActive( true );

            }
        }
        else
        {

            if( cameras[0].gameObject.transform.rotation.eulerAngles.y <= 90 )
            {
                lasers[0].SetActive( false );
                StartCoroutine( pause(true) );
            }
            else
            {
                Vector3 turn = new Vector3( 0, -100 * Time.deltaTime, 0 );
                cameras[0].gameObject.transform.Rotate( turn );
                lasers[0].SetActive( true );
            }
        }
     
	}

    IEnumerator pause(bool pause)
    {
        yield return new WaitForSeconds( 5 );
        isTurn = pause;
    }

}
