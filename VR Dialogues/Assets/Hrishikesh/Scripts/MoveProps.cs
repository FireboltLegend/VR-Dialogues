using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class MoveProps : MonoBehaviour
{
    [SerializeField] private PropData[] propDatas;
    public bool moveToPosition2;
    private Process pythonProcess;

    private void Update()
    {
        if (moveToPosition2)
        {
            for (int i = 0; i < propDatas.Length; i++)
                propDatas[i].prop.position = Vector3.Lerp(propDatas[i].prop.position, propDatas[i].position2.position, propDatas[i].speed2);
        }
        else
        {
            for (int i = 0; i < propDatas.Length; i++)
                propDatas[i].prop.position = Vector3.Lerp(propDatas[i].prop.position, propDatas[i].position1.position, propDatas[i].speed1);
        }
    }

    public void Toggle()
    {
        moveToPosition2 = !moveToPosition2;
        pythonProcess = GameObject.FindGameObjectWithTag("Finish").GetComponent<ChatbotManager>().ReturnPythonProcess();
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
            pythonProcess.Dispose();
        }

    }
}

[Serializable]
public class PropData
{
    public Transform prop;
    public Transform position1;
    public Transform position2;
    public float speed1 = 0.1f;
    public float speed2 = 0.1f;
}