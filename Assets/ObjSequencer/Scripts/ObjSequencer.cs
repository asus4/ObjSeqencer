using UnityEngine;
using System.Collections;

public class ObjSequencer : MonoBehaviour
{
    [SerializeField, Tooltip("Target that play models")]
    MeshRenderer target;

    [Range(1f, 60f), Tooltip("Frame rate")]
    public float fps = 30f;

    public bool loop;
    public bool autoStart;
    // This is int, because for controll from unity animation
    public float frame;
 
    [SerializeField]
    string filePath;

    MeshFilter filter;
    ObjClip clip;

    void Start()
    {
        filter = target.GetComponent<MeshFilter>();
        if (filter == null)
        {
            filter = target.gameObject.AddComponent<MeshFilter>();
        }

        clip = ObjClip.LoadFramesFromFile(filePath);

        if (autoStart)
        {
            Play();
        }
    }

    void OnValidate()
    {
        if (target != null) return;
        target = GetComponent<MeshRenderer>();
        if (target != null) return;
        target = gameObject.AddComponent<MeshRenderer>();
    }

    void Update()
    {
        clip.ApplyMesh(filter.mesh, (int)frame);
    }

    public void Play(int start = 0)
    {
        Stop();
        frame = start;
        StartCoroutine(_Play());
    }

    public void Stop()
    {
        StopCoroutine(_Play());
    }

    IEnumerator _Play()
    {
        while (Application.isPlaying)
        {
            if (frame >= clip.Count - 1)
            {
                if (loop)
                {
                    frame = 0;
                }
                else
                {
                    yield break;
                }
            }
            frame++;
            yield return new WaitForSeconds(1 / fps);
        }
    }

    public string FilePath
    {
        get { return filePath; }
        set { filePath = value; }
    }
}