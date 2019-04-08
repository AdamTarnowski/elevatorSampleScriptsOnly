using UnityEngine;
using System;

public class PlaySoundStateMachineBehaviour : StateMachineBehaviour
{
    #region MEMBERS

    private AudioSource CachedAudioSource;

    #endregion

    #region UNITY_EVENTS

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CachedAudioSource.Play();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CachedAudioSource.Stop();
    }

    #endregion

    #region FUNCTIONS

    public void Setup(AudioSource audioSource)
    {
        CachedAudioSource = audioSource;
    }

    #endregion
}