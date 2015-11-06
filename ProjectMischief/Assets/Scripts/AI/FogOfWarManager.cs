using UnityEngine;
using System.Collections;

public class FogOfWarManager : MonoBehaviour 
{

    void Start()
    {
        //getting the mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
       
        //have to filter out whatever game object its attatched too, to make sure the colours have
        //been set correctly before they get altered later, stops invalid array errors where colours  return null
        if(gameObject.name == "Fow50")
        {
            for(int i = 0; i < vertices.Length; i++)
            {
                colors[i] = Color.grey;
                //colors[i].a = 1;
            }
            mesh.colors32 = colors;
        }
        if(gameObject.name == "Fow100")
        {
            for(int i = 0; i < vertices.Length; i++)
            {
                colors[i] = Color.black;
                colors[i].a = 1;
            }
            mesh.colors32 = colors;      
        }
        //only the 50% fog needs to be refreshed
        if(gameObject.name == "Fow50")
        {
            RefreshFog();
        }
    }
    
    IEnumerator RefreshFog()
    {
        while(true)
        {
            Mesh mesh= GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Color32[] colors = mesh.colors32;
           
            for(int i = 0; i < colors.Length; i++)
            {
                //if the alpha of the mesh isnt 1, add 0.25 alpha on each time
                //this should cause less of a flicker than a full reset to 1 alpha causes
                //giving the fog a gradual regrowth feel
                if(colors[i].a <= 1)
                {
                    //getting the current colours alpha in a var
                    byte f = colors[i].a;
                    //incrementing the alpha by 1/4
                    f += (byte)0.1;
                    //f shouldnt go above 1, but incase this sets the alpha correctly
                    if(f > 1)
                    {
                        f = (byte)1.0;
                    }

                    //colors[i].a = f;
                   
                }
     
            }
            mesh.colors32 = colors;
           
             yield return new WaitForSeconds(1);
        }
    }
}
