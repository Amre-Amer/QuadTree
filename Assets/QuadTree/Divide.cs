using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Divide : MonoBehaviour {
    float maxX = 100;
    float maxZ = 100;
    public int numPoints = 10;
    Rect rectRange;
    GameObject rangeGo;
    Rect rectQT;
    PointDataStruct[] points;
    List<PointDataStruct> results;
    QuadClass qt;
    GameObject goParent;
    GameObject goGrandparent;
    public int fps;
    int cntFrames;
    public bool ynUseQT; 
//    public bool ynUseGraphics;
    public int cntSearch;
    public int cntResult;
    public int cntResult2;

    public struct PointDataStruct {
        public float x;
        public float z;
        public int index;
        public GameObject go;
        public float yaw;
        public float speed;
    }

    public class QuadClass {
        public GameObject goGrandparent;
        public int maxPointData;
        public int lastPointData;
        public PointDataStruct[] pointData;
        public bool isDivided;
        public Rect rectQuad;
        public QuadClass quad1;
        public QuadClass quad2;
        public QuadClass quad3;
        public QuadClass quad4;
        public GameObject quadGo;
        public QuadClass(Rect rect, GameObject goGrandParent0) {
            goGrandparent = goGrandParent0;
            maxPointData = 4;
            isDivided = false;
            rectQuad = new Rect(rect);
            pointData = new PointDataStruct[4];
            lastPointData = 0;
            Show(rectQuad);
        }

        public void GetRangeData(Rect rectRange, List<PointDataStruct>results, int cnt) {
            cnt++;
            //Debug.Log("-\n");
            if (RectsIntersect(rectRange, rectQuad) == false) {
                return;
            }
            for (int p = 0; p < lastPointData; p++) {
                if (rectRange.Contains(new Vector2(pointData[p].x, pointData[p].z)))
                {
                    results.Add(pointData[p]);
                }
            }
            if (isDivided == true) {
                quad1.GetRangeData(rectRange, results, cnt);                
                quad2.GetRangeData(rectRange, results, cnt);                
                quad3.GetRangeData(rectRange, results, cnt);                
                quad4.GetRangeData(rectRange, results, cnt);                
            }
        }

        public bool RectsIntersect(Rect rect1, Rect rect2) {
            bool result = true;
            float w1 = rect1.size.x;
            float h1 = rect1.size.y;
            float w2 = rect2.size.x;
            float h2 = rect2.size.y;
            Vector2 center1 = rect1.center;
            Vector2 center2 = rect2.center;
            if (center1.x + w1 < center2.x - w2) {
                result = false;
            }
            if (center1.x - w1 > center2.x + w2)
            {
                result = false;
            }
            if (center1.y + h1 < center2.y - h2)
            {
                result = false;
            }
            if (center1.y - h1 > center2.y + h2)
            {
                result = false;
            }
            return result;
        }

        public bool InsertData(float x, float z, int index) {
            bool result = false;
            if (rectQuad.Contains(new Vector2(x, z)) == false) {
                return result;
            }
            if (lastPointData < maxPointData) {
                pointData[lastPointData].x = x;
                pointData[lastPointData].z = z;
                pointData[lastPointData].index = index;
                result = true;
                quadGo.name += " |";
                //
                lastPointData++;
            } else {
                if (isDivided == false)
                {
                    float w = rectQuad.size.x / 2;
                    float h = rectQuad.size.y / 2;
                    Rect rect;
                    rect = new Rect(rectQuad.center.x - w, rectQuad.center.y - h, w, h);
                    quad1 = new QuadClass(rect, goGrandparent);
                    rect = new Rect(rectQuad.center.x, rectQuad.center.y - h, w, h);
                    quad2 = new QuadClass(rect, goGrandparent);
                    rect = new Rect(rectQuad.center.x - w, rectQuad.center.y, w, h);
                    quad3 = new QuadClass(rect, goGrandparent);
                    rect = new Rect(rectQuad.center.x, rectQuad.center.y, w, h);
                    quad4 = new QuadClass(rect, goGrandparent);
                    isDivided = true;
                }
                    //
                bool yn = false;
                if (yn == false) {
                    yn = quad1.InsertData(x, z, index);
                }
                if (yn == false)
                {
                    yn = quad2.InsertData(x, z, index);
                }
                if (yn == false)
                {
                    yn = quad3.InsertData(x, z, index);
                }
                if (yn == false)
                {
                    yn = quad4.InsertData(x, z, index);
                }
                result = yn;
            }
            return result;            
        }
        void Show(Rect rect)
        {
            GameObject goParent = new GameObject("quad:" + rect);
            goParent.transform.parent = goGrandparent.transform;
            GameObject go;
            float thick = .2f;
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = new Vector3(rect.center.x + rect.size.x / 2, 0, rect.center.y);
            go.transform.localScale = new Vector3(thick, thick, rect.size.y);
            go.transform.eulerAngles = new Vector3(0, 0, 0);
            go.transform.parent = goParent.transform;
            //
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = new Vector3(rect.center.x - rect.size.x / 2, 0, rect.center.y);
            go.transform.localScale = new Vector3(thick, thick, rect.size.y);
            go.transform.eulerAngles = new Vector3(0, 0, 0);
            go.transform.parent = goParent.transform;
            //
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = new Vector3(rect.center.x, 0, rect.center.y + rect.size.y / 2);
            go.transform.localScale = new Vector3(thick, thick, rect.size.x);
            go.transform.eulerAngles = new Vector3(0, 90, 0);
            go.transform.parent = goParent.transform;
            //
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = new Vector3(rect.center.x, 0, rect.center.y - rect.size.y / 2);
            go.transform.localScale = new Vector3(thick, thick, rect.size.x);
            go.transform.eulerAngles = new Vector3(0, 90, 0);
            go.transform.parent = goParent.transform;
            //
            quadGo = goParent;
        }
    }

	// Use this for initialization
	void Start () {
        Debug.Log("numPoints:" + numPoints + "\n");
        InitPoints();
        InitRectQT();
        CreateRange();
        InvokeRepeating("showFPS", 1, 1);
	}

    // Update is called once per frame
    void Update()
    {
        UpdatePoints();
        //
        //UpdateRange();
        ////
        UpdateQT();
        //

        UpdateCollisions();
        //
        cntFrames++;
    }

    void UpdatePoints() {
        for (int p = 0; p < points.Length; p++) {
            MovePoint(p);
        }
    }

    void UpdateCollisions() {
        cntSearch = 0;
        cntResult = 0;
        cntResult2 = 0;
        for (int p = 0; p < points.Length; p++)
        {
            CheckPointBorder(p);
            CheckPointCollision(p);
        }
    }

    void MovePoint(int p) {
        points[p].go.transform.eulerAngles = new Vector3(0, points[p].yaw, 0);
        points[p].go.transform.position += points[p].go.transform.forward * points[p].speed;
    }

    void CheckPointCollision(int p) {
        float distNear = 2;
        Vector3 pos = points[p].go.transform.position;
        if (ynUseQT == true) {
            rangeGo.transform.position = pos;
            rangeGo.transform.localScale = new Vector3(distNear, 0, distNear);
            //
            UpdateRange();
            //
            UpdateResultsQT(); 
            if (results.Count > 0) {
                cntResult ++;
                cntResult2 += results.Count;
                TurnAroundPoint(p);
            }
        } else {
            for (int pp = 0; pp < points.Length; pp++) {
                if (pp != p)
                {
                    float dist = Vector3.Distance(points[pp].go.transform.position, pos);
                    cntSearch++;
                    if (dist < distNear)
                    {
                        TurnAroundPoint(pp);
                        break;
                    }
                }
            }           
        }   
    }

    void UpdateQT()
    {
        if (goGrandparent != null) DestroyImmediate(goGrandparent);
        goGrandparent = new GameObject("goGrandparent");
        qt = new QuadClass(rectQT, goGrandparent);
        InsertPoints2QT(qt);
        //
    }

    void UpdateResultsQT() {
        // results
        //clearPointColors();
        results = new List<PointDataStruct>();
        qt.GetRangeData(rectRange, results, cntSearch);
        //showResults(results);
    }

    void CheckPointBorder(int p) {
        float x = points[p].go.transform.position.x;        
        float z = points[p].go.transform.position.z; 
        if (x < 0 || x > maxX || z < 0 || z > maxZ) {
            TurnAroundPoint(p);
        }
    }

    void TurnAroundPoint(int p) {
        float yaw = points[p].yaw;
        yaw += 180 + Random.Range(-45, 45);
        points[p].yaw = yaw;
        points[p].go.transform.eulerAngles = new Vector3(0, yaw, 0);
//        Debug.Log("turn around\n");
    }

    void CreateRange()
    {
        float x = 35;
        float z = 40;
        float w = 50;
        float h = 35;
        rangeGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 pos = new Vector3(x, 0, z);
        rangeGo.transform.position = pos;
        rangeGo.transform.localScale = new Vector3(w, 0, h);
        rectRange = new Rect(pos.x - w / 2, pos.z - h / 2, w, h);
    }

    void UpdateRange()
    {
        Vector3 pos = rangeGo.transform.position;
        float w = rangeGo.transform.localScale.x;
        float h = rangeGo.transform.localScale.z;
        rectRange = new Rect(pos.x - w / 2, pos.z - h / 2, w, h);
    }

    void InitRectQT() {
        rectQT = new Rect(0, 0, maxX, maxZ);
    }

    void clearPointColors() {
        foreach (PointDataStruct p in points) {
            p.go.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    void InsertPoints2QT(QuadClass qt) {
        foreach (PointDataStruct p in points)
        {
            qt.InsertData(p.x, p.z, p.index);
        }
    }

    void showResults(List<PointDataStruct>results) {
        foreach (PointDataStruct p in results) {
            points[p.index].go.GetComponent<Renderer>().material.color = Color.green;            
        } 
    }
	
    void InitPoints() {
        if (goParent != null) Destroy(goParent);
        points = new PointDataStruct[numPoints];
        goParent = new GameObject("parent points");
        for (int p = 0; p < numPoints; p++) {
            Vector3 pos = new Vector3(Random.Range(0, maxX), 0, Random.Range(0, maxZ));
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.transform.parent = goParent.transform;
            go.name = "point " + pos;
            go.transform.position = pos;
            points[p].x = pos.x;
            points[p].z = pos.z;
            points[p].index = p;
            points[p].go = go;
            points[p].yaw = Random.Range(0f, 360f);
            points[p].speed = Random.Range(.1f, .3f);
        }
    }

    void showFPS() {
        fps = cntFrames;
        cntFrames = 0;
    }
}
