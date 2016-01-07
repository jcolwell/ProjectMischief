using UnityEngine;
using System.Collections;

public class PlayerCheckPoint : MonoBehaviour 
{
    Vector3 checkPoint;

	// Use this for initialization
	void Start () 
	{
        checkPoint = gameObject.transform.position;
	}
	
    public void SetCheckPoint(Vector3 postion)
    {
        checkPoint = postion;
    }

    public void GoToCheckPoint()
    {
        Moving m = GetComponent<Moving>();

        m.ToggleNavMeshAgent();

        gameObject.transform.position = checkPoint;

        m.ToggleNavMeshAgent();

        
        m.setTarget( checkPoint );
    }

}
