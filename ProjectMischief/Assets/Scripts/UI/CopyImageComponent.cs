using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Image))]
public class CopyImageComponent : MonoBehaviour 
{
    public Image imageToCopy;
    public Image gameObjectImage;
    public string folderForExpandedImages = "PaintingsExpanded/";

	public void ChangeImageComponent()
    {
        gameObjectImage.sprite = Resources.Load<Sprite>( folderForExpandedImages + imageToCopy.sprite.name );
    }

}
