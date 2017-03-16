using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class LineRenderer2D : MonoBehaviour {
    [SerializeField]
    private Vector3[] points;
    [SerializeField]
    private float lineWidth = 0.1f;
    [SerializeField]
    private bool connectLines = true;
    [SerializeField]
    private bool renderPoints = false;
    [SerializeField]
    private float circleRadius = 1f;

    private float halfLineWidth;

    private Mesh mesh;

    private List<Vector3> vertices;
    private List<Vector2> uv;
    private List<int> triangles;

    public Vector3[] Points { get { return points; } set { points = value; Triangulate(); } }

    // Use this for initialization
    void Start ()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "2DLine";
        vertices = new List<Vector3>();
        uv = new List<Vector2>();
        triangles = new List<int>();
    }

    /// <summary>
    /// Updates the mesh.
    /// </summary>
    private void Triangulate()
    {
        vertices.Clear();
        uv.Clear();
        triangles.Clear();
        mesh.Clear();

        halfLineWidth = lineWidth * 0.5f;

        if(renderPoints)
        {
            TriangulateLinesBetweenPoints();
        }
        else if(connectLines)
        {
            TriangulateLinesConnected();
        }
        else
        {
            TriangulateLinesSimple();
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        Vector3[] normals = new Vector3[vertices.Count];
        for(int i = 0; i < 0; ++i)
        {
            normals[i] = Vector3.up;
        }
        mesh.normals = normals;
    }

    /// <summary>
    /// Creates a line between all points.
    /// Does not go through the point itself but only touches it's outline.
    /// </summary>
    private void TriangulateLinesBetweenPoints()
    {
        if (points != null && points.Length > 1)
        {
            if(renderPoints)
            {
                TriangulatePoint(points[0]);
            }
            
            for (int i = 0; i < points.Length - 1; ++i)
            {
                Vector3 lineDirection = points[i + 1] - points[i];
                Vector3 startPoint;
                Vector3 endPoint;
                if (lineDirection.x < 0 && lineDirection.z < 0)
                {
                    startPoint = MathUtility.LineIntersectionPoint(points[i], points[i + 1], points[i] + Vector3.right * circleRadius, points[i] + Vector3.up * circleRadius).Value;
                    endPoint = MathUtility.LineIntersectionPoint(points[i], points[i + 1], points[i+1] + Vector3.left * circleRadius, points[i+1] + Vector3.down * circleRadius).Value;
                }
                else if (lineDirection.x < 0 && lineDirection.z >= 0)
                {
                    startPoint = MathUtility.LineIntersectionPoint(points[i], points[i + 1], points[i] + Vector3.right * circleRadius, points[i] + Vector3.down * circleRadius).Value;
                    endPoint = MathUtility.LineIntersectionPoint(points[i], points[i + 1], points[i + 1] + Vector3.left * circleRadius, points[i + 1] + Vector3.up * circleRadius).Value;
                }
                else if (lineDirection.x >= 0 && lineDirection.z < 0)
                {
                    startPoint = MathUtility.LineIntersectionPoint(points[i], points[i + 1], points[i] + Vector3.left * circleRadius, points[i] + Vector3.up * circleRadius).Value;
                    endPoint = MathUtility.LineIntersectionPoint(points[i], points[i + 1], points[i + 1] + Vector3.right * circleRadius, points[i + 1] + Vector3.down * circleRadius).Value;
                }
                else
                {
                    startPoint = MathUtility.LineIntersectionPoint(points[i], points[i + 1], points[i] + Vector3.left * circleRadius, points[i] + Vector3.down * circleRadius).Value;
                    endPoint = MathUtility.LineIntersectionPoint(points[i], points[i + 1], points[i + 1] + Vector3.right * circleRadius, points[i + 1] + Vector3.up * circleRadius).Value;
                }

                TriangulateLine(startPoint, endPoint);
                if (renderPoints)
                {
                    TriangulatePoint(points[i + 1]);
                }
            }
        }
        else if (points != null && points.Length == 1)
        {
            if (renderPoints)
            {
                TriangulatePoint(points[0]);
            }
        }
    }

    /// <summary>
    /// Creates a simple line between all points. The ends of two successive lines will have a different angle.
    /// </summary>
    private void TriangulateLinesSimple()
    {
        if (points != null && points.Length > 1)
        {
            if (renderPoints)
            {
                TriangulatePoint(points[0]);
            }
            for (int i = 0; i < points.Length - 1; ++i)
            {
                TriangulateLine(points[i], points[i + 1]);
                if (renderPoints)
                {
                    TriangulatePoint(points[i + 1]);
                }
            }
        }
        else if (points != null && points.Length == 1)
        {
            if (renderPoints)
            {
                TriangulatePoint(points[0]);
            }
        }
    }

    /// <summary>
    /// Creates a line between all points. The ends of two successive lines will have the same angle.
    /// </summary>
    private void TriangulateLinesConnected()
    {
        if(points != null && points.Length > 2)
        {
            if (renderPoints)
            {
                TriangulatePoint(points[0]);
            }
            TriangulateLine(points[0], points[0], points[1], points[2]);
            if (renderPoints)
            {
                TriangulatePoint(points[1]);
            }
            for (int i = 1; i < points.Length - 2; ++i)
            {
                TriangulateLine(points[i - 1], points[i], points[i + 1], points[i + 2]);
                if (renderPoints)
                {
                    TriangulatePoint(points[i + 1]);
                }
            }
            TriangulateLine(points[points.Length - 3], points[points.Length - 2], points[points.Length - 1], points[points.Length - 1]);
            if (renderPoints)
            {
                TriangulatePoint(points[points.Length - 1]);
            }
        }
        else if(points != null && points.Length == 2)
        {
            if (renderPoints)
            {
                TriangulatePoint(points[0]);
            }
            TriangulateLine(points[0], points[0], points[1], points[1]);
            if (renderPoints)
            {
                TriangulatePoint(points[1]);
            }
        }
        else if(points != null && points.Length == 1)
        {
            if (renderPoints)
            {
                TriangulatePoint(points[0]);
            }
        }
    }

    /// <summary>
    /// Adds two triangles to create a line between two points.
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    private void TriangulateLine(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 lineDirection = endPoint - startPoint;
        Vector3 lineDirectionRight = (new Vector3(-lineDirection.z, 0, lineDirection.x) / Mathf.Sqrt(lineDirection.x * lineDirection.x + lineDirection.z * lineDirection.z)) * -halfLineWidth;
        Vector3 lineDirectionLeft = (new Vector3(-lineDirection.z, 0, lineDirection.x) / Mathf.Sqrt(lineDirection.x * lineDirection.x + lineDirection.z * lineDirection.z)) * halfLineWidth;

        vertices.Add(startPoint + lineDirectionRight);
        uv.Add(Vector2.right);
        int startRight = vertices.Count - 1;
        vertices.Add(endPoint + lineDirectionRight);
        uv.Add(Vector2.right + Vector2.up);
        int endRight = vertices.Count - 1;
        vertices.Add(endPoint + lineDirectionLeft);
        uv.Add(Vector2.up);
        int endLeft = vertices.Count - 1;
        vertices.Add(startPoint + lineDirectionLeft);
        uv.Add(Vector2.zero);
        int startLeft = vertices.Count - 1;

        AddQuad(startLeft, endLeft, endRight, startRight);
    }

    /// <summary>
    /// Adds two triangles to create a line between two points.
    /// Both ends will be rotated by an angleOffset.
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="angleOffset">Angle in degree.</param>
    private void TriangulateLine(Vector3 startPoint, Vector3 endPoint, float angleOffset)
    {
        Vector3 lineDirection = endPoint - startPoint;
        float rightSinAngle = -Mathf.Sin((angleOffset - 90) * Mathf.Deg2Rad);
        float rightCosAngle = Mathf.Cos((angleOffset - 90) * Mathf.Deg2Rad);
        float leftSinAngle = -Mathf.Sin((angleOffset + 90) * Mathf.Deg2Rad);
        float leftCosAngle = Mathf.Cos((angleOffset + 90) * Mathf.Deg2Rad);

        Vector3 lineDirectionRight = new Vector3(lineDirection.x * rightCosAngle - lineDirection.z * rightSinAngle, 0, lineDirection.z * rightCosAngle + lineDirection.x * rightSinAngle);
        Vector3 lineDirectionLeft = new Vector3(lineDirection.x * leftCosAngle - lineDirection.z * leftSinAngle, 0, lineDirection.z * leftCosAngle + lineDirection.x * leftSinAngle);

        lineDirectionRight = lineDirectionRight / Mathf.Sqrt(lineDirectionRight.x * lineDirectionRight.x + lineDirectionRight.z * lineDirectionRight.z) * halfLineWidth;
        lineDirectionLeft = lineDirectionLeft / Mathf.Sqrt(lineDirectionLeft.x * lineDirectionLeft.x + lineDirectionLeft.z * lineDirectionLeft.z) * halfLineWidth;

        vertices.Add(startPoint + lineDirectionRight);
        uv.Add(Vector2.right);
        int startRight = vertices.Count - 1;
        vertices.Add(endPoint + lineDirectionRight);
        uv.Add(Vector2.right + Vector2.up);
        int endRight = vertices.Count - 1;
        vertices.Add(endPoint + lineDirectionLeft);
        uv.Add(Vector2.up);
        int endLeft = vertices.Count - 1;
        vertices.Add(startPoint + lineDirectionLeft);
        uv.Add(Vector2.zero);
        int startLeft = vertices.Count - 1;

        AddQuad(startRight, endRight, endLeft, startLeft);
    }

    /// <summary>
    /// Adds two triangles to create a line between startPoint and endPoint. Takes previous and next point into account to connect the points in the correct angle
    /// </summary>
    /// <param name="previousPoint"></param>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="nextPoint"></param>
    private void TriangulateLine(Vector3 previousPoint, Vector3 startPoint, Vector3 endPoint, Vector3 nextPoint)
    {
        Vector3 lineDirection = endPoint - startPoint;
        Vector3 lineDirectionRight = (new Vector3(-lineDirection.z, 0, lineDirection.x) / Mathf.Sqrt(lineDirection.x * lineDirection.x + lineDirection.z * lineDirection.z)) * -halfLineWidth;
        Vector3 lineDirectionLeft = (new Vector3(-lineDirection.z, 0, lineDirection.x) / Mathf.Sqrt(lineDirection.x * lineDirection.x + lineDirection.z * lineDirection.z)) * halfLineWidth;
              
        Vector3 previousLineDirection = startPoint - previousPoint;
        Vector3 previousLineDirectionRight;
        Vector3 previousLineDirectionLeft;
        if (previousLineDirection != Vector3.zero)
        {
            previousLineDirectionRight = (new Vector3(-previousLineDirection.z, 0, previousLineDirection.x) / Mathf.Sqrt(previousLineDirection.x * previousLineDirection.x + previousLineDirection.z * previousLineDirection.z)) * -halfLineWidth;
            previousLineDirectionLeft = (new Vector3(-previousLineDirection.z, 0, previousLineDirection.x) / Mathf.Sqrt(previousLineDirection.x * previousLineDirection.x + previousLineDirection.z * previousLineDirection.z)) * halfLineWidth;
        }
        else
        {
            previousLineDirectionRight = lineDirectionRight;
            previousLineDirectionLeft = lineDirectionLeft;
        }

        Vector3 nextLineDirection = nextPoint - endPoint;
        Vector3 nextLineDirectionRight;
        Vector3 nextLineDirectionLeft;
        if (nextLineDirection != Vector3.zero)
        {
            nextLineDirectionRight = (new Vector3(-nextLineDirection.z, 0, nextLineDirection.x) / Mathf.Sqrt(nextLineDirection.x * nextLineDirection.x + nextLineDirection.z * nextLineDirection.z)) * -halfLineWidth;
            nextLineDirectionLeft = (new Vector3(-nextLineDirection.z, 0, nextLineDirection.x) / Mathf.Sqrt(nextLineDirection.x * nextLineDirection.x + nextLineDirection.z * nextLineDirection.z)) * halfLineWidth;
        }
        else
        {
            nextLineDirectionRight = lineDirectionRight;
            nextLineDirectionLeft = lineDirectionLeft;
        }

        Vector3 startPointOffsetRight = (lineDirectionRight + previousLineDirectionRight);
        startPointOffsetRight = startPointOffsetRight / Mathf.Sqrt(startPointOffsetRight.x * startPointOffsetRight.x + startPointOffsetRight.z * startPointOffsetRight.z) *-halfLineWidth;
        Vector3 startPointOffsetLeft = (lineDirectionLeft + previousLineDirectionLeft);
        startPointOffsetLeft = startPointOffsetLeft / Mathf.Sqrt(startPointOffsetLeft.x * startPointOffsetLeft.x + startPointOffsetLeft.z * startPointOffsetLeft.z) * -halfLineWidth;
        Vector3 endLinePointRight = (lineDirectionRight + nextLineDirectionRight);
        endLinePointRight = endLinePointRight / Mathf.Sqrt(endLinePointRight.x * endLinePointRight.x + endLinePointRight.z * endLinePointRight.z) * -halfLineWidth;
        Vector3 endPointOffsetLeft = (lineDirectionLeft + nextLineDirectionLeft);
        endPointOffsetLeft = endPointOffsetLeft / Mathf.Sqrt(endPointOffsetLeft.x * endPointOffsetLeft.x + endPointOffsetLeft.z * endPointOffsetLeft.z) * -halfLineWidth;

        vertices.Add(startPoint + startPointOffsetRight);
        uv.Add(Vector2.right);
        int startRight = vertices.Count - 1;
        vertices.Add(endPoint + endLinePointRight);
        uv.Add(Vector2.right + Vector2.up);
        int endRight = vertices.Count - 1;
        vertices.Add(endPoint + endPointOffsetLeft);
        uv.Add(Vector2.up);
        int endLeft = vertices.Count - 1;
        vertices.Add(startPoint + startPointOffsetLeft);
        uv.Add(Vector2.zero);
        int startLeft = vertices.Count - 1;

        AddQuad(startRight, endRight, endLeft, startLeft);
    }
    
    /// <summary>
    /// Adds lines around a point to create a diamond shape.
    /// </summary>
    /// <param name="point"></param>
    private void TriangulatePoint(Vector2 point)
    {
        var right = point + Vector2.right * circleRadius;
        var up = point + Vector2.up * circleRadius;
        var left = point + Vector2.left * circleRadius;
        var down = point + Vector2.down * circleRadius;
        TriangulateLine(down, right, up, left);
        TriangulateLine(right, up, left, down);
        TriangulateLine(up, left, down, right);
        TriangulateLine(left, down, right, up);
    }

    private void AddQuad(int a, int b, int c, int d)
    {
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
        triangles.Add(a);
        triangles.Add(c);
        triangles.Add(d);
    }
}