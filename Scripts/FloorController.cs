using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    #region EVENTS

    public Action<FloorController> OnFloorRequested = delegate { };

    #endregion

    #region MEMBERS

    [SerializeField] private List<ButtonController> buttons;
    [SerializeField] private DoorsController doors;

    #endregion

    #region PROPERTIES

    public List<ButtonController> Buttons { get { return buttons; } }
    public DoorsController Doors { get { return doors; } }

    #endregion

    #region UNITY_EVENTS

    private void Awake()
    {
        AttachToEvents();
    }

    private void OnDestroy()
    {
        DetachFromEvents();
    }

    #endregion

    #region FUNCTIONS

    public void NotifyOnLiftArrival()
    {
        for (int i = 0; i < Buttons.Count; i++)
            Buttons[i].SetButtonState(ButtonController.ButtonStateEnum.IDLE);
    }

    private void AttachToEvents()
    {
        for (int i = 0; i < Buttons.Count; i++)

            Buttons[i].OnButtonPressed += HandleOnButtonPressed;
    }

    private void DetachFromEvents()
    {
        for (int i = 0; i < Buttons.Count; i++)
            Buttons[i].OnButtonPressed -= HandleOnButtonPressed;
    }

    private void HandleOnButtonPressed()
    {
        for (int i = 0; i < Buttons.Count; i++)
            Buttons[i].SetButtonState(ButtonController.ButtonStateEnum.REQUESTED);

        OnFloorRequested(this);
    }

    #endregion
}
