using UnityEngine;
using System.Collections;

public static class MathUtility
{
    /// <summary>
    /// Calculates the angle for a direction with 0 meaning upwards.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static float DirectionToRadian (Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x);
    }

    /// <summary>
    /// Calculates the angle for a direction with 0 meaning upwards.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float DirectionToRadian(float x, float y)
    {
        return Mathf.Atan2(y, x);
    }

    /// <summary>
    /// Calculates the angle for a direction with 0 meaning upwards.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static float DirectionToDegree(Vector2 vector)
    {
        float angle = Mathf.Rad2Deg * Mathf.Atan2(vector.y, vector.x);
        if(angle < 0)
        {
            angle += 360f;
        }
        return angle;
    }

    /// <summary>
    /// Calculates the angle for a direction with 0 meaning upwards.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float DirectionToDegree(float x, float y)
    {
        return Mathf.Rad2Deg * Mathf.Atan2(y, x);
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
    }

    public static Vector2 RadianToVector2(float radian, float length)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * length;
    }

    public static Vector2 DegreeToVector2(float degree, float length)
    {
        return new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad)) * length;
    }

    /// <summary>
    /// Calculates the intersection point for two lines.
    /// </summary>
    /// <param name="startPoint1">The startpoint of the first line.</param>
    /// <param name="endPoint1">The endpoint of the first line.</param>
    /// <param name="startPoint2">The startpoint of the second line.</param>
    /// <param name="endPoint2">The endpoint of the second line.</param>
    /// <returns>Null if the lines are parallel, the intersection point otherwise.</returns>
    public static Vector2? LineIntersectionPoint(Vector2 startPoint1, Vector2 endPoint1, Vector2 startPoint2, Vector2 endPoint2)
    {
        // Get A,B,C of first line - points : startPoint1 to endPoint1
        float A1 = endPoint1.y - startPoint1.y;
        float B1 = startPoint1.x - endPoint1.x;
        float C1 = A1 * startPoint1.x + B1 * startPoint1.y;

        // Get A,B,C of second line - points : startPoint2 to endPoint2
        float A2 = endPoint2.y - startPoint2.y;
        float B2 = startPoint2.x - endPoint2.x;
        float C2 = A2 * startPoint2.x + B2 * startPoint2.y;

        // Check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
            return null; // parallel lines don't have an intersection point

        // Return the intersection point
        return new Vector2(
            (B2 * C1 - B1 * C2) / delta,
            (A1 * C2 - A2 * C1) / delta
        );
    }
}