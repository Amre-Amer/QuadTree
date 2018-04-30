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
    public int deepestLevel;
    public float deepestSize;
    public float closestDist;
    public int fps;
    public float elapsedTimeMS;
    public bool ynShowQuads = true;
    public bool ynUseQuadTree = true;
    public bool ynFollowTarget;
    public bool ynAvoid = true;
    public bool ynClosestDist;
    bool ynClosestDistLast;
    public bool ynATC;
    public bool ynStep;
    public bool ynRandom;
    public bool ynTrails;
    int cntFps;
    QTCircleStruct rangeBall;
    GlobalClass global;
    GUIStyle guiStyle;
    float timeStart;
    int[] randomChoices;

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
        if (ynStep == true && Time.realtimeSinceStartup - timeStart < 1.5f) {
            
            return;
        }
        timeStart = Time.realtimeSinceStartup;
        LoadGlobal();
        UpdateBalls();
        UnloadGlobal();
        cntFps++;
        global.cntFrames++;
    }

    void InitGlobal() {
        global = new GlobalClass();
        //
        if (ynFollowTarget == true)
        {
            global.target = new Vector3(maxX / 2, 0, maxY / 2);
            global.targetGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            global.targetGo.transform.position = global.target;
            global.targetGo.transform.localScale = new Vector3(3, 3, 3);
            global.targetGo.GetComponent<Renderer>().material.color = Color.cyan;
            global.radTarget = maxX * .35f;
        }
    }

    void LoadGlobal() {
        global.ynShowQuads = ynShowQuads;
        global.ynUseQuadTree = ynUseQuadTree;
        global.capacity = capacity;
        global.ynFollowTarget = ynFollowTarget;
        global.ynAvoid = ynAvoid;
        global.ynATC = ynATC;
        global.ynStep = ynStep;
        global.ynRandom = ynRandom;
        global.ynTrails = ynTrails;
    }

    void UnloadGlobal() {
        cntQuads = global.cntQuads;
        cntResults = global.cntResults;
        cntSearch = global.cntSearch;
        ynShowQuads = global.ynShowQuads;
        elapsedTimeMS = global.elapsedTimeMS;
        deepestLevel = global.deepestLevel;
        deepestSize = global.deepestSize;
        closestDist = global.closestDist;
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
        global.MakeMaterialTransparent(go.GetComponent<Renderer>().material);
        go.GetComponent<Renderer>().material.color = new Color(.25f, .25f, .25f, .25f);
    }

    void InitBalls() {
        global.parentBalls = new GameObject("parentBalls");
        global.balls = new BallStruct[numBalls];
        for (int b = 0; b < numBalls; b++) {
            Vector2 pos = new Vector2(Random.Range(0, maxX), Random.Range(0, maxY));
            float s1 = speed * Random.Range(.75f, 1.5f);
            float s2 = speed * Random.Range(.75f, 1.5f);
            float ss1 = Random.Range(-s1, s2);
            float ss2 = Random.Range(-s1, s2);
            Vector2 velocity = new Vector2(ss1, ss2);
            global.balls[b] = new BallStruct(pos, velocity, rad, global);
            if (ynTrails == true)
            {
                if (b == numBalls - 1 || b == 0 || b == numBalls / 2)
                {
                    TrailRenderer tr = global.balls[b].go.AddComponent<TrailRenderer>();
                    //tr.GetComponent<Renderer>().material.color = Color.blue;
                }
            }
        }
    }

    void UpdateBalls() {
        float t = Time.realtimeSinceStartup;
        ResetGlobalCounts();
        if (ynUseQuadTree == true) {
            InitQuadTree();
        } else {
            global.ynShowQuads = false;
            RemoveQuads();
        }
        if (ynFollowTarget == true) {
            UpdateTarget();
        }
        resetRandom();
        for (int b0 = 0; b0 < numBalls; b0++)
        {
            int b = getRandomBall(b0);
            global.balls[b].Move();
            global.balls[b].CheckBorders(maxX, maxY);
            if (global.ynAvoid == true)
            {
                rangeBall.center = global.balls[b].pos;
                int bNearest = global.balls[b].FindNearest(global.balls, rangeBall, global);
                if (bNearest > -1)
                {
                    global.balls[b].Avoid(global.balls[bNearest].pos);
                }
            }
        }
        if (ynClosestDist == true)
        {
            if (ynClosestDistLast == false) {
                Debug.Log("clostest\n");
                global.closestDist = maxX;
            }
            calcClosestDist();
        }
        ynClosestDistLast = ynClosestDist;
        float tAdd = (Time.realtimeSinceStartup - t) * 1000f;
        float smooth = .125f;
        global.elapsedTimeMS = smooth * tAdd + (1 - smooth) * global.elapsedTimeMS;
    }

    void calcClosestDist() {
        for (int b = 0; b < numBalls; b++) {
            for (int b0 = 0; b0 < numBalls; b0++)
            {
                if (b != b0)
                {
                    float dist = Vector2.Distance(global.balls[b].pos, global.balls[b0].pos);
                    if (dist < global.closestDist)
                    {
                        global.closestDist = dist;
                    }
                }
            }
        }
    }
    void resetRandom() {
        randomChoices = new int[numBalls];
    }

    int getRandomBall(int bCheck) {
        int bResult;
        bool ynFound;
        do
        {
            bResult = Random.Range(0, numBalls);
            ynFound = true;
            for (int c = 0; c < bCheck; c++) {
                if (bResult == randomChoices[c]) {
                    ynFound = false;
                    break;
                }

            }
        } while (ynFound == false);
        randomChoices[bCheck] = bResult;
        return bResult;
    }

    void UpdateTarget() {
        float x = global.centerQuadTree.x + global.radTarget * Mathf.Cos(global.cntFrames * Mathf.Deg2Rad);
        float z = global.centerQuadTree.y + global.radTarget * Mathf.Sin(global.cntFrames * Mathf.Deg2Rad);
        global.target = new Vector3(x, 0, z);
        global.targetGo.transform.position = global.target;
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
        global.centerQuadTree = new Vector2(x, y);
        global.scaleQuadTree = new Vector2(sx, sy);
        global.quadTree = new QTQuadClass(global.centerQuadTree, global.scaleQuadTree, global);
        global.quadTree.LoadBalls(global.balls);
    }

    void ResetGlobalCounts() {
        global.cntQuads = 0;
        global.cntSearch = 0;
        global.cntResults = 0;
        global.deepestLevel = 0;
        global.deepestSize = global.scaleQuadTree.x;
        if (global.closestDist == 0)
        {
            global.closestDist = maxX;
        }
    }

    void RemoveQuads() {
        if (global.parentQuads != null) DestroyImmediate(global.parentQuads);
    }

	private void OnGUI()
	{
        guiStyle.fontSize = 40;
        GUI.Label(new Rect(10, 10, 100, 20), "fps:" + fps, guiStyle);
	}
}

