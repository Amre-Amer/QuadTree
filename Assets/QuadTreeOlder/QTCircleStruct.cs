using UnityEngine;

public struct QTCircleStruct
{
    public Vector2 center;
    public float rad;
    public QTCircleStruct(Vector2 center0, float rad0)
    {
        center = center0;
        rad = rad0;
    }
    public bool Contains(Vector2 pos)
    {
        float dist = Vector2.Distance(center, pos);
        if (dist <= rad)
        {
            return true;
        }
        return false;
    }
}
