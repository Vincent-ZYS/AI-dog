using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState:FSMState {

    private List<Transform> path = new List<Transform>();
    private int index = 0;
    private Transform playerTransform;
   
    [Header("指定初始位置")]
    public Transform initalPosition;
    public PatrolState(FSMSystem fsm) : base(fsm)
    {
        stateID = StateID.Patrol;
        initalPosition = GameObject.Find("DogInitiatePosition").transform;
        Transform pathTransform = GameObject.Find("Paths").transform;
       
        Transform[] children = pathTransform.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != pathTransform)
            {
                path.Add(child);
            }
        }
     

    }


    public override void Act(GameObject npc)
    {

        CameraController.Instance.SetCamera(new Vector3(6f, 47f, 95f));
        npc.transform.LookAt(path[index].position);
        Vector3 offset = path[index].transform.position;
        offset.y = npc.transform.position.y;
        npc.transform.Translate(Vector3.forward * Time.deltaTime * 10);
        AnimationExcuting.instance.anim.SetBool("Walk", true);
        AnimationExcuting.instance.anim.SetBool("Bark", false);
        AnimationExcuting.instance.anim.SetBool("Run", false);
        if (Vector3.Distance(npc.transform.position, path[index].position) < 1)
        {
            index++;
            index %= path.Count;
        }
    }

    public override void Reason(GameObject npc)
    {
        string keycode = Message.instance.GetKeyCodes();
        switch(keycode)
        {
            case "过来":
            case "坐下":
               ComebackInitalPosition();
                break;
            case "超市":
                ComebackInitalPosition(3);
                break;

        }
        if (GameObject.FindWithTag(Tags.enemy) == null) return;
        if (Vector3.Distance(npc.transform.position, GameObject.FindGameObjectWithTag(Tags.enemy).transform.position) <= 30.0f)
        {
            fsm.PerformTransition(Transition.SeePlayer);
        }
    }
  void  ComebackInitalPosition(float speed=3)
    {
        Debug.Log("speed::" + speed);
        while (Vector3.Distance(GameObject.FindGameObjectWithTag(Tags.player).transform.position, initalPosition.position) > 1.0f)
        {
            GameObject.FindGameObjectWithTag(Tags.player).transform.Translate(Vector3.forward * Time.deltaTime * speed);
            GameObject.FindGameObjectWithTag(Tags.player).transform.LookAt(initalPosition.position);
            AnimationExcuting.instance.anim.SetBool("Walk", true);
            AnimationExcuting.instance.anim.SetBool("Bark", false);
        }
       
        CameraController.Instance.SetCamera(new Vector3(5,9.6f,36.2f));
        GameObject.FindGameObjectWithTag(Tags.player).transform.forward = initalPosition.transform.forward;
        fsm.PerformTransition(Transition.Open);

    }
}
