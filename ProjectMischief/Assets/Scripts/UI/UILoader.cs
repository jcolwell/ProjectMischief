using UnityEngine;
using System.Collections;

public class UILoader : MonoBehaviour {

    static public bool gameIsPaused = false;
    void Start()
    {
        gameIsPaused = false;
        Application.LoadLevelAdditive( "UILevel" );
    }
}
