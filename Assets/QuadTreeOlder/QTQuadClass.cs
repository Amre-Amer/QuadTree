using System.Collections.Generic;
using UnityEngine;

public class QTQuadClass
{
    public int numQuads = 4;
    public QTRectStruct rectQuad;
    public QTQuadClass[] quads;
    public QTDataStruct[] data;
    public bool isDivided;
    public int lastData;
    public GameObject go;
    public GlobalClass global;
    public QTQuadClass(Vector2 center, Vector2 scale, GlobalClass global0)
    {
        global = global0;
        global.cntQuads++;
        rectQuad = new QTRectStruct(center, scale);
        data = new QTDataStruct[global.capacity];
        lastData = 0;
        isDivided = false;
        if (global.ynShowQuads == true)
        {
            go = new GameObject("quad");
            go.transform.parent = global.parentQuads.transform;
            float s = .025f;
            float w = scale.x;
            float h = scale.y;
            //
            GameObject go0;
            go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go0.transform.parent = go.transform;
            go0.name = "L";
            go0.transform.position = new Vector3(center.x - w / 2, 0, center.y);
            go0.transform.localScale = new Vector3(s * h, 0, h);
            //
            go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go0.transform.parent = go.transform;
            go0.name = "R";
            go0.transform.position = new Vector3(center.x + w / 2, 0, center.y);
            go0.transform.localScale = new Vector3(s * h, 0, h);
            //
            go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go0.transform.parent = go.transform;
            go0.name = "D";
            go0.transform.position = new Vector3(center.x, 0, center.y - h / 2);
            go0.transform.localScale = new Vector3(w, 0, s * w);
            //
            go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go0.transform.parent = go.transform;
            go0.name = "U";
            go0.transform.position = new Vector3(center.x, 0, center.y + h / 2);
            go0.transform.localScale = new Vector3(w, 0, s * w);
        }
    }
    public void SelectRange(QTCircleStruct rangeCircle, List<QTDataStruct> results)
    {
        if (rectQuad.IntersectsCircle(rangeCircle) == false)
        {
            return;
        }
        global.cntSearch++;
        for (int d = 0; d < lastData; d++)
        {
            if (rangeCircle.Contains(data[d].pos) == true)
            {
                results.Add(data[d]);
            }
        }
        if (isDivided == true)
        {
            for (int q = 0; q < numQuads; q++)
            {
                quads[q].SelectRange(rangeCircle, results);
            }
        }
    }
    public void LoadBalls(BallStruct[] balls)
    {
        for (int b = 0; b < balls.Length; b++)
        {
            BallStruct ball = balls[b];
            QTDataStruct dat = new QTDataStruct(ball.pos, b);
            Insert(dat);
        }
    }
    public bool Insert(QTDataStruct dat)
    {
        bool result = false;
        if (rectQuad.Contains(dat.pos) == false)
        {
            return result;
        }
        if (lastData < global.capacity)
        {
            data[lastData] = dat;
            lastData++;
            result = true;
        }
        else
        {
            if (isDivided == false)
            {
                quads = new QTQuadClass[numQuads];
                int q = 0;
                for (int ix = -1; ix <= 1; ix += 2)
                {
                    for (int iy = -1; iy <= 1; iy += 2)
                    {
                        float w = rectQuad.scale.x;
                        float h = rectQuad.scale.y;
                        float x = rectQuad.center.x;
                        float y = rectQuad.center.y;
                        Vector2 center = new Vector2(x + ix * w / 4, y + iy * h / 4);
                        Vector2 scale = new Vector2(w / 2, h / 2);
                        quads[q] = new QTQuadClass(center, scale, global);
                        q++;
                    }
                }
                isDivided = true;
            }
            bool yn = false;
            for (int q = 0; q < numQuads; q++)
            {
                if (yn == false)
                {
                    yn = quads[q].Insert(dat);
                    if (yn == true)
                    {
                        result = true;
                        break;
                    }
                }
            }
        }
        return result;
    }
}


