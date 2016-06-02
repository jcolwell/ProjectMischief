using UnityEngine;
using System.Collections;

public class EndOfLevel : MonoBehaviour 
{
    public static bool allPaintingsComplete = false;

    public string playerTag = "Player";
    public string nextLevel;

    public bool loadLevelSelectUI = false;

    public GameObject model;
    public GameObject mapIcon;



    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(playerTag))
        {
            if (loadLevelSelectUI)
            {
                UIManager.instance.EndLevel(-1);
                return;
            }

            UIManager.instance.EndLevel(nextLevel);
        }
    }
	
    void Start()
    {
        allPaintingsComplete = false;
        model.SetActive(false);
        mapIcon.SetActive(false);

        UIManager.instance.SetExitWorldPos(transform.position);
    }

    void Update()
    {
        if(allPaintingsComplete)
        {
            model.SetActive(true);
            mapIcon.SetActive(true);
        }
    }
}
