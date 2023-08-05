using UnityEngine;

public class GameUtils
{
    static public float Distance2d(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    static public Vector3 Direction2d(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return b - a;
    }

    static public Vector3 ZeroHeight(Vector3 v)
    {
        v.y = 0f;
        return v;
    }
}
