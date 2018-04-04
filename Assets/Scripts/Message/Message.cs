using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message : MonoBehaviour {

    public static Message instance;
    private bool isShow = false;
    GameObject message_button;
    [HideInInspector]
    public string keycode;
   void Awake()
    {
        instance = this;
    }
    void Start()
    {
        message_button = GameObject.Find("message_button");
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
        message_button.SetActive(true);
    }
    void Show()
    {
        this.gameObject.SetActive(true);
        message_button.SetActive(false);
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
