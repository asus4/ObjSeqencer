using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using UnityEngine;

public class ObjClip : ScriptableObject, IEnumerable<ObjClip.Frame>
{
    #region Inner Class
    [Serializable]
    public class Frame
    {
        public Vector3[] vertices;
        public Vector2[] uv;
        public int[] triangles;
        public Vector3[] normals;
    }

    [Serializable]
    public class FrameSerializable
    {
        public float[] verticesX;
        public float[] verticesY;
        public float[] verticesZ;
        public float[] uvX;
        public float[] uvY;
        public int[] triangles;
        public float[] normalsX;
        public float[] normalsY;
        public float[] normalsZ;

        public Frame ToFrame()
        {
            var vertices = new Vector3[verticesX.Length];
            for (int i = 0; i < verticesX.Length; ++i)
            {
                vertices[i] = new Vector3(
                    verticesX[i],
                    verticesY[i],
                    verticesZ[i]
                );
            }
            var uv = new Vector2[uvX.Length];
            for (int i = 0; i < uvX.Length; ++i)
            {
                uv[i] = new Vector2(
                    uvX[i],
                    uvY[i]
                );
            }
            var normals = new Vector3[normalsX.Length];
            for (int i = 0; i < normalsX.Length; ++i)
            {
                normals[i] = new Vector3(
                    normalsX[i],
                    normalsY[i],
                    normalsZ[i]
                );
            }

            return new Frame()
            {
                vertices = vertices,
                uv = uv,
                triangles = triangles,
                normals = normals
            };
        }
        public static FrameSerializable ToSerializable(Frame frame)
        {
            var s = new FrameSerializable()
            {
                verticesX = frame.vertices.Select(v => v.x).ToArray(),
                verticesY = frame.vertices.Select(v => v.y).ToArray(),
                verticesZ = frame.vertices.Select(v => v.z).ToArray(),
                uvX = frame.uv.Select(v => v.x).ToArray(),
                uvY = frame.uv.Select(v => v.y).ToArray(),
                triangles = frame.triangles,
                normalsX = frame.normals.Select(v => v.x).ToArray(),
                normalsY = frame.normals.Select(v => v.y).ToArray(),
                normalsZ = frame.normals.Select(v => v.z).ToArray()
            };
            return s;
        }
    }
    #endregion // Inner Class

    public Frame[] frames;
    int lastFrame = -1;

    void OnDestroy()
    {
        frames = null;
    }

    public void ApplyMesh(Mesh mesh, int index)
    {
        Debug.AssertFormat(0 <= index && index < frames.Length, "Out of index :{0}", index );
        if (lastFrame.Equals(index)) return;

        var frame = frames[index];
        mesh.Clear();
        mesh.vertices = frame.vertices;
        mesh.uv = frame.uv;
        mesh.triangles = frame.triangles;
        mesh.normals = frame.normals;

        lastFrame = index;
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

    public static ObjClip LoadFramesFromFile(string path)
    {
        if (!Path.IsPathRooted(path))
        {
            path = Path.Combine(Application.streamingAssetsPath, path);
        }
        if (!File.Exists(path))
        {
            throw new Exception("File not found : " + path);
        }
        ObjClip clip = ScriptableObject.CreateInstance<ObjClip>();

        using (Stream file = File.Open(path, FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(file);
            var frames = obj as FrameSerializable[];
            clip.frames = frames.Select(f => f.ToFrame()).ToArray();
            file.Close();
        }
        return clip;
    }

    public void SaveFramesToFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        using (Stream file = File.Open(path, FileMode.Create))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, frames.Select(f => FrameSerializable.ToSerializable(f)).ToArray());
            file.Close();
        }
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
