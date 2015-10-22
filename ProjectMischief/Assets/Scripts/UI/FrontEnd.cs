using UnityEngine;
using System.Collections;

public class FrontEnd : MonoBehaviour 
{
    public void Level1()
    {
        Application.LoadLevel( "Level02" );
    }

    public void Exit()
    {
        Application.Quit();
    }
	
}
