using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkState : FSMState {
    public BarkState(FSMSystem fsm) : base(fsm)
    {
        stateID = StateID.Bark;
       
    }

    public override void Act(GameObject npc)
    {
        AnimationExcuting.instance.anim.SetBool("Bark", true);
        AnimationExcuting.instance.anim.SetBool("Walk", false);
    }

    public override void Reason(GameObject npc)
    {
        if(GameObject.FindGameObjectWithTag(Tags.enemy)==null)
        {
            fsm.PerformTransition(Transition.LostPlayer);
        }
        else if (Vector3.Distance(GameObject.FindGameObjectWithTag(Tags.enemy).transform.position, npc.transform.position) > 60)
        {
            fsm.PerformTransition(Transition.LostPlayer);
        }

    }

}
