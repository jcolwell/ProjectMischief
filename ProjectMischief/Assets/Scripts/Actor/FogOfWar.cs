using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour
{ 
    static public FogOfWar instance;
    RaycastHit hit;
    public float radius = 0.04f;


    void Update ()
    {
        instance = this;
        Vector3 Orgin = transform.position;
        Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f) ;
  
        Ray ray = new Ray(Orgin, Up);
        
        if( Physics.Raycast( ray, out hit, 500 ))
        {
            MeshRenderer filter = hit.collider.gameObject.GetComponent<MeshRenderer>();
            Mesh mesh = hit.collider.gameObject.GetComponent<MeshFilter>().mesh;
            Vector3 relativePoint;
    
            if( hit.collider.gameObject.transform.tag == "Fow50" )
            { 
                    relativePoint = filter.transform.InverseTransformPoint(hit.point);
                    HalfMesh( ref mesh, relativePoint, radius, ref filter );
            }
        }
    }

    void HalfMesh( ref Mesh mesh, Vector3 position, float inRadius, ref MeshRenderer filter )
    {
        Vector3[] vertices = mesh.vertices;
        float sqrRadius = inRadius * inRadius;
        int vCount = mesh.vertexCount ;

        Color[] colours = new Color[vCount];

        for (int i = 0 ; i < vCount ; ++i)
        {
            float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
            colours[i].a = Mathf.Min(sqrMagnitude/sqrRadius, 1.0f);
        }

        mesh.colors = colours;
    }

    public void ChangeRadius(float r)
    {
        radius = r;
    }
}
