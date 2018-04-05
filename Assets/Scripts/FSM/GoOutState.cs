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
        // 恢复AStar AI
   
        EnviromentManager.instance.ChangeDirectlight(0.4f);
        DogAI.instance.canMove = true;
        DogAI.instance.canSearch = true;
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
                IdleState idle = new IdleState(fsm);
                idle.isCanPatrol = true;
                fsm.PerformTransition(Transition.Back);
                break;
        }
        if (DogAI.instance.target == null)
        {
            DogPackage.instance.ShowPackage(true);
            fsm.PerformTransition(Transition.Back);
            DrawPath.instance.start = GameObject.FindGameObjectWithTag(Tags.player).transform;
            DrawPath.instance.end = GameObject.Find("DogInitiatePosition").transform;
            DrawPath.instance.ShowPath();
        }
    }
}
