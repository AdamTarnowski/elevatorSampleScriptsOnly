using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    #region EVENTS

    public Action OnButtonPressed = delegate { };

    #endregion

    #region MEMBERS

    [SerializeField] private Animator buttonAnimatorController;
    [SerializeField] private MeshRenderer cachedRenderer;
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private AudioSource cachedAudioSource;
    [Space(20), SerializeField] private List<ButtonStateColor> stateColors;
    private ButtonStateEnum CurrentState = ButtonStateEnum.IDLE;
    private Material ClonedMaterial;

    #endregion

    #region PROPERTIES

    private Animator ButtonAnimatorController { get { return buttonAnimatorController; } }
    private MeshRenderer CachedRenderer { get { return cachedRenderer; } }
    private List<ButtonStateColor> StateColors { get { return stateColors; } }
    private AudioClip PressSound { get { return pressSound; } }
    private AudioSource CachedAudioSource { get { return cachedAudioSource; } }

    #endregion

    #region UNITY_EVENTS

    private void Awake()
    {
        ClonedMaterial = CachedRenderer.material;

        SetColor();
    }

    private void Destroy()
    {
        Destroy(ClonedMaterial);
    }

    #endregion

    #region FUNCTIONS

    public void Press()
    {
        if (CurrentState == ButtonStateEnum.IDLE)
        {
            ButtonAnimatorController.Play("buttonPress", 0, 0);
            CachedAudioSource.PlayOneShot(PressSound);
            OnButtonPressed();
        }
    }

    public void SetButtonState(ButtonStateEnum state)
    {
        if (state != CurrentState)
        {
            CurrentState = state;
            SetColor();
        }
    }

    private void SetColor()
    {
        for (int i = 0; i < StateColors.Count; i++)
            if (StateColors[i].State == CurrentState)
            {
                ClonedMaterial.EnableKeyword("_EMISSION");
                ClonedMaterial.SetColor("_EmissionColor", StateColors[i].ButtonColor);
            }
    }

    #endregion

    #region CLASSES

    [Serializable]
    private class ButtonStateColor
    {
        #region MEMBERS

        [SerializeField] private ButtonStateEnum state;
        [SerializeField] private Color buttonColor;

        #endregion

        #region PROPERTIES

        public ButtonStateEnum State { get { return state; } }
        public Color ButtonColor { get { return buttonColor; } }

        #endregion
    }

    #endregion

    #region ENUMS

    public enum ButtonStateEnum
    {
        IDLE,
        REQUESTED
    }

    #endregion
}
