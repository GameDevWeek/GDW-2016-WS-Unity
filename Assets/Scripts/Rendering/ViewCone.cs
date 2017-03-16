using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class ViewCone : MonoBehaviour {

    [HideInInspector] public float viewRadius = 10.0f;

    [SerializeField]
    private LayerMask viewBlockingLayers;
    [SerializeField]
    private int nrOfRaycasts = 90;
    [SerializeField]
    private float fieldOfView = 90;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private Vector3[] vertices;
    private Vector3[] normals;

    private StatePatternEnemy enemy;

    private void Start()
    {
        vertices = new Vector3[nrOfRaycasts + 1];
        normals = new Vector3[nrOfRaycasts + 1];
        mesh = new Mesh();
        mesh.name = transform.parent.name + " ViewConeMesh";
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        InvokeRepeating("UpdateViewCone", 0, 0.3f);
        enemy = transform.parent.GetComponent<StatePatternEnemy>();
    }

    private void UpdateViewCone()
    {
        if(!meshRenderer.enabled)
        {
            return;
        }
        viewRadius = enemy.sightRange;
        //Collider[] objectsInRange = Physics.OverlapSphere(transform.position, viewRadius, viewBlockingLayers);
        
        vertices[0] = Vector3.zero;
        normals[0] = Vector3.up;
        RaycastHit hit;
        for(int i = 0; i < nrOfRaycasts; ++i)
        {
            float angle = ((90 - (fieldOfView * 0.5f) + ((float)i / nrOfRaycasts) * fieldOfView)) % 360;
            Vector2 localDirection = MathUtility.DegreeToVector2(angle, 1);
            Vector3 globalDirection = transform.TransformDirection(new Vector3(localDirection.x, 0, localDirection.y));

            if (Physics.Raycast(transform.position, globalDirection, out hit, viewRadius, viewBlockingLayers))
            {
                vertices[i + 1] = transform.InverseTransformPoint(hit.point);

                //Debug.DrawLine(transform.position, hit.point, Color.magenta);
            }
            else
            {
                vertices[i + 1] = new Vector3(localDirection.x * viewRadius, 0, localDirection.y * viewRadius);
                //Debug.DrawLine(transform.position, transform.position + new Vector3(direction2D.x * viewRadius, 0, direction2D.y * viewRadius), Color.magenta);
            }
            normals[i + 1] = Vector3.up;
        }
        var triangles = new int[(nrOfRaycasts - 2) * 3 + 3];
        for (int i = 0, j = 0; i < triangles.Length; i += 3, ++j)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 2;
            triangles[i + 2] = j + 1;
        }
        triangles[triangles.Length - 3] = 0;
        triangles[triangles.Length - 2] = 1;
        triangles[triangles.Length - 1] = triangles.Length / 3 - 1;
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        meshFilter.mesh = mesh;
    }
}
