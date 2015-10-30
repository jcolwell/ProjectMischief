using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VersionText : MonoBehaviour
{
    Text verNum;

    // Use this for initialization
    void Start ()
    {
        verNum = GetComponent<Text> ();
        verNum.text = "Version: " + Application.version;
    }
}
