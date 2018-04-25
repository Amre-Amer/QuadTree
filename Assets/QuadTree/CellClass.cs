using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellClass
{
    float scaCell = 5;
    public float yaw;
    public float speed;
    public GameObject go;
    public CellClass(Vector3 pos, float yaw0, float speed0)
    {
        go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.transform.localScale = new Vector3(scaCell, .3f, scaCell);
        go.transform.position = pos;
        yaw = yaw0;
        go.transform.eulerAngles = new Vector3(scaCell, yaw, scaCell);
        go.name = "point:" + go.transform.position;
        speed = speed0;
    }

    public void MoveForward()
    {
        go.transform.position += go.transform.forward * speed;
        CheckBounce();
    }

    public void TurnAround()
    {
        yaw = go.transform.eulerAngles.y;
        yaw += 180 - Random.Range(-45f, 45f);
        go.transform.eulerAngles = new Vector3(0, yaw, 0);
    }

    public bool IsNear(GameObject goCheck)
    {
        bool result = false;
        float distNear = (scaCell + goCheck.transform.localScale.x) / 2;
        Vector3 posCheck = goCheck.transform.position;
        float dist = Vector3.Distance(go.transform.position, posCheck);
        if (dist <= distNear)
        {
            result = true;
        }
        return result;
    }

    void CheckBounce()
    {
        Vector3 pos = go.transform.position;
        if (Vector3.Distance(Vector3.zero, pos) > 100)
        {
            go.transform.LookAt(Vector3.zero);
            yaw = go.transform.eulerAngles.y;
        }
    }
}

