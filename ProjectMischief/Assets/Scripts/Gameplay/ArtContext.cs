﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArtContext : MonoBehaviour 
{
    //[HideInInspector]
    public string[] paintingchoices = new string[(int)ArtFields.eMax];
    //[HideInInspector]
    public string[] yearChoices = new string[(int)ArtFields.eMax];
    //[HideInInspector]
    public string[] artistChoices = new string[(int)ArtFields.eMax];
    //[HideInInspector]
    public string[] correctChoices = new string[(int)ArtFields.eMax];
    //[HideInInspector]
    public string[] currentChoices = new string[(int)ArtFields.eMax];
    //[HideInInspector]
    public Sprite art;
    //[HideInInspector]
    public bool isForegry;
    //[HideInInspector]
    public int artID = -1;
    //[HideInInspector]
    public string description;
    public string artFileName;
}

[Serializable]
public class ArtFileInfo
{
    public int id;
    public string year;
    public string artist;
    public string name;
    public string artFileName;
    public string description;
}