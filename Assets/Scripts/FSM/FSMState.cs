using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Transition
{
    NullTransition=0,
    Close,
    Open,
    SeePlayer,
    LostPlayer
}
public enum StateID
{
    NullStateID=0,
    Idle,
    Sleep,
    Patrol,
    Chase
}
public abstract class FSMState{
    protected StateID stateID;
    public StateID ID { get { return stateID; } }
    protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();
    protected FSMSystem fsm;

    public FSMState(FSMSystem fsm)
    {
        this.fsm = fsm;
    }


    public void AddTransition(Transition trans, StateID id)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("不允许NullTransition"); return;
        }
        if (id == StateID.NullStateID)
        {
            Debug.LogError("不允许NullStateID"); return;
        }
        if (map.ContainsKey(trans))
        {
            Debug.LogError("添加转换条件的时候，" + trans + "已经存在于map中"); return;
        }
        map.Add(trans, id);
    }
    public void DeleteTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("不允许NullTransition"); return;
        }
        if (map.ContainsKey(trans) == false)
        {
            Debug.LogError("删除转换条件的时候，" + trans + "不存在于map中"); return;
        }
        map.Remove(trans);
    }
    public StateID GetOutputState(Transition trans)
    {
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }
        return StateID.NullStateID;
    }

    public virtual void DoBeforeEntering() { }
    public virtual void DoAfterLeaving() { }
    public abstract void Act(GameObject npc);//npc对应的是你操纵的游戏物体
    public abstract void Reason(GameObject npc);//判断转换条件
}
