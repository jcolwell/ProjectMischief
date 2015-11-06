//======================================================
// File: VisionCone.cs
// Discription:    
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//======================================================

//======================================================
// Class
//======================================================
[RequireComponent ( typeof ( MeshFilter ) )]
[RequireComponent ( typeof ( MeshRenderer ) )]
public class VisionCone : MonoBehaviour
{
    //==================================================
    // Public
    //==================================================
    public float voidRadius = 5.0f;
    public float fovAngle = 90;
    public float fovMaxDist = 15;
    public float quality = 4;
    public LayerMask cullingMask;
    public List<RaycastHit> hits = new List<RaycastHit> ();

    public List<Material> materials;
    //==================================================

    //==================================================
    // Private
    //==================================================
    Mesh mesh;
    MeshRenderer meshRenderer;
    //==================================================

    //==================================================

    void Start ()
    {
        mesh = GetComponent<MeshFilter> ().mesh;
        meshRenderer = GetComponent<MeshRenderer> ();
        meshRenderer.material = materials[0];
    }

    //==================================================

    void Update ()
    {
        CastRays ();
        UpdateMesh ();
    }
    void LateUpdate()
    {
    }

    //==================================================

    void CastRays ()
    {
        int numRays = ( int ) ( fovAngle * quality + 0.5f );
        float currentAngle = fovAngle / -2;

        hits.Clear ();
        for ( int i = 0; i < numRays; ++i )
        {
            Vector3 direction = Quaternion.AngleAxis ( currentAngle, transform.up ) * transform.forward;

            //Vector3 position = direction * voidRadius + transform.position;
            Vector3 position = transform.position;

            RaycastHit hit = new RaycastHit ();
            if (Physics.Raycast ( position, direction, out hit, fovMaxDist, cullingMask ) == false)
            {
                hit.point = position + ( direction * fovMaxDist );
            }

            hits.Add ( hit );
            currentAngle += 1f / quality;
        }
    }

    //==================================================

    void UpdateMesh ()
    {
        Vector3[] newVertices = new Vector3[hits.Count + 1];

        if ( hits == null || hits.Count == 0 ) return;

        //Create vertex index list to reference when building triangles
        int[] newTriangles = new int[( hits.Count - 1 ) * 3];
        if ( mesh.vertices.Length != hits.Count + 1 )
        {
            mesh.Clear ();
            for ( int i = 0, v = 1; i < newTriangles.Length; i = i + 3, ++v )
            {
                newTriangles[i] = 0;
                newTriangles[i + 1] = v;
                newTriangles[i + 2] = v + 1;
            }
        }

        //Construct the vertices for the triangles
        newVertices[0] = Vector3.zero;
        for ( int i = 1; i <= hits.Count; i++ )
        {
            newVertices[i] = transform.InverseTransformPoint ( hits[i - 1].point );
        }

        //Rebuild the UV map
        Vector2[] newUV = new Vector2[newVertices.Length];
        for ( int i = 0; i < newUV.Length; ++i ) 
        {
            newUV[i] = new Vector2 ( newVertices[i].x, newVertices[i].z );
        }

        //Put the Mesh back together
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.uv = newUV;

        mesh.RecalculateNormals ();
        mesh.RecalculateBounds ();
    }

    //==================================================

    //void UpdateMeshMaterial ()
    //{
    //    for ( int i = 0; i < materials.Count; ++i )
    //    {
    //        if ( i == ( int ) status && meshRenderer.material != materials[i] )
    //        {
    //            meshRenderer.material = materials[i];
    //        }
    //    }
    //}

}
//======================================================