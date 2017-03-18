using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class ViewCone : MonoBehaviour {

	public float MainViewRadius { get{return mainViewRadius;} set{mainViewRadius = value;} }
	public float FullViewRadius { get{return fullViewRadius;} set{fullViewRadius=value;} }
    public float ConeAlpha { get; set; }

	[SerializeField]
	private float mainViewRadius = 0;
	[SerializeField]
	private float fullViewRadius = 0;

    [SerializeField]
    private LayerMask viewBlockingLayers;
    [SerializeField]
    private int nrOfRaycasts = 90;
    [SerializeField]
    private float fieldOfView = 90;
    [SerializeField]
    private float collisionOffset = 0.05f;
    [SerializeField]
    private Color colorAlarmed = Color.red;
    [SerializeField]
    Color colorDefault = Color.grey;
    [SerializeField]
    Color colorOuterAlarmed = new Color(1, 0, 0, 0.5f);
    [SerializeField]
    Color colorOuterDefault = new Color(0.5f, 0.5f, 0.5f, 0.5f);


    private LineRenderer2D lineRenderer;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh coneMesh;

    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uv;
    Vector3[] outlinePoints;


    private void Start()
    {
        ConeAlpha = 1f;
        vertices = new Vector3[nrOfRaycasts * 2 + 1];
        normals = new Vector3[nrOfRaycasts * 2 + 1];
        uv = new Vector2[nrOfRaycasts * 2 + 1];
        outlinePoints = new Vector3[2 + nrOfRaycasts];
        coneMesh = new Mesh();
        coneMesh.name = transform.parent.name + " ViewConeMainMesh";
        lineRenderer = GetComponentInChildren<LineRenderer2D>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        coneMesh.subMeshCount = 2;
        Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;
        setAlarmed(false, 0);
    }

    private void Update()
    {
        if(!meshRenderer.enabled)
        {
            return;
        }
        
        vertices[0] = Vector3.zero;
        normals[0] = Vector3.up;
        uv[0] = Vector3.zero;
        
        outlinePoints[0] = Vector3.zero;
        outlinePoints[outlinePoints.Length - 1] = Vector3.zero;

        RaycastHit hit;

        for (int i = 0; i < nrOfRaycasts; ++i)
        {
			float angle = ((90 - (fieldOfView * 0.5f) + ((float)i / (nrOfRaycasts-1)) * fieldOfView)) % 360;
            Vector2 localDirection = MathUtility.DegreeToVector2(angle, 1);
            Vector3 globalDirection = transform.TransformDirection(new Vector3(localDirection.x, 0, localDirection.y));

            if (Physics.Raycast(transform.position, globalDirection, out hit, FullViewRadius, viewBlockingLayers))
            {
                if((hit.point - transform.position).magnitude > MainViewRadius)
                {
                    vertices[i + 1] = new Vector3(localDirection.x * (MainViewRadius - collisionOffset), 0, localDirection.y * (MainViewRadius - collisionOffset));
                }
                else
                {
                    vertices[i + 1] = transform.InverseTransformPoint(hit.point - globalDirection * collisionOffset);
                }
                vertices[nrOfRaycasts + i + 1] = transform.InverseTransformPoint(hit.point - globalDirection * collisionOffset);
            }
            else
            {
                vertices[i + 1] = new Vector3(localDirection.x * (MainViewRadius - collisionOffset), 0, localDirection.y * (MainViewRadius - collisionOffset));
                vertices[nrOfRaycasts + i + 1] = new Vector3(localDirection.x * (FullViewRadius - collisionOffset), 0, localDirection.y * (FullViewRadius - collisionOffset));
            }
            outlinePoints[i + 1] = vertices[nrOfRaycasts + i + 1];

            normals[i + 1] = Vector3.up;
            normals[nrOfRaycasts + i + 1] = Vector3.up;
            uv[i + 1] = new Vector2((float)i / (nrOfRaycasts - 1), 0.5f);
            uv[nrOfRaycasts + i + 1] = new Vector2((float)i / (nrOfRaycasts - 1), 1.0f);
        }
        var trianglesMainCone = new int[(nrOfRaycasts - 2) * 3 + 3];
        for (int i = 0, j = 0; i < trianglesMainCone.Length; i += 3, ++j)
        {
            trianglesMainCone[i] = 0;
            trianglesMainCone[i + 1] = j + 2;
            trianglesMainCone[i + 2] = j + 1;
        }
        var trianglesOuterCone = new int[((nrOfRaycasts * 2) - 2) * 3];
        for (int i = 0, j = 0; i < trianglesOuterCone.Length - 1; i += 6, ++j)
        {
            trianglesOuterCone[i] = j + 1;
            trianglesOuterCone[i + 1] = j + 2 + nrOfRaycasts;
            trianglesOuterCone[i + 2] = j + 1 + nrOfRaycasts;
            trianglesOuterCone[i + 3] = j + 1;
            trianglesOuterCone[i + 4] = j + 2;
            trianglesOuterCone[i + 5] = j + 2 + nrOfRaycasts;
        }

        coneMesh.vertices = vertices;
        coneMesh.normals = normals;
        coneMesh.SetTriangles(trianglesMainCone, 0);
        coneMesh.SetTriangles(trianglesOuterCone, 1);

        coneMesh.uv = uv;
        meshFilter.mesh = coneMesh;

		if (lineRenderer != null) {
        	lineRenderer.Points = outlinePoints;
		}
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isAlarmed"></param>
    /// <param name="t">Value between 0 and 1. 1 Means fully alarmed.</param>
    public void setAlarmed(bool isAlarmed, float t)
    {
        Color colorMain;
        Color colorOuter;
        if (isAlarmed)
        {
            colorMain = colorAlarmed;
            colorOuter = colorOuterAlarmed * (1 - t) + colorAlarmed * t;
        }
        else
        {
            colorMain = colorDefault;
            colorOuter = colorOuterDefault;
        }
        colorMain.a *= ConeAlpha;
        colorOuter.a *= ConeAlpha;
        meshRenderer.materials[0].color = colorMain;
        meshRenderer.materials[1].color = colorOuter;
    }
}
