using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour
{
    RaycastHit hit;
    float radius = 1.0f;
     
    void Update ()
    {
        int layerMask = 1 << 13;
       
        RaycastHit[] hits;
        
        Ray ray = new Ray(transform.position, transform.up);
            
        //Physics.Raycast(transform.position, Vector3.up, 500.0f, layerMask);
        //RaycastHit hit = hits[i];

        if( Physics.Raycast( ray, out hit, 500 ))
        {
            MeshFilter filter = hit.collider.gameObject.GetComponent<MeshFilter>();
            Vector3 relativePoint;

            //print( "Filter " + filter );

            if( hit.transform.tag == "Fow50" )
            {          
                if(filter)
                {
                    relativePoint = filter.transform.InverseTransformPoint(hit.point);
                    HalfMesh(filter.mesh, relativePoint, radius);
                    //FogOfWarManager FOG = hit.collider.gameObject.GetComponent<FogOfWarManager>();
                }
            }

            if( hit.transform.tag == "Fow100" )
            {
                print( "100" );
                if(filter)
                {
                    relativePoint = filter.transform.InverseTransformPoint(hit.point);
                    FullMesh(filter.mesh, relativePoint, radius);
                    //FogOfWarManager FOG = hit.collider.gameObject.GetComponent<FogOfWarManager>();
                }
            }
        }
    }
           
    void HalfMesh(Mesh mesh, Vector3 position, float inRadius)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        float sqrRadius = inRadius * inRadius;
        Color[] colours = mesh.colors;
     
        for(int i = 0;i < vertices.Length; i++)
        {
            //getting the vertices around the local point passed
            float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
            //if the vertex is too far away, dont carry on
            if (sqrMagnitude > sqrRadius)
                continue;

            colours[i].a = 0;
        }
        print( "50");
        mesh.colors = colours;
    }

    void FullMesh( Mesh mesh, Vector3 position, float inRadius )
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        float sqrRadius = inRadius * inRadius;
        Color[] colours = mesh.colors;

        for(int i=0;i<vertices.Length;i++)
        {
            //getting the vertices around the local point passed
            float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
            //if the vertex is too far away, dont carry on
            if (sqrMagnitude > sqrRadius)
                continue;
            //set the alpha to 0
            colours[i].a = 0;
        }

        mesh.colors = colours;
    }
}
