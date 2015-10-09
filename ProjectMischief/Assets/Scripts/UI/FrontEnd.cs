using UnityEngine;
using System.Collections;

public class FrontEnd : MonoBehaviour 
{
    public void Level1()
    {
        Application.LoadLevel( 2 );
    }

    public void Exit()
    {
        Application.Quit();
    }
	
}
