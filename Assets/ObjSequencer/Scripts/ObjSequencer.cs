using UnityEngine;
using System.Collections;

public class ObjSequencer : MonoBehaviour
{
    [SerializeField]
    MeshRenderer target;

    [RangeAttribute(1f, 60f)]
    public float fps = 30f;

    public bool loop;
    public bool autoStart;

    public ObjClip clip;

    int frame;

    MeshFilter filter;

    void Start()
    {
        filter = target.GetComponent<MeshFilter>();
        if (autoStart)
        {
            Play();
        }
    }

    void OnValidate()
    {
        if (target == null)
        {
            target = GetComponent<MeshRenderer>();
        }
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
            if (frame >= clip.Count)
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
            clip.ApplyMesh(filter.mesh, frame);
            frame++;
            yield return new WaitForSeconds(1 / fps);
        }
    }
}