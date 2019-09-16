using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    static Console instance;
    public Text text;
    List<string> lines;
    void Start()
    {
        instance = this;
        lines = new List<string>();
        DontDestroyOnLoad(transform.root);

        WriteLine("F1 to toggle the console");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            text.gameObject.SetActive(!text.gameObject.activeSelf);
        }
    }

    // Update is called once per frame
    public static void WriteLine(string line)
    {
        instance.lines.Add(line);
        if (instance.lines.Count > 10)
            instance.lines.RemoveAt(1);
        instance.text.text = instance.lines.Aggregate((i, j) => i + "\n" + j);
    }

    public static void SetColor(Color color)
    {
        instance.text.color = color;
    }
}
