using UnityEngine;
using System.Collections;

public class LevelSelectUIControl : UIControl
{
    public uint firstLevel;
    public uint numLevels;

    uint curLevel = 0;

    LevelSelectUIControl()
        : base(UITypes.levelSelect)
    { }


	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
