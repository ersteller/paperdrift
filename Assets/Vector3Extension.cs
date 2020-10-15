using UnityEngine;

public static class Vector3Extension
{

    public static Vector3 Rotate(this Vector3 v, float rad)
    {
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        float tx = v.x;
        float ty = v.y;
        return new Vector3(cos * tx - sin * ty, sin * tx + cos * ty,0);
    }
}