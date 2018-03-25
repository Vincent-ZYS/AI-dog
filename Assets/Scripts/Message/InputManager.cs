﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InputManager : MonoBehaviour {
    public GameObject masterMessage;
    private Transform VerticalLayout;
    public static InputManager instance;
    private Text inputMessage;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        VerticalLayout = GameObject.Find("UICanvas").transform.Find("Message/Scroll View/VerticalLayout");
        inputMessage = GameObject.Find("UICanvas").transform.Find("Message/InputBg/InputField/Text").gameObject.GetComponent<Text>();
    }
  public void SendMessageClick()
    {
        if (inputMessage.text == "") return;
        GameObject master_message = GameObject.Instantiate(masterMessage);
        master_message.transform.SetParent(VerticalLayout);
        master_message.transform.Find("message_text").gameObject.GetComponent<Text>().text = inputMessage.text;
        inputMessage.gameObject.transform.parent.gameObject.GetComponent<InputField>().text = "";
       
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SendMessageClick();
        }
    }
    
 
}