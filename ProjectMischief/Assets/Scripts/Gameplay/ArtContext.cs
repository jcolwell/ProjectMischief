using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArtContext : MonoBehaviour 
{
    //[HideInInspector]
    public string[] paintingchoices = new string[3];
    //[HideInInspector]
    public string[] yearChoices = new string[3];
    //[HideInInspector]
    public string[] artistChoices = new string[3];
    //[HideInInspector]
    public string[] correctChoices = new string[3];
    //[HideInInspector]
    public string[] currentChoices = new string[3];
    //[HideInInspector]
    public Sprite art;
    //[HideInInspector]
    public bool isForegry;
    //[HideInInspector]
    public int artID = -1;
    //[HideInInspector]
    public string description;
}

[Serializable]
public class ArtFileInfo
{
    public string year;
    public string artist;
    public string name;
    public string artFileName;
    public string description;
}