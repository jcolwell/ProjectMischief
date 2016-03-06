using UnityEngine;
using System.Collections;

public class TutorialMSGMarker : MonoBehaviour 
{
    [MultilineAttribute]
    public string message = "";
    public string playerTag = "Player";

    void Start()
    {
        MeshRenderer meshRender = GetComponent<MeshRenderer>();
        if(meshRender != null)
        {
            Destroy(meshRender);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            UIManager.instance.PopUpTutorialMSG(message);
            Destroy(gameObject);
        }
    }
}
