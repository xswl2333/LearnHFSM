using FSM;
using System;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Jump : StateBase<NPCState>
{
    private Animator animator;
    private Timer timer;
    private Transform npcTransform;
    Action<int> onJumpTimes;
    private int currentTimes = 0;
    private Vector3 direction;

    public Jump(Animator animator, Transform npcTransform, Action<int> onJumpTimes, bool needsExitTime) : base(needsExitTime)
    {
        this.animator = animator;
        this.npcTransform = npcTransform;
        this.onJumpTimes = onJumpTimes;
        timer = new Timer();
    }

    public override void OnEnter()
    {
        animator.SetTrigger("Jump");
        timer.Reset();
    }

    public override void OnLogic()
    {
        //float diff = Mathf.Sin(Time.deltaTime) * 1.0f;
        //Vector3 temp = new Vector3(0, (npcTransform.position.y + diff) * -1.0f, 0);
        if (npcTransform.position.y>= 1)
        {
            direction = Vector3.down;
            currentTimes++;
            onJumpTimes?.Invoke(currentTimes);
        }
        if (npcTransform.position.y <= 0)
        {
            direction = Vector3.up;
        }

        npcTransform.Translate(direction * Time.deltaTime * 1.0f);

    }

    public override void OnExit()
    {
        animator.ResetTrigger("Jump");
        Vector3 tempPos = new Vector3(npcTransform.position.x, 0, 0);
        npcTransform.position = tempPos;
        currentTimes = 0;
        onJumpTimes?.Invoke(currentTimes);
    }
}