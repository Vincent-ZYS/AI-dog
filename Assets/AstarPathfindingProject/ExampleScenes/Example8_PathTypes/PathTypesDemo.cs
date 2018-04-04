using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding.Examples {
	/** Demos different path types.
	 * This script is an example script demoing a number of different path types included in the project.
	 * Since only the Pro version has access to many path types, it is only included in the pro version
	 * \astarpro
	 *
	 * \see Pathfinding.ABPath
	 * \see Pathfinding.MultiTargetPath
	 * \see Pathfinding.ConstantPath
	 * \see Pathfinding.FleePath
	 * \see Pathfinding.RandomPath
	 * \see Pathfinding.FloodPath
	 * \see Pathfinding.FloodPathTracer
	 */
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_path_types_demo.php")]
	public class PathTypesDemo : MonoBehaviour {
		public DemoMode activeDemo = DemoMode.ABPath;
		public enum DemoMode {
			ABPath,
			MultiTargetPath,
			RandomPath,
			FleePath,
			ConstantPath,
			FloodPath,
			FloodPathTracer
		}

		/** Start of paths */
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

		List<GameObject> lastRender = new List<GameObject>();

		List<Vector3> multipoints = new List<Vector3>();

		// Update is called once per frame
		void Start () {    
          
            DemoPath();
		}

		/** Will be called when the paths have been calculated */
		public void OnPathComplete (Path p) {
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

			for (int i = 0; i < p.vectorPath.Count; i++) {
				line.SetPosition(i, p.vectorPath[i] + pathOffset);
			}

			lastRender.Add(ob);
		}

		/** Destroys all previous render objects */
		void ClearPrevious () {
			for (int i = 0; i < lastRender.Count; i++) {
				Destroy(lastRender[i]);
			}
			lastRender.Clear();
		}

		/** Clears renders when the object is destroyed */
		void OnDestroy () {
			ClearPrevious();
			lastRender = null;
		}

		/** Starts a path specified by PathTypesDemo.activeDemo */
		void DemoPath () {
			Path p = null;

			switch (activeDemo) {
			case DemoMode.ABPath:
				p = ABPath.Construct(start.position, end.position,OnPathComplete);
				break;
			}

			if (p != null) {
				AstarPath.StartPath(p);
				lastPath = p;
			}
		}

	}
}
