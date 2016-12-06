using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ObjSequencer))]
public class ObjSequencerEditor : Editor
{
    ObjSequencer _target;

    public void OnEnable()
    {
        _target = (ObjSequencer)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (Application.isPlaying)
        {
            return;
        }

        if (GUILayout.Button("Load sequence folder"))
        {
            string folder = EditorUtility.OpenFolderPanel("Select obj sequence folder", "", "");
            LoadFolder(folder);
        }
    }

    void LoadFolder(string folder)
    {
        Stopwatch sw = new Stopwatch();

        // Load Mesh
        sw.Start();
        var files = FindObjs(folder);
        var meshes = new List<Mesh>();
        foreach (var path in files)
        {
            var go = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            MeshFilter filter = go.GetComponentInChildren<MeshFilter>();
            if (filter)
            {
                Mesh mesh = filter.sharedMesh;
                mesh.RecalculateNormals();
                meshes.Add(mesh);
            }
        }
        sw.Stop();
        UnityEngine.Debug.LogFormat("Added meshes:{0} - time:{1}", meshes.Count, sw.Elapsed);

        // Create Clip
        sw.Reset();
        sw.Start();
        ObjClip clip = ObjClip.CreateFromMeshes(meshes);
        sw.Stop();
        UnityEngine.Debug.LogFormat("Create clip:{0} - time:{1}", clip, sw.Elapsed);

        // Create binary
        sw.Reset();
        sw.Start();

        if(!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }
        string binaryPath = Path.Combine(Application.streamingAssetsPath, Path.GetFileName(folder) + ".bytes");
        clip.SaveFramesToFile(binaryPath);
        sw.Stop();
        UnityEngine.Debug.LogFormat("Saved clip:{0} - time:{1}", binaryPath, sw.Elapsed);

        _target.FilePath = Path.GetFileName(binaryPath);
    }

    static string[] FindObjs(string folder)
    {
        string[] paths = System.IO.Directory.GetFiles(folder, "*.obj");
        return paths.Select(path => ToRelativePath(path)).ToArray();
    }

    static string ToRelativePath(string absPath)
    {
        return "Assets" + absPath.Substring(Application.dataPath.Length);
    }
}