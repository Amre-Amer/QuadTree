using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree : MonoBehaviour {
    int maxX = 100;
    int maxZ = 100;
    public int numCells = 5;
    CellClass[] cells;
    int cntFrames;
    public int fps;
    public int checks;
    int checks0;
    public bool useQuadTree;
    GameObject goRange;

	// Use this for initialization
	void Start () {
        initCells();
        initQT();
//        UpdateCells();
        InvokeRepeating("showFPS", 1, 1);
	}

    void initQT() {
        QT qt = new QT(4, new Vector2(0, 0), maxX, maxZ);
        for (int c = 0; c < numCells; c++)
        {
            Vector2 pos = new Vector2(cells[c].go.transform.position.x, cells[c].go.transform.position.z);
            qt.InsertData(pos, c);
        }
        List<PointStruct> result = new List<PointStruct>();
//        Rect range = new Rect(0, 0, maxX/2, maxZ/2); 
        Rect range = new Rect(0, 0, maxX * .45f, maxZ * .45f);
        updateRange(range);
        qt.GetPointsFromRange(result, range);
        foreach (PointStruct p in result) {
            Debug.Log("result:range:" + range + " point:" + p.pos + " " + p.dataIndex + "\n");
            cells[p.dataIndex].go.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void updateRange(Rect rect) {
        goRange = GameObject.CreatePrimitive(PrimitiveType.Cube);
        goRange.transform.position = new Vector3(rect.center.x, 0, rect.center.y);
        goRange.transform.localScale = new Vector3(rect.size.x, .25f, rect.size.y);
        //MakeMaterialTransparent(goRect.GetComponent<Renderer>().material);
        goRange.GetComponent<Renderer>().material.color = Color.cyan;
        goRange.name = "range:" + goRange.transform.position + " " + goRange.transform.localScale;
    }
	
	// Update is called once per frame
	void UpdateX () {
//        initQT();
        UpdateCells();
        cntFrames++;
	}

    void showFPS()
    {
        fps = cntFrames;
        cntFrames = 0;
    }

    void initCells() {
        cells = new CellClass[numCells];
        for (int c = 0; c < numCells; c++)
        {
            float yaw = Random.Range(0, 360);
            float speed = Random.Range(.01f/2, .05f/2);
            Vector3 pos = new Vector3(Random.Range(0, maxX), 0, Random.Range(0, maxZ));
            cells[c] = new CellClass(pos, yaw, speed);
        }
    }

    void UpdateCells() {
        checks0 = 0;
        for (int c = 0; c < numCells; c++) {
            UpdateCell(c);            
        }
        checks = checks0;
    }

    void UpdateCell(int checkC) {
        GameObject go = cells[checkC].go;
        for (int c = 0; c < numCells; c++)
        {
            if (c != checkC) {
                checks0++;
                if (cells[c].IsNear(go))
                {
                    //cells[c].TurnAround();
                }
            }
            cells[c].MoveForward();
        }
    }
}
