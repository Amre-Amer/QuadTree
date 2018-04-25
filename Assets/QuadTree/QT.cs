using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QT
{
    QuadClass quad;
    public int maxQuadPoints;
    public QT(int maxQuadPoints0, Vector2 pos, float maxX, float maxZ) {
        maxQuadPoints = maxQuadPoints0;
        Rect rect = new Rect(pos.x, pos.y, maxX, maxZ);
        Debug.Log("qt:" + rect + "\n");
        quad = new QuadClass(rect);
    }

    public bool InsertData(Vector2 pos, int dataIndex) {
        return quad.InsertPoint(pos, dataIndex);        
    }

    public void GetPointsFromRange(List<PointStruct>result, Rect rectRange) {
        quad.GetDataFromRange(result, rectRange);        
    }
}

public struct PointStruct {
    public int dataIndex;
    public Vector2 pos;
    public PointStruct(Vector2 pos0, int dataIndex0) {
        pos = pos0;
        dataIndex = dataIndex0;
    }
}

public class QuadClass {
    public bool ynDivided;
    public int lastPoint;
    public Rect rect;
    public QuadClass q1;
    public QuadClass q2;
    public QuadClass q3;
    public QuadClass q4;
    public int maxQuadPoints = 4;
    public PointStruct[] quadPoints;
    GameObject goRect;
    public QuadClass(Rect rect00) {
        float w = rect00.size.x;
        float h = rect00.size.y;
        Vector2 pos = new Vector2(rect00.center.x - w / 2, rect00.center.y - h / 2);
        rect = new Rect(pos, new Vector2(w, h));
        Debug.Log("quad:" + rect + "\n");
        ynDivided = false;
        quadPoints = new PointStruct[maxQuadPoints];
        lastPoint = 0;
        goRect = GameObject.CreatePrimitive(PrimitiveType.Cube);
        goRect.transform.position = new Vector3(rect.center.x, 0, rect.center.y);
        goRect.transform.localScale = new Vector3(rect.size.x, 0, rect.size.y);
        MakeMaterialTransparent(goRect.GetComponent<Renderer>().material);
        goRect.GetComponent<Renderer>().material.color = Color.black;
        goRect.name = "quad:" + goRect.transform.position + " " + goRect.transform.localScale;
    }

    public void GetDataFromRange(List<PointStruct>result, Rect range) {
        if (RectIntersectRect(range, rect) == false) {
            return;
        }
//        Debug.Log("get:" + rect + "\n");
        for (int p = 0; p < lastPoint; p++) {
            if (range.Contains(quadPoints[p].pos) == true)
            {
//                Debug.Log("get add:" + rect + " contains:" + quadPoints[p].pos + "\n");
                result.Add(quadPoints[p]);
            }
        }
        if (ynDivided == true)
        {
            q1.GetDataFromRange(result, range);
            q2.GetDataFromRange(result, range);
            q3.GetDataFromRange(result, range);
            q4.GetDataFromRange(result, range);
        }
    }

    public bool RectIntersectRect(Rect rect1, Rect rect2)
    {
        bool result = true;
        if (rect1.center.x - rect1.size.x / 2 > rect2.center.x + rect2.size.x / 2)
        {
            result = false;
        }
        if (rect1.center.x + rect1.size.x / 2 < rect2.center.x - rect2.size.x / 2)
        {
            result = false;
        }
        if (rect1.center.y - rect1.size.y / 2 > rect2.center.y + rect2.size.y / 2)
        {
            result = false;
        }
        if (rect1.center.y + rect1.size.y / 2 < rect2.center.y - rect2.size.y / 2)
        {
            result = false;
        }
        return result;
    }

    public bool InsertPoint(Vector2 point, int dataIndex)
    {
//        Debug.Log(point + " " + rect + "\n");
        bool result = false;
        if (rect.Contains(point) == false)
        {
            return result;
        }
        if (lastPoint < maxQuadPoints) {
            quadPoints[lastPoint].pos = point;
            quadPoints[lastPoint].dataIndex = dataIndex;
            lastPoint++;
            result = true;
//            Debug.Log("add:" + point + " in " + rect + "\n");
            if (lastPoint == 1) goRect.GetComponent<Renderer>().material.color = Color.red;
            if (lastPoint == 2) goRect.GetComponent<Renderer>().material.color = Color.green;
            if (lastPoint == 3) goRect.GetComponent<Renderer>().material.color = Color.blue;
            if (lastPoint == 4) goRect.GetComponent<Renderer>().material.color = Color.yellow;
            if (lastPoint == 5) goRect.GetComponent<Renderer>().material.color = Color.cyan;
            goRect.name += " |";
        }
        else
        {
            Rect rect0;
            ynDivided = true;
            float w = rect.size.x / 2;
            float h = rect.size.y / 2;
            //rect0 = new Rect(rect.center + new Vector2(-w, 0), rect.size / 2);
            rect0 = new Rect(rect.center.x - w, rect.center.y - h, w, h);
            Debug.Log(rect + " divide 1:" + rect0 + "\n");
            q1 = new QuadClass(rect0);
            //rect0 = new Rect(rect.center + new Vector2(0, 0), rect.size / 2);
            rect0 = new Rect(rect.center.x, rect.center.y - h, w, h);
            Debug.Log(rect + " divide 2:" + rect0 + "\n");
            q2 = new QuadClass(rect0);
            //rect0 = new Rect(rect.center + new Vector2(-w, -h), rect.size / 2);
            rect0 = new Rect(rect.center.x - w, rect.center.y, w, h);
            Debug.Log(rect + " divide 3:" + rect0 + "\n");
            q3 = new QuadClass(rect0);
            //rect0 = new Rect(rect.center + new Vector2(0, -h), rect.size / 2);
            rect0 = new Rect(rect.center.x, rect.center.y, w, h);
            Debug.Log(rect + " divide 4:" + rect0 + "\n");
            q4 = new QuadClass(rect0);
            bool yn = false;
            yn = q1.InsertPoint(point, dataIndex);
            if (yn == false) 
            {
                yn = q2.InsertPoint(point, dataIndex);
            }
            if (yn == false)
            {
                yn = q3.InsertPoint(point, dataIndex);
            }
            if (yn == false)
            {
                yn = q4.InsertPoint(point, dataIndex);
            }
            //Debug.Log("add div:" + point + " " + rect + " div: " + rect0 + "\n");
            result = yn;
        }
        return result;
    }
    void MakeMaterialTransparent(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}
