using UnityEngine;
using System;

public class StateMachineEventsRouter : StateMachineBehaviour
{
    #region EVENTS

    public Action<AnimatorStateInfo> OnAnimatorStateEnter = delegate { };
    public Action<AnimatorStateInfo> OnAnimatorStateExit = delegate { };

    #endregion

    #region UNITY_EVENTS

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnAnimatorStateEnter(stateInfo);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnAnimatorStateExit(stateInfo);
    }
    
    #endregion
}