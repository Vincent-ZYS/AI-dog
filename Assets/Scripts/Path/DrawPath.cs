using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class DrawPath : MonoBehaviour {

    public static DrawPath instance;
    public Transform start;

    /** Target point of paths */
    public Transform end;

    /** Offset from the real path to where it is rendered.
     * Used to avoid z-fighting
     */
    public Vector3 pathOffset;

    /** Material used for rendering paths */
    public Material lineMat;

    /** Material used for rendering result of the ConstantPath */
    public Material squareMat;
    public float lineWidth;

    public int searchLength = 1000;
    public int spread = 100;
    public float aimStrength = 0;

    Path lastPath = null;
    FloodPath lastFloodPath = null;

 public   List<GameObject> lastRender = new List<GameObject>();

    List<Vector3> multipoints = new List<Vector3>();
    void Awake()
    {
        instance = this;
    }
    public void ShowPath()
    {
        Path p = null;
        p = ABPath.Construct(start.position, end.position, OnPathComplete);

        if (p != null)
        {
            AstarPath.StartPath(p);
            lastPath = p;
        }
    }
    public void OnPathComplete(Path p)
    {
        // To prevent it from creating new GameObjects when the application is quitting when using multithreading.
        if (lastRender == null) return;

        ClearPrevious();

        if (p.error) return;

        GameObject ob = new GameObject("LineRenderer", typeof(LineRenderer));
        LineRenderer line = ob.GetComponent<LineRenderer>();
        line.sharedMaterial = lineMat;
#if UNITY_5_5_OR_NEWER
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = p.vectorPath.Count;
#else
			line.SetWidth(lineWidth, lineWidth);
			line.SetVertexCount(p.vectorPath.Count);
#endif

        for (int i = 0; i < p.vectorPath.Count; i++)
        {
            line.SetPosition(i, p.vectorPath[i] + pathOffset);
        }

        lastRender.Add(ob);
    }

    /** Destroys all previous render objects */
   public void ClearPrevious()
    {
        for (int i = 0; i < lastRender.Count; i++)
        {
            Destroy(lastRender[i]);
        }
        lastRender.Clear();
    }

    
}
