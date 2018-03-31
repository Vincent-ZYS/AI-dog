using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding.Examples;
public class ComeBackState : FSMState
{
    private Transform initialPosition;
    public ComeBackState(FSMSystem fsm) : base(fsm)
    {
        stateID = StateID.ComeBack;
        initialPosition= GameObject.Find("DogInitiatePosition").transform;
    }
    public override void Act(GameObject npc)
    {
        DogAI.instance.target = initialPosition;
        AnimationExcuting.instance.anim.SetBool("Run", true);
    }

    public override void Reason(GameObject npc)
    {
        if (Vector3.Distance(npc.transform.position, initialPosition.position) <=7.0f)
        {
            fsm.PerformTransition(Transition.Open);
            //npc.transform.LookAt(-initialPosition.forward);
            //CameraController.Instance.SetCamera(new Vector3(5, 9.6f, 36.2f));
            npc.transform.forward = -initialPosition.forward;
            npc.transform.rotation = Quaternion.identity;
            CameraController.Instance.gameObject.GetComponent<AstarSmoothFollow2>().enabled = false;
            Debug.Log("到达");
        }
        
    }
}
