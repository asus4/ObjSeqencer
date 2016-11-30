using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjClip : ScriptableObject, IEnumerable<ObjClip.Frame>
{
    [Serializable]
    public class Frame
    {
        public Vector3[] vertices;
        public Vector2[] uv;
        public int[] triangles;
        public Vector3[] normals;
    }

    public Frame[] frames;

    public void ApplyMesh(Mesh mesh, int index)
    {
        var frame = frames[index];
        mesh.Clear();
        mesh.vertices = frame.vertices;
        mesh.uv = frame.uv;
        mesh.triangles = frame.triangles;
        mesh.normals = frame.normals;
    }

    public static ObjClip CreateFromMeshes(IEnumerable<Mesh> meshes)
    {
        var clip = CreateInstance<ObjClip>();
        var frames = new List<Frame>();
        foreach (Mesh mesh in meshes)
        {
            mesh.RecalculateNormals();
            frames.Add(new Frame()
            {
                vertices = mesh.vertices,
                uv = mesh.uv,
                triangles = mesh.triangles,
                normals = mesh.normals
            });
        }
        clip.frames = frames.ToArray();
        return clip;
    }

    public IEnumerator<Frame> GetEnumerator()
    {
        foreach (var frame in frames)
        {
            yield return frame;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Frame this[int index] { get { return frames[index]; } }
    public int Count { get { return frames.Length; } }
}
