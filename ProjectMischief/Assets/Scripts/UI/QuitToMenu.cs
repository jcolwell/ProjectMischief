using UnityEngine;
using System.Collections;

public class QuitToMenu : MonoBehaviour 
{
    public void ToMenu()
    {
        Application.LoadLevel( "FrontEnd" );
    }
}
