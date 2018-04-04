using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestPath : MonoBehaviour {
    GameObject[] shop;
    Transform pet;
	void Start()
    {
        shop = GameObject.FindGameObjectsWithTag(Tags.shop);
        pet = GameObject.FindGameObjectWithTag(Tags.player).transform;
    }
    public void BestPathClick()
    {
        GameObject temp = shop[0];
        for(int i=1;i<shop.Length;i++)
        {
            if(Vector3.Distance(pet.position,shop[i].transform.position)< Vector3.Distance(pet.position,temp.transform.position))
            {
                temp = shop[i];
            }
        }
        DrawPath.instance.end = temp.transform;
        DrawPath.instance.start = pet.transform;
        DrawPath.instance.ShowPath();
        DogAI.instance.target = temp.transform;
       
    }
}
