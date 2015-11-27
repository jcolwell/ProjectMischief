using UnityEngine;
using System.Collections;

public class HazardsManager : MonoBehaviour 
{
    public GameObject[] cameras;
    public GameObject[] lasers;
    bool isTurn = true;
    int pauseTime = 5;

	void Update () 
    {
        laser laserTemp;

        for( int i = 0; i < cameras.Length; ++i )
        {
            if( isTurn )
            {
                if( cameras[i].gameObject.transform.rotation.eulerAngles.y >= 240 )
                {
                    StartCoroutine( pause( false ) );
                }

                else
                {
                    Vector3 turn = new Vector3( 0, 100 * Time.deltaTime, 0 );
                    cameras[i].gameObject.transform.Rotate( turn );
                }
            }

            else
            {
                if( cameras[i].gameObject.transform.rotation.eulerAngles.y <= 90 )
                {
                    StartCoroutine( pause( true ) );
                }

                else
                {
                    Vector3 turn = new Vector3( 0, -100 * Time.deltaTime, 0 );
                    cameras[i].gameObject.transform.Rotate( turn );
                }
            }
        }

        for( int i = 0; i < lasers.Length; ++i )
        {
            if( isTurn )
            {
                if( cameras[0].gameObject.transform.rotation.eulerAngles.y >= 240 )
                {
                    laserTemp = lasers[i].GetComponent<laser>();
                    laserTemp.ToggleLazer( false );
                    StartCoroutine( pause( false ) );
                }

                else
                {
                    laserTemp = lasers[i].GetComponent<laser>();
                    laserTemp.ToggleLazer( true );
                }
            }

            else
            {
                if( cameras[0].gameObject.transform.rotation.eulerAngles.y <= 90 )
                {
                    laserTemp = lasers[i].GetComponent<laser>();
                    laserTemp.ToggleLazer( false );
                    StartCoroutine( pause( true ) );
                }
                else
                {
                    laserTemp = lasers[i].GetComponent<laser>();
                    laserTemp.ToggleLazer( true );
                }
            }
        }
	}

    IEnumerator pause(bool pause)
    {
        yield return new WaitForSeconds( pauseTime );
        isTurn = pause;
    }
}
