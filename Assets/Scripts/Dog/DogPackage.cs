using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPackage : MonoBehaviour {

    public static DogPackage instance;
    public GameObject package;
    void Awake()
    {
        instance = this;
    }
    public void ShowPackage(bool isShow)
    {
        package.SetActive(isShow);
    }
}
