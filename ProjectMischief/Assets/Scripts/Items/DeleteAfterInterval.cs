using UnityEngine;
using System.Collections;

public class DeleteAfterInterval : MonoBehaviour
{
    public float lifeTime = 1.0f;
    float timeElapsed = 0;

    void Start()
    {
        timeElapsed = 0.0f;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if( timeElapsed >= lifeTime )
        {
            Destroy( this.gameObject );
        }
    }
}
