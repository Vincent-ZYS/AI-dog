using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CinematicEffects;
public class EnviromentManager : MonoBehaviour
{

    public static EnviromentManager instance;
    public Light dirctlight;
    public DepthOfField depth;
    void Awake()
    {
        instance = this;
    }
    public void ChangeDirectlight(float target)
    {
        dirctlight.intensity = Mathf.Lerp(dirctlight.intensity, target, 2.0f*Time.deltaTime);
    }
    public void ChangeDepthField(bool isEnable)
    {
        depth.enabled = isEnable;
    }
}
