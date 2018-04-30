using System.Collections.Generic;
using UnityEngine;

public struct BallStruct
{
    public float rad;
    public Vector2 pos;
    public Vector2 velocity;
    public GameObject go;
    public GlobalClass global;
    public BallStruct(Vector2 pos0, Vector2 velocity0, float rad0, GlobalClass global0)
    {
        global = global0;
        rad = rad0;
        pos = pos0;
        velocity = velocity0;
        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.parent = global.parentBalls.transform;
        go.name = "ball";
        go.transform.position = new Vector3(pos.x, 0, pos.y);
        go.transform.localScale = new Vector3(rad * 2, rad * 2, rad * 2);
        SetColor(Random.ColorHSV());
    }
    public void Move()
    {
        Vector3 pos0 = Vector3.zero;
        if (global.ynFollowTarget == true) {
            go.transform.LookAt(global.target);
            pos0 = go.transform.position + go.transform.forward * velocity.magnitude;
        } else {
            pos0 = go.transform.position += new Vector3(velocity.x, 0, velocity.y);
        }
        MoveTo(pos0);
    }
    public void MoveTo(Vector3 pos0) {
        float smooth = .1f;
        go.transform.position = (1 - smooth) * go.transform.position + smooth * pos0;
        UpdatePos(pos0);
    }
    void UpdatePos(Vector3 pos0) {
        pos = new Vector2(pos0.x, pos0.z);
    }
    public void Avoid(Vector2 posAvoid)
    {
        float mag = velocity.magnitude;
        velocity = pos - posAvoid;
        if (velocity.sqrMagnitude > 1)
        {
            velocity = velocity.normalized;
        }
        velocity *= mag;
    }
    public bool IsNear(Vector2 posNear)
    {
        float distNear = rad * 2;
        float dist = Vector2.Distance(pos, posNear);
        if (dist <= distNear)
        {
            return true;
        }
        return false;
    }
    public int FindNearest(BallStruct[] balls, QTCircleStruct range, GlobalClass global)
    {
        int result = -1;
        if (global.ynUseQuadTree == true)
        {
            List<QTDataStruct> results = new List<QTDataStruct>();
            global.quadTree.SelectRange(range, results);
            if (results.Count > 0)
            {
                for (int r = 0; r < results.Count; r++)
                {
                    if (go != balls[results[r].dataIndex].go)
                    {
                        result = results[r].dataIndex;
                        global.cntResults = results.Count;
                        break;
                    }
                }
            }
        }
        else
        {
            for (int b = 0; b < balls.Length; b++)
            {
                global.cntSearch++;
                if (go != balls[b].go)
                {
                    if (balls[b].IsNear(pos))
                    {
                        result = b;
                        break;
                    }
                }
            }
        }
        return result;
    }
    public void CheckBorders(float maxX, float maxY)
    {
        if (pos.x < rad)
        {
            velocity.x = Mathf.Abs(velocity.x);
            return;
        }
        if (pos.x > maxX - rad)
        {
            velocity.x = -1 * Mathf.Abs(velocity.x);
            return;
        }
        if (pos.y < rad)
        {
            velocity.y = Mathf.Abs(velocity.y);
            return;
        }
        if (pos.y > maxY - rad)
        {
            velocity.y = -1 * Mathf.Abs(velocity.y);
            return;
        }
    }
    public void SetColor(Color col)
    {
        go.GetComponent<Renderer>().material.color = col;
    }
}
