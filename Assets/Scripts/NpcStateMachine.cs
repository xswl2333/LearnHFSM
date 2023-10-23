using FSM;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum NPCState
{
    IDLE,
    WALK,
    REST,
}

public enum SuperState
{
    NULL,
    DAY,
    NIGHT,

}

public class NpcStateMachine : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float idleTime;
    [SerializeField] private float restTime;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private SpriteRenderer restZone;

    //�ֲ�����״̬
    private StateMachine<SuperState, string> fsm;
    private StateMachine<SuperState,NPCState,string> NightSuperState;
    private StateMachine<SuperState,NPCState,string> DaySuperState;

    private int currentIndex = 0;

    private void Start()
    {
        //�����ʼ��
        DaySuperState = new StateMachine<SuperState, NPCState, string>();
        DaySuperState.AddState(NPCState.IDLE, new Idle(animator, idleTime, true));
        DaySuperState.AddState(NPCState.WALK, new Walk(animator, waypoints,
            transform, OnCurrentIndexChanged, speed, false));//����Ҫ�ȴ�������Ҫ�˳�ʱ

        DaySuperState.AddTransition(NPCState.WALK, NPCState.IDLE,
          transition => Vector2.Distance(transform.position, waypoints[currentIndex].position) < 0.1f);

        DaySuperState.AddTransition(NPCState.IDLE, NPCState.WALK);

        //ҹ���ʼ��
        NightSuperState = new StateMachine<SuperState, NPCState, string>();
        NightSuperState.AddState(NPCState.IDLE, new Idle(animator, idleTime, true));

        NightSuperState.AddState(NPCState.WALK, new Walk(animator, waypoints,
            transform, OnCurrentIndexChanged, speed, false));//����Ҫ�ȴ�������Ҫ�˳�ʱ��
        NightSuperState.AddState(NPCState.REST, new Rest(animator, restTime, true));

        NightSuperState.AddTransition(NPCState.IDLE, NPCState.WALK);
        NightSuperState.AddTransition(NPCState.REST, NPCState.WALK);

        NightSuperState.AddTransition(NPCState.WALK, NPCState.REST, transition => Vector2.Distance(transform.position, waypoints[currentIndex].position) < 0.1f &&
        restZone.bounds.Contains(transform.position));//�������ӿ��̣�����ǰ���ж�

        NightSuperState.AddTransition(NPCState.WALK, NPCState.IDLE,
            transition => Vector2.Distance(transform.position, waypoints[currentIndex].position) < 0.1f);

        NightSuperState.Init();
        //��ҹ�л�
        // fsm init
        fsm = new StateMachine<SuperState, string>();

        fsm.AddState(SuperState.DAY, DaySuperState);
        fsm.AddState(SuperState.NIGHT, NightSuperState);

        fsm.AddTriggerTransition("Day", new Transition<SuperState>(SuperState.NIGHT, SuperState.DAY));
        fsm.AddTriggerTransition("Night", new Transition<SuperState>(SuperState.DAY, SuperState.NIGHT));
        fsm.Init();

    }

    private void Update()
    {

        fsm.OnLogic();
    }

    private void OnCurrentIndexChanged(int newIndex)
    {
        currentIndex = newIndex;//ί��
    }

    public void TriggerNight(bool isNight)
    {
        fsm.Trigger(isNight ? "Night" : "Day"); 
    }

}





