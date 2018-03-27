using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderMap : MonoBehaviour {
    // Update is called once per frame
    public Transform miniMap;
	void Update () {
        if (GameObject.Find("MiniMapCamera")== null)
            return;
#if UNITY_EDITOR
        if(Input.GetMouseButton(0))
        {
           
           if(Input.GetAxis("Mouse X")!=0)
            {
                miniMap.transform.Translate(transform.right *- Input.GetAxis("Mouse X") * 5);
               
            }
            if (Input.GetAxis("Mouse Y") != 0)
            {
                miniMap.transform.Translate(transform.up * -Input.GetAxis("Mouse Y") * 5);
                
            }
        }
#endif

    }
}
