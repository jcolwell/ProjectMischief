using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Image))]
public class CopyImageComponent : MonoBehaviour 
{
    public Image imageToCopy;
    public Image gameObjectImage;

	public void ChangeImageComponent()
    {
        gameObjectImage.sprite = imageToCopy.sprite;
    }

}
