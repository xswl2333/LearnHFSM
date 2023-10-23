using FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCState
{
    IDLE,
    WALK,
    REST,
}

public class NpcStateMachine : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float idleTime;
    [SerializeField] private float restTime;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private SpriteRenderer restZone;

    private StateMachine<NPCState> fsm;

    private int currentIndex = 0;
    private void Start()
    {

        fsm=new StateMachine<NPCState>();
        fsm.AddState(NPCState.IDLE,new Idle(animator,idleTime,true));

        fsm.AddState(NPCState.WALK, new Walk(animator, waypoints,
            transform, OnCurrentIndexChanged, speed, false));//不需要等待，不需要退出时间
        fsm.AddState(NPCState.REST, new Rest(animator, restTime, true));

        fsm.AddTransition(NPCState.IDLE,NPCState.WALK);
        fsm.AddTransition(NPCState.REST, NPCState.WALK);
        
        fsm.AddTransition(NPCState.WALK, NPCState.REST, transition => Vector2.Distance(transform.position, waypoints[currentIndex].position) < 0.1f &&
        restZone.bounds.Contains(transform.position));//条件更加苛刻，放在前面判断

        fsm.AddTransition(NPCState.WALK,NPCState.IDLE,
            transition=> Vector2.Distance(transform.position, waypoints[currentIndex].position) < 0.1f);

        fsm.Init();

    }

    private void Update()
    {

        fsm.OnLogic();
    }

    private void OnCurrentIndexChanged(int newIndex)
    {
        currentIndex = newIndex;//委托
    }
}





