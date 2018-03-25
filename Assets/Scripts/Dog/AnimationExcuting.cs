using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationExcuting : MonoBehaviour {

    public static AnimationExcuting instance;
    public Animator anim;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

}
