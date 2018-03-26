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

        fsm.AddState(sleepState);
        fsm.AddState(IdleState);
        fsm.AddState(patrolState);
       
    }
    void Update()
    {
        fsm.Update(this.gameObject);
    }
  
}
