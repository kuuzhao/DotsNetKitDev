using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;

public class TestSafetyHandle : MonoBehaviour
{
    MyDataStream m_MyDataStream;
    // Start is called before the first frame update
    void Start()
    {
        m_MyDataStream = new MyDataStream(Allocator.Persistent);
    }

    // Update is called once per frame
    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 400, 200, 50), "TestSafetyHandle"))
        {
            var myJob = new MyJob
            {
                dataStream = m_MyDataStream
            };
            Debug.Log("TestSafetyHandle");
            m_MyDataStream.CheckValid();
            myJob.Schedule();
        }
    }
}

struct MyJob : IJob
{
    public MyDataStream dataStream;

    public void Execute()
    {
        Debug.Log("MyJob.Execute()");
        dataStream.CheckValid();
    }
}

public unsafe struct MyDataStream
{
    int m_UniqueNumber;
    AtomicSafetyHandle m_Safety;
    [NativeSetClassTypeToNullOnSchedule] internal DisposeSentinel m_DisposeSentinel;

    public MyDataStream(Allocator allocator)
    {
        m_UniqueNumber = UnityEngine.Random.Range(1, 10000);
        DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 1, allocator);
    }
    public bool CheckValid()
    {
        string info = string.Format("uniqueNumber({0}), versionNode({1}), version({2})", m_UniqueNumber, m_Safety.versionNode.ToString(), m_Safety.version);
        AtomicSafetyHandle.GetWriterName(m_Safety);
        UnityEngine.Debug.Log(info);
        return true;
    }
}