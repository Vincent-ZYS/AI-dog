using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding.Examples;
public class GoOutState : FSMState
{
    public  GoOutState(FSMSystem fsm):base(fsm)
    {
        stateID = StateID.GoOut;
    }
    public override void Act(GameObject npc)
    {
        CameraController.Instance.gameObject.GetComponent<AstarSmoothFollow2>().enabled = true;
        AnimationExcuting.instance.anim.SetBool("Run", true);
    }

    public override void Reason(GameObject npc)
    {
  
        string keycode = Message.instance.GetKeyCodes();
        switch (keycode)
        {
            case "回来":
                fsm.PerformTransition(Transition.Back);
                break;
            case "巡逻":
                DogAI.instance.target = GameObject.Find("DogInitiatePosition").transform;
                if (Vector3.Distance(npc.transform.position, GameObject.Find("DogInitiatePosition").transform.position) <= 1.0f)
                {
                    fsm.PerformTransition(Transition.SeePlayer);
                }                   
               break;
        }
        if (DogAI.instance.target == null)
            fsm.PerformTransition(Transition.Back);
    }
}
