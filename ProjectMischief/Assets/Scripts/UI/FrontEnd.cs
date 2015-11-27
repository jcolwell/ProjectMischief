using UnityEngine;
using System.Collections;

public class FrontEnd : MonoBehaviour 
{
    public void LoadLevel(string level)
    {
        Application.LoadLevel( level );
    }

    public void LoadLevel( int level )
    {
        Application.LoadLevel( level );
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadShop()
    {
        Application.LoadLevelAdditive( "UIStore" );
    }
	
}
