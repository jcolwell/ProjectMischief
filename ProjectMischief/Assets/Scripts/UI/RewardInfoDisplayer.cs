using UnityEngine;
using System.Collections;

public class RewardInfoDisplayer : MonoBehaviour
{
    public GameObject[] icons;
    public GameObject[] textObjects;
    public float duration= 1.0f;
    float timeElpased = 0.0f;
    int curIndex = 0;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        timeElpased += Time.unscaledDeltaTime;

        if(timeElpased >= duration && curIndex < icons.Length && curIndex <textObjects.Length)
        {
            icons[curIndex].SetActive(true);
            textObjects[curIndex].SetActive(true);
            ++curIndex;
            timeElpased = 0.0f;
        }
	}
}
