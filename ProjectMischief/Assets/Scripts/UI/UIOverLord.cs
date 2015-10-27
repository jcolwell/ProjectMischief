using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIOverLord : MonoBehaviour {

    static public bool gameIsPaused = false;   

    void Start()
    {
        gameIsPaused = false;
        Application.LoadLevelAdditive( "UILevel" );
    }
}
