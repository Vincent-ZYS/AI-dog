using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
1,第一步先添加State枚举
2,编写状态脚本
3,添加转换脚本的过度条件
4,添加其他脚本对新状态的转换条件
*/
public class DogAI : AIPath {
    public static DogAI instance;
    private FSMSystem fsm;
    /** 移动的最小距离 */
    public float sleepVelocity = 0.4F;
    new void Awake()
    {
        instance = this;
        base.Awake();
    }
    new void Start()
    {
        InitFSM();
        base.Start();
    }
    void InitFSM()
    {
        fsm = new FSMSystem();
        FSMState sleepState = new SleepState(fsm);
        sleepState.AddTransition(Transition.Open, StateID.Idle);


        FSMState IdleState = new IdleState(fsm);
        IdleState.AddTransition(Transition.Close, StateID.Sleep);
        IdleState.AddTransition(Transition.SeePlayer, StateID.Patrol);
        IdleState.AddTransition(Transition.Shopping, StateID.GoOut);

        FSMState patrolState = new PatrolState(fsm);
        patrolState.AddTransition(Transition.Open, StateID.Idle);
        patrolState.AddTransition(Transition.SeePlayer, StateID.Chase);


        FSMState chaseState = new ChaseState(fsm);
        chaseState.AddTransition(Transition.LostPlayer, StateID.Patrol);
        chaseState.AddTransition(Transition.FindEnemy, StateID.Bark);

        FSMState barkState = new BarkState(fsm);
        barkState.AddTransition(Transition.LostPlayer, StateID.Patrol);

        FSMState gooutState = new GoOutState(fsm);
        gooutState.AddTransition(Transition.Back, StateID.ComeBack);
        gooutState.AddTransition(Transition.SeePlayer, StateID.Patrol);

        FSMState comebackState = new ComeBackState(fsm);
        comebackState.AddTransition(Transition.Open, StateID.Idle);

        fsm.AddState(sleepState);
        fsm.AddState(IdleState);
        fsm.AddState(patrolState);
        fsm.AddState(chaseState);
        fsm.AddState(barkState);
        fsm.AddState(comebackState);
        fsm.AddState(gooutState);

    }
    protected override void Update()
    {
        base.Update();
        fsm.Update(this.gameObject);
    }
    /**
     * Called when the end of path has been reached.
     * An effect (#endOfPathEffect) is spawned when this function is called
     * However, since paths are recalculated quite often, we only spawn the effect
     * when the current position is some distance away from the previous spawn-point
     */
    public override void OnTargetReached()
    {
        //TODO 到目的地所要的逻辑
        target = null;

    }


}
