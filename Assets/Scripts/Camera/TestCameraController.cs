using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraController : MonoBehaviour {	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            CameraController.Instance.SetCamera(new Vector3(-0.7736f, 3.40f, 8.88f));
        }
		
	}
}
