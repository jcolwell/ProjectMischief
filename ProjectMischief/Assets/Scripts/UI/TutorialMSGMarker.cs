using UnityEngine;
using System.Collections;

public class TutorialMSGMarker : MonoBehaviour 
{
    [MultilineAttribute]
    public string message = "";
    public string playerTag = "Player";

    public bool giveToolsToPlayer = false;
    public int smokeBombsToGive = 0;
    public int mirrosToGive = 0;
    public int jammersToGive = 0;

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
            if(giveToolsToPlayer)
            {
                PersistentSceneData sceneData = PersistentSceneData.GetPersistentData();
                sceneData.IncreaseNumTools( ToolTypes.eJammer, jammersToGive );
                sceneData.IncreaseNumTools( ToolTypes.eMirror, mirrosToGive );
                sceneData.IncreaseNumTools( ToolTypes.eSmokeBomb, smokeBombsToGive );

                UIManager.instance.UpdateToolCount();
            }

            Destroy(gameObject);
        }
    }
}
