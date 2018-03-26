using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepState :FSMState {
    public  SleepState(FSMSystem fsm):base(fsm)
    {
        stateID = StateID.Sleep;

    }
    public override void Act(GameObject npc)
    {
        AnimationExcuting.instance.anim.SetBool("Sleep", true);
    }

    public override void Reason(GameObject npc)
    {
        string keycode = Message.instance.GetKeyCodes();
        switch (keycode)
        {
            case "你好":
            case "启动":
            case "小狗同学":
                fsm.PerformTransition(Transition.Open);
                break;
            case "巡逻":
                fsm.PerformTransition(Transition.SeePlayer);
                break;
        }
    }
}
