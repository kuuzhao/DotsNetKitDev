using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

#if UNITY_EDITOR
class BatchModePlayer
{
    [MenuItem("DotsNetKit/TestRunPlayer")]
    static void RunPlayer()
    {
        var editorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
        foreach (var editorWindow in editorWindows)
        {
            var editorWindowName = editorWindow.ToString();
            if (!editorWindowName.Contains("GameView"))
                editorWindow.Close();
            else
                Debug.Log(editorWindowName);
        }

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var go = new GameObject("LoadMain");
        go.AddComponent<LoadMainProxy>();

        EditorApplication.isPlaying = true;
    }

    [MenuItem("DotsNetKit/Run Player/1")]
    static void Run1Player()
    {
        CreateHardLinkIfNeeded();
#if UNITY_EDITOR_WIN
        DoRunPlayer(1);
#endif
    }
    [MenuItem("DotsNetKit/Run Player/2")]
    static void Run2Players()
    {
        CreateHardLinkIfNeeded();
#if UNITY_EDITOR_WIN
        for (int i = 1; i <= 2; ++i)
            DoRunPlayer(i);
#endif
    }
    [MenuItem("DotsNetKit/Run Player/3")]
    static void Run3Players()
    {
        CreateHardLinkIfNeeded();
#if UNITY_EDITOR_WIN
        for (int i = 1; i <= 3; ++i)
            DoRunPlayer(i);
#endif
    }
    [MenuItem("DotsNetKit/Run Player/4")]
    static void Run4Players()
    {
        CreateHardLinkIfNeeded();
#if UNITY_EDITOR_WIN
        for (int i = 1; i <= 4; ++i)
            DoRunPlayer(i);
#endif
    }

    // The range of i is [1, 4]
    static void DoRunPlayer(int i)
    {
        string prjPath = Directory.GetParent(Application.dataPath).FullName;
        string playerPrjRoot = Path.Combine(prjPath, "DotsNetKitPlayers");
        string playerPrjPath = Path.Combine(playerPrjRoot, string.Format("Player{0}", i));

        string unityArgs = string.Format("-disable-assembly-updater -projectPath {0} -executeMethod BatchModePlayer.RunPlayer", playerPrjPath);

#if true
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        startInfo.FileName = EditorApplication.applicationPath;
        startInfo.Arguments = unityArgs;
        process.StartInfo = startInfo;
        process.Start();
#else
        Debug.Log(unityArgs);
#endif
    }

    // [MenuItem("DotsNetKit/Run Clients/CreateLink")]
    static void CreateHardLinkIfNeeded()
    {
        string prjPath = Directory.GetParent(Application.dataPath).FullName;
        string playerPrjRoot = Path.Combine(prjPath, "DotsNetKitPlayers");
        for (int i = 1; i <= 4; ++i)
        {
            string playerPrjName = string.Format("Player{0}", i);
            string playerPrjPath = Path.Combine(playerPrjRoot, playerPrjName);

#if UNITY_EDITOR_WIN
            MakeHardLinedPrj(playerPrjPath);
#endif
        }
    }

#if UNITY_EDITOR_WIN
    static void MakeHardLinedPrj(string playerPrjPath)
    {
        Directory.CreateDirectory(playerPrjPath);
        // remove existing links
        string linkToAssets = Path.Combine(playerPrjPath, "Assets");
        string linkToLibrary = Path.Combine(playerPrjPath, "Library");
        string linkToPackages = Path.Combine(playerPrjPath, "Packages");
        string linkToProjectSettings = Path.Combine(playerPrjPath, "ProjectSettings");
        string linkToAutoBuild = Path.Combine(playerPrjPath, "AutoBuild");

        ExecuteCommand("rmdir " + linkToAssets);
        ExecuteCommand("rmdir " + linkToLibrary);
        ExecuteCommand("rmdir " + linkToPackages);
        ExecuteCommand("rmdir " + linkToProjectSettings);
        ExecuteCommand("rmdir " + linkToAutoBuild);

        string prjPath = Directory.GetParent(Application.dataPath).FullName;
        string targetAssets = Path.Combine(prjPath, "Assets");
        string targetLibrary = Path.Combine(prjPath, "Library");
        string targetPackages = Path.Combine(prjPath, "Packages");
        string targetProjectSettings = Path.Combine(prjPath, "ProjectSettings");
        string targetAutoBuild = Path.Combine(prjPath, "AutoBuild");

        ExecuteCommand(string.Format("mklink /J {0} {1}", linkToAssets, targetAssets));
        ExecuteCommand(string.Format("mklink /J {0} {1}", linkToLibrary, targetLibrary));
        ExecuteCommand(string.Format("mklink /J {0} {1}", linkToPackages, targetPackages));
        ExecuteCommand(string.Format("mklink /J {0} {1}", linkToProjectSettings, targetProjectSettings));
        ExecuteCommand(string.Format("mklink /J {0} {1}", linkToAutoBuild, targetAutoBuild));
    }

    static void ExecuteCommand(string cmd)
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = string.Format("/C {0}", cmd);
        process.StartInfo = startInfo;
        process.Start();
    }
#endif
}


#endif