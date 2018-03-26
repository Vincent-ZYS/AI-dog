using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        fsm.AddState(sleepState);
        fsm.AddState(IdleState);
        fsm.AddState(patrolState);
        fsm.AddState(chaseState);

    }
    void Update()
    {
        fsm.Update(this.gameObject);
    }
  
}
