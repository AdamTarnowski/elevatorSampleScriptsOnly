using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsController : MonoBehaviour
{
    #region EVENTS

    public Action<DoorsController> OnDoorsClosed = delegate { };

    #endregion

    #region MEMBERS

    [SerializeField] private Animator cachedAnimator;
    [SerializeField] private AudioSource cachedAudioSource;
    private AnimatorStateInfo CurrentStateInfo;
    private float OpeningAnimationPositionToSet;
    private StateMachineEventsRouter[] CachedEventRouters;

    #endregion

    #region PROPERTIES

    private Animator CachedAnimator { get { return cachedAnimator; } }
    private AudioSource CachedAudioSource { get { return cachedAudioSource; } }
    private bool IsPlayerBlockingDoors;

    #endregion

    #region UNITY_EVENTS

    private void Start()
    {
        CacheEventRouters();
        SetupAnimatorStateMachineBehaviours();
        AttachToEvents();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
            IsPlayerBlockingDoors = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == true)
            IsPlayerBlockingDoors = false;
    }

    private void OnDestroy()
    {
        DetachFromEvents();
    }

    #endregion

    #region FUNCTIONS

    private void CacheEventRouters()
    {
        CachedEventRouters = CachedAnimator.GetBehaviours<StateMachineEventsRouter>();
    }

    private void SetupAnimatorStateMachineBehaviours()
    {
        PlaySoundStateMachineBehaviour[] soundPlayers = CachedAnimator.GetBehaviours<PlaySoundStateMachineBehaviour>();

        for (int i = 0; i < soundPlayers.Length; i++)
            soundPlayers[i].Setup(CachedAudioSource);
    }

    private void AttachToEvents()
    {
        for (int i = 0; i < CachedEventRouters.Length; i++)
            CachedEventRouters[i].OnAnimatorStateEnter += HandleOnAnimatorStateEnter;
    }

    private void DetachFromEvents()
    {
        for (int i = 0; i < CachedEventRouters.Length; i++)
        {
            if (CachedEventRouters[i] == null)
                return;

            CachedEventRouters[i].OnAnimatorStateEnter -= HandleOnAnimatorStateEnter;
        }
    }

    public bool Open()
    {
        CurrentStateInfo = CachedAnimator.GetCurrentAnimatorStateInfo(0);

        if (CurrentStateInfo.IsName("idle") == true)
        {
            CachedAnimator.Play("openDoors", 0);
            return true;
        }
        else if (CurrentStateInfo.IsName("closeDoors") == true)
            InterruptClosing(CurrentStateInfo);

        return false;
    }

    private void InterruptClosing(AnimatorStateInfo info)
    {
        OpeningAnimationPositionToSet = 1.0f - (info.normalizedTime % 1.0f);
        CachedAnimator.Play("openDoors", 0, OpeningAnimationPositionToSet);
    }

    public void HandleOnAnimatorStateEnter(AnimatorStateInfo stateInfo)
    {
        if (stateInfo.IsName("idle") == true)
            OnDoorsClosed(this);

        if (stateInfo.IsName("closeDoors") == true)
            StartCoroutine(PlayerDetectionRoutine());
    }

    public void HandleOnAnimatorStateExit(AnimatorStateInfo stateInfo)
    {
        if (stateInfo.IsName("closeDoors") == true)
            StopCoroutine(PlayerDetectionRoutine());
    }

    private IEnumerator PlayerDetectionRoutine()
    {
        while (true)
        {
            if (IsPlayerBlockingDoors == true)
                Open();
            yield return null;
        }
    }

    #endregion
}
