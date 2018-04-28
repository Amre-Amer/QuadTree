using UnityEngine;

public struct QTRectStruct
{
    public Vector2 center;
    public Vector2 scale;
    public QTRectStruct(Vector2 center0, Vector2 scale0)
    {
        center = center0;
        scale = scale0;
    }
    public bool Contains(Vector2 pos)
    {
        if (pos.x < center.x - scale.x / 2)
        {
            return false;
        }
        if (pos.x > center.x + scale.x / 2)
        {
            return false;
        }
        if (pos.y < center.y - scale.y / 2)
        {
            return false;
        }
        if (pos.y > center.y + scale.y / 2)
        {
            return false;
        }
        return true;
    }
    public bool IntersectsCircle(QTCircleStruct circle)
    {
        if (center.x + scale.x / 2 < circle.center.x - circle.rad)
        {
            return false;
        }
        if (center.x - scale.x / 2 > circle.center.x + circle.rad)
        {
            return false;
        }
        if (center.y + scale.y / 2 < circle.center.y - circle.rad)
        {
            return false;
        }
        if (center.y - scale.y / 2 > circle.center.y + circle.rad)
        {
            return false;
        }
        return true;
    }
    public bool IntersectsRect(QTRectStruct rect)
    {
        if (center.x + scale.x / 2 < rect.center.x - rect.scale.x / 2)
        {
            return false;
        }
        if (center.x - scale.x / 2 > rect.center.x + rect.scale.x / 2)
        {
            return false;
        }
        if (center.y + scale.y / 2 < rect.center.y - rect.scale.y / 2)
        {
            return false;
        }
        if (center.y - scale.y / 2 > rect.center.y + rect.scale.y / 2)
        {
            return false;
        }
        return true;
    }
}
