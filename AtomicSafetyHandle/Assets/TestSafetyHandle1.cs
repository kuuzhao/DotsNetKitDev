using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using Unity.Networking.Transport;

public class TestSafetyHandle1 : MonoBehaviour
{
    DataStreamWriter m_MyDataStream;
    // Start is called before the first frame update
    void Start()
    {
        m_MyDataStream = new DataStreamWriter(1000, Allocator.Persistent);
    }

    // Update is called once per frame
    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 200, 50), "TestSafetyHandle"))
        {
            var myJob = new MyJob1
            {
                dataStream = m_MyDataStream
            };
            Debug.Log("TestSafetyHandle");
            m_MyDataStream.CheckValid();
            myJob.Schedule();
        }
    }
}

struct MyJob1 : IJob
{
    public DataStreamWriter dataStream;

    public void Execute()
    {
        Debug.Log("MyJob.Execute()");
        dataStream.CheckValid();
    }
}

