using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState:FSMState {
    public bool isCanPatrol = false;
    public  IdleState(FSMSystem fsm):base(fsm)
    {
        stateID = StateID.Idle;

    }
    public override void Act(GameObject npc)
    {
        AnimationExcuting.instance.anim.SetBool("Sleep", false);
        AnimationExcuting.instance.anim.SetBool("Walk", false);
        AnimationExcuting.instance.anim.SetBool("Run", false);
    }

    public override void Reason(GameObject npc)
    {  if(isCanPatrol)
        {
            fsm.PerformTransition(Transition.SeePlayer);
        }
        string keycode = Message.instance.GetKeyCodes();
        switch(keycode)
        {
            case "关闭":
            case "再见":
            case "拜拜":
                SwitchClosing();
                break;
            case "超市":
                break;
            case "巡逻":
                fsm.PerformTransition(Transition.SeePlayer);
                break;            
        }
        if (DogAI.instance.target == null) return;
        if (DogAI.instance.target.tag==Tags.shop)
        {
            fsm.PerformTransition(Transition.Shopping);
        }
    }
    void SwitchClosing()
    {
        fsm.PerformTransition(Transition.Close);
    }

}
    