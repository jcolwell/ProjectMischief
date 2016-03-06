using UnityEngine;
using System.Collections;

public class EndOfLevel : MonoBehaviour 
{
    public string playerTag = "Player";
    public string nextLevel;

    public bool nextLevelRandom = false;
    public int minLevel = 2;
    public int maxLevel = 4;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(playerTag))
        {
            if(nextLevelRandom)
            {
                int nextLevelIndex = Random.Range(minLevel, maxLevel);
                UIManager.instance.EndLevel(nextLevelIndex);
                return;
            }

            UIManager.instance.EndLevel(nextLevel);
        }
    }
	
}
