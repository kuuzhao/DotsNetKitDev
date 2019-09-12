using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class Main : MonoBehaviour
{
    private bool showButton = true;

    void Start()
    {
        Screen.SetResolution(640, 480, false);
    }

    void OnGUI()
    {
        if (!showButton)
            return;

        if (GUI.Button(new Rect(100, 100, 100, 50), "Start sever"))
        {
            Server.Start();
            showButton = false;
        }

        if (GUI.Button(new Rect(100, 200, 100, 50), "Start client"))
        {
            Client.Start();
            showButton = false;
        }
    }
}
