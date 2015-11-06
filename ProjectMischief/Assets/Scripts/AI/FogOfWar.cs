using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour
{
    RaycastHit hit;
    float radius = 1.0f;
     
    void Update ()
    {      
  
        Ray ray = new Ray(transform.position, transform.up);
        
        if( Physics.Raycast( ray, out hit, 500 ))
        {
            MeshRenderer filter = hit.collider.gameObject.GetComponent<MeshRenderer>();
            Mesh mesh = hit.collider.gameObject.GetComponent<MeshFilter>().mesh;
            Vector3 relativePoint;
            Color Org = filter.material.color; 
    
            if( hit.collider.gameObject.transform.tag == "Fow50" )
            { 
                    relativePoint = filter.transform.InverseTransformPoint(hit.point);
                    HalfMesh(mesh, relativePoint, radius, filter);
            }
        }
    }

    void HalfMesh( Mesh mesh, Vector3 position, float inRadius, MeshRenderer filter )
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        float sqrRadius = inRadius * inRadius;
        Color[] colours = new Color[vertices.Length];



        for(int i = 0;i < vertices.Length; i++)
        {
            //getting the vertices around the local point passed
            float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
            //if the vertex is too far away, dont carry on
            if (sqrMagnitude > sqrRadius)
                continue;
            colours[i].a = 0;

        }
        filter.material.color = new Color( 1f, 1f, 1f, 0f );
        //print( "50");
        //mesh.colors = colours;
    }
}
