using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
1,第一步先添加State枚举
2,编写状态脚本
3,添加转换脚本的过度条件
4,添加其他脚本对新状态的转换条件
*/
public class DogAI : MonoBehaviour {
    private FSMSystem fsm;
    void Start()
    {
     
        InitFSM();
    }
    void InitFSM()
    {
        fsm = new FSMSystem();
        FSMState sleepState = new SleepState(fsm);
        sleepState.AddTransition(Transition.Open, StateID.Idle);


        FSMState IdleState = new IdleState(fsm);
        IdleState.AddTransition(Transition.Close, StateID.Sleep);
        IdleState.AddTransition(Transition.SeePlayer, StateID.Patrol);

        FSMState patrolState = new PatrolState(fsm);
        patrolState.AddTransition(Transition.Open, StateID.Idle);
        patrolState.AddTransition(Transition.SeePlayer, StateID.Chase);


        FSMState chaseState = new ChaseState(fsm);
        chaseState.AddTransition(Transition.LostPlayer, StateID.Patrol);
        chaseState.AddTransition(Transition.FindEnemy, StateID.Bark);

        FSMState barkState = new BarkState(fsm);
        barkState.AddTransition(Transition.LostPlayer, StateID.Patrol);

        fsm.AddState(sleepState);
        fsm.AddState(IdleState);
        fsm.AddState(patrolState);
        fsm.AddState(chaseState);
        fsm.AddState(barkState);

    }
    void Update()
    {
        fsm.Update(this.gameObject);
    }
  
}
