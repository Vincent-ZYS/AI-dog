using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding.Examples;
public class IdleState:FSMState {
    public bool isCanPatrol = false;
    public AstarSmoothFollow2 followScript;
    public  IdleState(FSMSystem fsm):base(fsm)
    {
        stateID = StateID.Idle;
        followScript = GameObject.Find("Main Camera").GetComponent<AstarSmoothFollow2>();
    }
    public override void Act(GameObject npc)
    {
        followScript.distance = 11;
        followScript.horzintal = 0;
        DogPackage.instance.ShowPackage(false);
        if (DrawPath.instance.lastRender != null && DrawPath.instance.lastRender.Count > 0)
        {
            DrawPath.instance.ClearPrevious();
        }
        EnviromentManager.instance.ChangeDirectlight(2.5f);
        EnviromentManager.instance.ChangeDepthField(true);
        GameObject.Find("Main Camera").GetComponent<DrawPath>().enabled = false;
        AnimationExcuting.instance.anim.SetBool("Sleep", false);
        AnimationExcuting.instance.anim.SetBool("Walk", false);
        AnimationExcuting.instance.anim.SetBool("Run", false);
    }

    public override void Reason(GameObject npc)
    {  if(isCanPatrol)
        {
            EnviromentManager.instance.ChangeDepthField(false);
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
                EnviromentManager.instance.ChangeDepthField(false);
                fsm.PerformTransition(Transition.SeePlayer);
                break;            
        }
        if (DogAI.instance.target == null) return;
        if (DogAI.instance.target.tag==Tags.shop)
        {
            EnviromentManager.instance.ChangeDepthField(false);
            fsm.PerformTransition(Transition.Shopping);
        }
    }
    void SwitchClosing()
    {
        fsm.PerformTransition(Transition.Close);
    }

}
    