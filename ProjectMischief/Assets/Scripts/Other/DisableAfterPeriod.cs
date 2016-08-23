using UnityEngine;
using System.Collections;

public class DisableAfterPeriod : MonoBehaviour
{
    public float duration = 3.0f;
    public bool useUnscaledDeltaTime = true;
    float timeElapsed = 0.0f;
	
	// Update is called once per frame
	void Update ()
    {
        float delta = useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;

        timeElapsed += delta;
        if(timeElapsed >= duration)
        {
            timeElapsed = 0.0f;
            gameObject.SetActive(false);
        }
	}
}
