using System.Collections.Generic;
using UnityEngine;

public class QTQuadClass
{
    public int numQuads = 4;
    public QTRectStruct rectQuad;
    public QTQuadClass[] quads;
    public List<QTDataStruct> data;
    public GameObject go;
    public GlobalClass global;
    public float sy;
    public int level; 
    public float upDown;
    public QTQuadClass(Vector2 center, Vector2 scale, GlobalClass global0)
    {
        global = global0;
        global.cntQuads++;
        rectQuad = new QTRectStruct(center, scale);
        quads = new QTQuadClass[numQuads];
        data = new List<QTDataStruct>();
        upDown = 1;
        float scaleSy = .05f;
        sy = global.scaleQuadTree.x / rectQuad.scale.x;
        level = (int)Mathf.Log(sy, 2);
        if (level > global.deepestLevel) {
            global.deepestLevel = level;
            global.deepestSize = scale.x;
        }
        sy = level * 2f;
        sy *= global.scaleQuadTree.y * scaleSy;
        if (global.ynShowQuads == true)
        {
            go = new GameObject("quad");
            go.transform.parent = global.parentQuads.transform;
            float s = .025f;
            float w = scale.x;
            float h = scale.y;
            //
            //for (float ang = 0; ang < 360; ang += 90) {
            //    GameObject go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    go0.transform.parent = go.transform;
            //    go0.name = "quad side " + ang;
            //    Vector3 pos = new Vector3(center.x, -sy / 2, center.y);
            //    go0.transform.position = pos + Vector3.right * w / 2;
            //    go0.transform.localScale = new Vector3(s * h, sy, h);
            //    go0.transform.Rotate(0, ang, 0);
            //    global.MakeMaterialTransparent(go0.GetComponent<Renderer>().material);
            //    go0.GetComponent<Renderer>().material.color = Color.clear;
            //}
            bool ynTransparent = false;
            GameObject go0;
            go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go0.transform.parent = go.transform;
            go0.name = "L";
            go0.transform.position = new Vector3(center.x - w / 2, upDown * sy/2, center.y);
            go0.transform.localScale = new Vector3(s * h, sy, h);
            if (ynTransparent == true) global.MakeMaterialTransparent(go0.GetComponent<Renderer>().material);
            go0.GetComponent<Renderer>().material.color = new Color(1, 0, 0, .25f);
            //
            go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go0.transform.parent = go.transform;
            go0.name = "R";
            go0.transform.position = new Vector3(center.x + w / 2, upDown * sy / 2, center.y);
            go0.transform.localScale = new Vector3(s * h, sy, h);
            if (ynTransparent == true) global.MakeMaterialTransparent(go0.GetComponent<Renderer>().material);
            go0.GetComponent<Renderer>().material.color = new Color(0, 1, 0, .25f);
            //
            go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go0.transform.parent = go.transform;
            go0.name = "D";
            go0.transform.position = new Vector3(center.x, upDown * sy / 2, center.y - h / 2);
            go0.transform.localScale = new Vector3(w, sy, s * w);
            if (ynTransparent == true) global.MakeMaterialTransparent(go0.GetComponent<Renderer>().material);
            go0.GetComponent<Renderer>().material.color = new Color(0, 0, 1, .25f);
            //
            go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go0.transform.parent = go.transform;
            go0.name = "U";
            go0.transform.position = new Vector3(center.x, upDown * sy / 2, center.y + h / 2);
            go0.transform.localScale = new Vector3(w, sy, s * w);
            if (ynTransparent == true) global.MakeMaterialTransparent(go0.GetComponent<Renderer>().material);
            go0.GetComponent<Renderer>().material.color = new Color(1, 1, 0, .25f);
            //
            go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go0.transform.parent = go.transform;
            go0.name = "B";
            go0.transform.position = new Vector3(center.x, upDown * sy, center.y);
            go0.transform.localScale = new Vector3(w, .01f, h);
//            if (ynTransparent == true) global.MakeMaterialTransparent(go0.GetComponent<Renderer>().material);
            global.MakeMaterialTransparent(go0.GetComponent<Renderer>().material);
            go0.GetComponent<Renderer>().material.color = new Color(.5f, .5f, .5f, .75f);
        }
    }
    public void SelectRange(QTCircleStruct rangeCircle, List<QTDataStruct> results)
    {
        if (rectQuad.IntersectsCircle(rangeCircle) == false)
        {
            return;
        }
        global.cntSearch++;
        for (int d = 0; d < data.Count; d++)
        {
            if (rangeCircle.Contains(data[d].pos) == true)
            {
                results.Add(data[d]);
            }
        }
        for (int q = 0; q < numQuads; q++)
        {
            if (quads[q] != null)
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
    //void UpdateBallPos() {
        
    //}
    public bool Insert(QTDataStruct dat)
    {
        bool result = false;
        if (rectQuad.Contains(dat.pos) == false)
        {
            return result;
        }
        if (data.Count < global.capacity)
        {
            if (global.ynATC == true)
            {
                Vector2 posXY = global.balls[dat.dataIndex].pos;
                Vector3 pos = new Vector3(posXY.x, sy * upDown, posXY.y);
                global.balls[dat.dataIndex].MoveTo(pos);
            }
            data.Add(dat);
            result = true;
            if (global.ynShowQuads == true)
            {
                go.name += " |";
            }
        }
        else
        {
            bool yn = false;
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
                    QTRectStruct rect = new QTRectStruct(center, scale);
                    if (rect.Contains(dat.pos))
                    {
                        if (quads[q] == null)
                        {
                            quads[q] = new QTQuadClass(center, scale, global);
                         }
                        yn = quads[q].Insert(dat);
                        if (yn == true)
                        {
                            result = true;
                            break;
                        }
                    }
                    q++;
                }
                if (result == true)
                {
                    break;
                }
            }
        }
        return result;
    }
}


