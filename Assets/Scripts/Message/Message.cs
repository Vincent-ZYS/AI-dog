using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message : MonoBehaviour {

    public static Message instance;
    private bool isShow = false;
    [HideInInspector]
    public string keycode;
   void Awake()
    {
        instance = this;
    }
    void Start()
    {
        this.gameObject.SetActive(false);
    }
    public void TransformState()
    {
        if(isShow)
        {
            Hide();
            isShow = false;

        }
        else
        {
            isShow = true;
            Show();
        }
    }
    void Hide()
    {
        this.gameObject.SetActive(false);
    }
    void Show()
    {
        this.gameObject.SetActive(true);
    }
    public void SetKeyCodes(string keycode)
    {
        this.keycode = keycode;
    }
    public string GetKeyCodes()
    {
        return keycode;
    }
}
