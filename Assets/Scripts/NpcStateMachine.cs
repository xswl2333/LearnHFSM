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
    JUMP,
}

public enum SuperState
{
    NULL,
    DAY,
    NIGHT,
    DUSK//�ƻ�

}

public class NpcStateMachine : MonoBehaviour
{
    [SerializeField] private float oneDayTime;//һ���ʱ��
    [SerializeField] private Animator animator;
    [SerializeField] private float idleTime;
    [SerializeField] private float restTime;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private SpriteRenderer restZone;
    [SerializeField] private CameraColor mainCamera;
    

    //�ֲ�����״̬
    private StateMachine<SuperState, string> fsm;
    private StateMachine<SuperState, NPCState, string> NightSuperState;
    private StateMachine<SuperState, NPCState, string> DaySuperState;
    private StateMachine<SuperState, NPCState, string> DuskSuperState;

    private int jumpTimes = 0;
    private int currentIndex = 0;
    private float currentTimes = 1;//��ǰʱ��
    private SuperState currentDayType= SuperState.DAY;

    private void Start()
    {
        var tempObject= GameObject.Find("Main Camera");
        mainCamera = tempObject.GetComponent<CameraColor>();
        //�����ʼ��
        DaySuperState = new StateMachine<SuperState, NPCState, string>();
        DaySuperState.AddState(NPCState.IDLE, new Idle(animator, idleTime, true));
        DaySuperState.AddState(NPCState.WALK, new Walk(animator, waypoints,
            transform, OnCurrentIndexChanged, speed, false));//����Ҫ�ȴ�������Ҫ�˳�ʱ

        DaySuperState.AddTransition(NPCState.WALK, NPCState.IDLE,
          transition => Vector2.Distance(transform.position, waypoints[currentIndex].position) < 0.2f);

        DaySuperState.AddTransition(NPCState.IDLE, NPCState.WALK);
        DaySuperState.Init();

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
            transition => Vector2.Distance(transform.position, waypoints[currentIndex].position) < 0.2f);

        NightSuperState.Init();
        //�ƻ��ʼ��
        DuskSuperState = new StateMachine<SuperState, NPCState, string>();
        DuskSuperState.AddState(NPCState.IDLE, new Idle(animator, idleTime, true));
        DuskSuperState.AddState(NPCState.JUMP, new Jump(animator, transform, OnCurrentJumpTimesChanged, false));
        DuskSuperState.AddTransition(NPCState.IDLE, NPCState.JUMP, transition => jumpTimes == 0);
        DuskSuperState.AddTransition(NPCState.JUMP, NPCState.IDLE,
          transition => jumpTimes >= 2);

        DuskSuperState.Init();
        //��ҹ�л�
        // fsm init
        fsm = new StateMachine<SuperState, string>();

        fsm.AddState(SuperState.DAY, DaySuperState);
        fsm.AddState(SuperState.NIGHT, NightSuperState);
        fsm.AddState(SuperState.DUSK, DuskSuperState);

        fsm.AddTransition(SuperState.DAY, SuperState.DUSK, transition => currentTimes >= 30);
        fsm.AddTransition(SuperState.DUSK, SuperState.NIGHT, transition => currentTimes >= 45);
        fsm.AddTriggerTransition("day", new Transition<SuperState>(SuperState.NIGHT, SuperState.DAY));

        fsm.Init();

    }

    private void Update()
    {
        if (Time.time >= currentTimes)
        {
            currentTimes += Time.deltaTime;
        }
        if(currentTimes<=30)
        {
            currentDayType = SuperState.DAY;
        }
        else if(currentTimes>30&&currentTimes<=45)
        {
            currentDayType = SuperState.DUSK;

        }
        else if(currentTimes<60)
        {
            currentDayType = SuperState.NIGHT;
        }
        else
        {
            fsm.Trigger("day");
            currentTimes = 0;
        }
        mainCamera.ChangeColor(currentDayType);
        fsm.OnLogic();
    }

    private void OnCurrentIndexChanged(int newIndex)
    {
        currentIndex = newIndex;//ί��
    }

    private void OnCurrentJumpTimesChanged(int newTimes)
    {
        jumpTimes = newTimes;//ί��
    }

    public void TriggerNight(bool isNight)
    {
        fsm.Trigger(isNight ? "Night" : "Day");
    }

}





