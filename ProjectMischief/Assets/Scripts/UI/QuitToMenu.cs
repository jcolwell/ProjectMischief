using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class QuitToMenu : MonoBehaviour 
{
    public void ToMenu()
    {
        SceneManager.LoadScene("FrontEnd", LoadSceneMode.Single);
    }
}
