using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeDemo : MonoBehaviour
{
    public int numBalls = 100;
    public float rad = 2;
    public float maxX = 100;
    public float maxY = 100;
    public float speed = .25f;
    public int cntSearch;
    public int cntQuads;
    public int cntResults;
    public int fps;
    public bool ynShowQuads;
    public bool ynUseQuadTree;
    BallStruct[] balls;
    int cntFps;
    int cntFrames;
    QTCircleStruct rangeBall;
    Vector2 centerQuadTree;
    Vector2 scaleQuadTree;
    GlobalClass global;
    GUIStyle guiStyle;

	private void Awake()
	{
        Application.targetFrameRate = 120;
	}
	void Start()
    {
        guiStyle = new GUIStyle();
        InitGlobal();
        InitFloor();
        InitRangeBall();
        InitBalls();
        InvokeRepeating("ShowFps", 1, 1);
    }

    void Update()
    {
        LoadGlobal();
        UpdateBalls();
        UnloadGlobal();
        cntFps++;
        cntFrames++;
    }

    void InitGlobal() {
        global = new GlobalClass();
    }

    void LoadGlobal() {
        global.ynShowQuads = ynShowQuads;
        global.ynUseQuadTree = ynUseQuadTree;
    }

    void UnloadGlobal() {
        cntQuads = global.cntQuads;
        cntResults = global.cntResults;
        cntSearch = global.cntSearch;
        ynShowQuads = global.ynShowQuads;
    }

    void InitRangeBall() {
        rangeBall = new QTCircleStruct(Vector2.zero, rad * 2);
    }

    void ShowFps() {
        fps = cntFps;
        cntFps = 0;
    }

    void InitFloor() {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "floor";
        go.transform.position = new Vector3(maxX / 2, 0, maxY / 2);
        go.transform.localScale = new Vector3(maxX, 0, maxY);
        MakeMaterialTransparent(go.GetComponent<Renderer>().material);
        go.GetComponent<Renderer>().material.color = new Color(.25f, .25f, .25f, .25f);
    }

    void InitBalls() {
        global.parentBalls = new GameObject("parentBalls");
        balls = new BallStruct[numBalls];
        for (int b = 0; b < numBalls; b++) {
            Vector2 pos = new Vector2(Random.Range(0, maxX), Random.Range(0, maxY));
            float s1 = speed * Random.Range(.5f, 1.5f);
            float s2 = speed * Random.Range(.5f, 1.5f);
            float ss1 = Random.Range(-s1, s2);
            float ss2 = Random.Range(-s1, s2);
            Vector2 velocity = new Vector2(ss1, ss2);
            balls[b] = new BallStruct(pos, velocity, rad, global);
        }
    }

    void UpdateBalls() {
        ResetGlobal();
        if (ynUseQuadTree == true) {
            InitQuadTree();
        } else {
            global.ynShowQuads = false;
            RemoveQuads();
        }
        for (int b = 0; b < numBalls; b++)
        {
            balls[b].Move();
            balls[b].CheckBorders(maxX, maxY);
            rangeBall.center = balls[b].pos;
            int bNearest = balls[b].FindNearest(balls, rangeBall, global);
            if (bNearest > -1) {
                balls[b].Avoid(balls[bNearest].pos);
            }
        }
    }

    void InitQuadTree()
    {
        if (ynShowQuads == true)
        {
            RemoveQuads();
            global.parentQuads = new GameObject("parentQuads");
        } else {
            RemoveQuads();
        }
        float x = maxX / 2;
        float y = maxY / 2;
        float sx = maxX;
        float sy = maxY;
        centerQuadTree = new Vector2(x, y);
        scaleQuadTree = new Vector2(sx, sy);
        global.quadTree = new QTQuadClass(centerQuadTree, scaleQuadTree, global);
        global.quadTree.LoadBalls(balls);
    }

    void ResetGlobal() {
        global.cntQuads = 0;
        global.cntSearch = 0;
        global.cntResults = 0;
    }

    void RemoveQuads() {
        if (global.parentQuads != null) DestroyImmediate(global.parentQuads);
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

	private void OnGUI()
	{
        guiStyle.fontSize = 40;
        GUI.Label(new Rect(10, 10, 100, 20), "fps:" + fps, guiStyle);
	}
}

public class QTQuadClass
{
    public int capacity = 4;
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
        data = new QTDataStruct[capacity];
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
        for (int b = 0; b < balls.Length; b++) {
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
        if (lastData < capacity)
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

public struct QTDataStruct
{
    public Vector2 pos;
    public int dataIndex;
    public QTDataStruct(Vector2 pos0, int dataIndex0) {
        pos = pos0;
        dataIndex = dataIndex0;
    }
}

public struct QTCircleStruct {
    public Vector2 center;
    public float rad;
    public QTCircleStruct(Vector2 center0, float rad0) {
        center = center0;
        rad = rad0;
    }
    public bool Contains(Vector2 pos) {
        float dist = Vector2.Distance(center, pos);
        if (dist <= rad) {
            return true;
        }
        return false;
    }
}

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
    public bool IntersectsCircle(QTCircleStruct circle) {
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

public struct BallStruct {
    public float rad;
    public Vector2 pos;
    public Vector2 velocity;
    public GameObject go;
    public BallStruct(Vector2 pos0, Vector2 velocity0, float rad0, GlobalClass global) {
        rad = rad0;
        pos = pos0;
        velocity = velocity0;
        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.parent = global.parentBalls.transform;
        go.name = "ball";
        go.transform.position = new Vector3(pos.x, 0, pos.y);
        go.transform.localScale = new Vector3(rad * 2, rad * 2, rad * 2);
    }
    public void Move() {
        go.transform.position += new Vector3(velocity.x, 0, velocity.y);
        UpdatePos();
    }
    public void UpdatePos() {
        pos = new Vector2(go.transform.position.x, go.transform.position.z);
    }
    public void Avoid(Vector2 posAvoid) {
        float mag = velocity.magnitude;
        velocity = pos - posAvoid;
        if (velocity.sqrMagnitude > 1)
        {
            velocity = velocity.normalized;
        }
        velocity *= mag;
    }
    public bool IsNear(Vector2 posNear) {
        float distNear = rad * 2;
        float dist = Vector2.Distance(pos, posNear);
        if (dist <= distNear)
        {
            return true;
        }
        return false;
    }
    public int FindNearest(BallStruct[] balls, QTCircleStruct range, GlobalClass global) { 
        int result = -1;
        if (global.ynUseQuadTree == true) {
            List<QTDataStruct> results = new List<QTDataStruct>();
            global.quadTree.SelectRange(range, results);
            if (results.Count > 0) {
                for (int r = 0; r < results.Count; r++) {
                    if (go != balls[results[r].dataIndex].go) {
                        result = results[r].dataIndex;
                        global.cntResults = results.Count;
                        break;                        
                    }
                }
            }
        } else {
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
    public void CheckBorders(float maxX, float maxY) {
        if (pos.x < rad) {
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
    public void SetColor(Color col) {
        go.GetComponent<Renderer>().material.color = col;
    }
}

public class GlobalClass {
    public int cntQuads;
    public int cntResults;
    public int cntSearch;
    public bool ynShowQuads;
    public GameObject parentQuads;
    public GameObject parentBalls;
    public bool ynUseQuadTree;
    public QTQuadClass quadTree;
}