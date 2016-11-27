using System.Collections.Generic;
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

        if (GUILayout.Button("Load Folder"))
        {
            string folder = EditorUtility.OpenFolderPanel("Select obj sequence folder", "", "");
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
            _target.meshes = meshes.ToArray();
        }
    }

    static string[] FindObjs(string folder)
    {
        string[] paths = System.IO.Directory.GetFiles(folder, "*.obj");
        return paths.Select(path =>
        {
            // To relative path
            return "Assets" + path.Substring(Application.dataPath.Length);
        }).ToArray();
    }
}