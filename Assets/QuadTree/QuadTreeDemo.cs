using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeDemo : MonoBehaviour
{
    public int capacity = 4;
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
        global.capacity = capacity;
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

