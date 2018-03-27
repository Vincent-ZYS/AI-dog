using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformState : MonoBehaviour {
    //星号量来标记商店界面是否打开
    private bool isShowShoppingMall = false;
    public static TransformState instance;
    void Awake()
    {
        instance = this;
    }
	public void MessageButtonClick()
    {
        Message.instance.TransformState();

    }
    public void ShoppingState()
    {
        if(isShowShoppingMall)
        {
            isShowShoppingMall = false;
            GameObject.Find("UICanvas").transform.Find("ShoppingMall").gameObject.SetActive(false);
        }
        else
        {
            isShowShoppingMall = true;
            GameObject.Find("UICanvas").transform.Find("ShoppingMall").gameObject.SetActive(true);
        }
    }
}
