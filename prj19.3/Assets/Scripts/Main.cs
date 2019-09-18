using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.DotsNetKit.NetCode;

public class Main : MonoBehaviour
{
    private bool showButton = true;

    void Start()
    {
        Screen.SetResolution(640, 480, false);
        SimpleConsole.Create();
    }

    void OnGUI()
    {
        if (!showButton)
            return;

        if (GUI.Button(new Rect(100, 100, 100, 50), "Start sever"))
        {
            Application.targetFrameRate = 60;
            SimpleServer.Start("myAppId");
            showButton = false;
        }

        if (GUI.Button(new Rect(100, 200, 100, 50), "Start client"))
        {
            Application.targetFrameRate = 60;
            SimpleClient.Connect("myAppId");
            showButton = false;
        }
    }
}
