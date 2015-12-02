//======================================================
// File:            VisionCone.cs
// Discription:     This purpose of this script is to represent
//                  and act as the eyes of the agent
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

//======================================================

//======================================================
// Class
//======================================================
[RequireComponent( typeof( MeshFilter ) )]
[RequireComponent( typeof( MeshRenderer ) )]
public class VisionCone:MonoBehaviour
{
    //==================================================
    // Public
    //==================================================
    public float voidRadius = 5.0f;
    public float fovAngle = 90;
    public float fovMaxDist = 15;
    public float quality = 4;
    public LayerMask cullingMask;
    public List<Vector3> positionList = new List<Vector3>();

    //public List<RaycastHit> hits = new List<RaycastHit> ();
    public List<Material> materials;
    //==================================================

    public enum Status
    {
        Chasing,
        Idle
    }


    //==================================================
    // Private
    //==================================================
    Mesh mesh;
    MeshRenderer meshRenderer;
    Status status;

    bool canSeePlayer;
    Vector3 playerPosition;
    //==================================================

    //==================================================

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        status = Status.Idle;

        canSeePlayer = false;
        playerPosition = new Vector3();
    }

    //==================================================

    void Update()
    {
        CastRays();
        UpdateMesh();
        UpdateMeshMaterial();
    }

    //==================================================

    void CastRays()
    {
        canSeePlayer = false;
        positionList.Clear();

        System.IO.StreamWriter log = new System.IO.StreamWriter( "log.txt" );

        int numRays = ( int )( fovAngle * quality + 0.5f );
        float currentAngle = fovAngle / -2;
        for( int i = 0; i < numRays; ++i, currentAngle += (1f / quality) )
        {
            Vector3 direction = Quaternion.AngleAxis( currentAngle, transform.up ) * transform.forward;

            Vector3 startPos = direction * voidRadius + transform.position;
            startPos = transform.InverseTransformPoint( startPos );

            RaycastHit hit = new RaycastHit();
            if( Physics.Raycast( transform.position, direction, out hit, fovMaxDist, cullingMask ) == false )
            {
                
                hit.point = transform.position + ( direction * fovMaxDist );
            }
            else if( hit.collider.CompareTag( "Player" ) )
            {
               // Debug.Log( "PLAYER SEEN! SENDING MESSAGE" );
                canSeePlayer = true;
                playerPosition = hit.point;
            }

            Vector3 endPos = transform.InverseTransformPoint( hit.point );

            log.WriteLine( startPos + "  " + endPos );

            positionList.Add( startPos );
            positionList.Add( endPos );
        }

        log.Close();
        ReportVision();
    }

    //==================================================

    private void ReportVision()
    {
        if (canSeePlayer)
        {
            status = Status.Chasing;
            SendMessageUpwards("PlayerVisible", playerPosition);
        }
        else
        {
            status = Status.Idle;
            SendMessageUpwards("PlayerNotVisible");
        }
    }

    //==================================================

    void UpdateMesh()
    {

        if( positionList == null || positionList.Count == 0 )
            return;

        //Create vertex index list to reference when building triangles
        int size = ( int )( positionList.Count );
        int[] newTriangles = new int[ size ];
        mesh.Clear();

        //Build new Triangles
        for( int i = 0; i < positionList.Count; i += 6 )
        {
            newTriangles[ i ] = i;
            newTriangles[ i + 1 ] = i + 1 ;
            newTriangles[ i + 2 ] = i + 2;

            newTriangles[ i + 3] = i + 2;
            newTriangles[ i + 4 ] = i + 1;
            newTriangles[ i + 5 ] = i + 3;
        }

        //Rebuild the UV map
        List<Vector2> newUV = new List<Vector2>();
        for( int i = 0; i < positionList.Count; ++i )
        {
            Vector2 v = new Vector2( positionList[ i ].x, positionList[ i ].z );
            newUV.Add( v );
        }

        //Put the Mesh back together
        mesh.vertices = positionList.ToArray();
        mesh.triangles = newTriangles;
        mesh.uv = newUV.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    //==================================================

    void UpdateMeshMaterial ()
    {
        for ( int i = 0; i < materials.Count; ++i )
        {
            if ( i == ( int ) status && meshRenderer.material != materials[i] )
            {
                meshRenderer.material = materials[i];
            }
        }
    }

}
//======================================================