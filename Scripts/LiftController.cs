using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftController : MonoBehaviour
{
    #region MEMBERS

    [SerializeField] private AudioSource cachedAudioSource;
    [SerializeField] private AudioClip arrivalSound;
    [Space(20), SerializeField] private FloorController CurrentFloor;
    private FloorController TargetFloor;
    [SerializeField] private DoorsController doors;
    [SerializeField] private ButtonController openDoorsButton;
    [SerializeField] private WhinchController whinch;
    [SerializeField] private Transform counterweight;
    [SerializeField] private float metersPerSecond = 2.0f;
    [Space(20), SerializeField] private List<FloorController> floors;
    private Queue<FloorController> RequestedFloors = new Queue<FloorController>();
    private int BusyDoorsCount;

    #endregion

    #region PROPERTIES

    private AudioSource CachedAudioSource { get { return cachedAudioSource; } }
    private AudioClip ArrivalSound { get { return arrivalSound; } }
    private DoorsController Doors { get { return doors; } }
    private ButtonController OpenDoorsButton { get { return openDoorsButton; } }
    private WhinchController Whinch { get { return whinch; } }
    private Transform Counterweight { get { return counterweight; } }
    private List<FloorController> Floors { get { return floors; } }
    private float MetersPerSecond { get { return metersPerSecond; } }

    #endregion

    #region UNITY_EVENTS

    private void Awake()
    {
        AttachToEvents();

        TargetFloor = CurrentFloor;
    }

    private void OnDestroy()
    {
        DetachFromEvents();
    }

    #endregion

    #region FUNCTIONS

    private void AttachToEvents()
    {
        for (int i = 0; i < Floors.Count; i++)
            Floors[i].OnFloorRequested += HandleOnFloorRequested;

        OpenDoorsButton.OnButtonPressed += TryOpenDoors;
    }

    private void DetachFromEvents()
    {
        for (int i = 0; i < Floors.Count; i++)
            Floors[i].OnFloorRequested -= HandleOnFloorRequested;

        OpenDoorsButton.OnButtonPressed -= TryOpenDoors;
    }

    private void TryOpenDoors()
    {
        if (CurrentFloor == null)
            return;

        if (Doors.Open() == true)
        {
            Doors.OnDoorsClosed += HandleOnDoorsClosed;
            BusyDoorsCount++;
        }

        if (CurrentFloor.Doors.Open() == true)
        {
            CurrentFloor.Doors.OnDoorsClosed += HandleOnDoorsClosed;
            BusyDoorsCount++;
        }

        CurrentFloor.NotifyOnLiftArrival();
    }

    private void BeginTransfer()
    {
        CurrentFloor = null;
        Whinch.SetSoundState(true);
        StartCoroutine(LiftTransferRoutine());
    }

    private void EndTransfer()
    {
        Whinch.SetSoundState(false);
        CachedAudioSource.PlayOneShot(ArrivalSound);
        CurrentFloor = TargetFloor;
        TryOpenDoors();
    }

    private IEnumerator LiftTransferRoutine()
    {
        Vector3 targetPosition;

        while (Vector3.Distance(transform.position, TargetFloor.transform.position) > 0.001f)
        {
            targetPosition = Vector3.MoveTowards(transform.position, TargetFloor.transform.position, MetersPerSecond * Time.deltaTime);
            UpdateWhinch(transform.position, targetPosition);
            UpdateCounterweightPosition(transform.position, targetPosition);
            transform.position = targetPosition;

            yield return null;
        }

        EndTransfer();
    }

    private void HandleOldestRequest()
    {
        TargetFloor = RequestedFloors.Dequeue();
        BeginTransfer();
    }

    private void HandleOnFloorRequested(FloorController floor)
    {
        if (floor == CurrentFloor)
            TryOpenDoors();
        else
        {
            RequestedFloors.Enqueue(floor);
            if (BusyDoorsCount == 0 && CurrentFloor != null)
                HandleOldestRequest();
        }
    }

    private void HandleOnDoorsClosed(DoorsController doors)
    {
        doors.OnDoorsClosed -= HandleOnDoorsClosed;
        BusyDoorsCount--;

        if (RequestedFloors.Count > 0 && BusyDoorsCount == 0)
            HandleOldestRequest();
    }

    private void UpdateWhinch(Vector3 liftPosition, Vector3 liftFrameTargetPosition)
    {
        Whinch.ApplyLiftPositionDelta(liftFrameTargetPosition.y - liftPosition.y);
    }

    private void UpdateCounterweightPosition(Vector3 liftPosition, Vector3 liftFrameTargetPosition)
    {
        Counterweight.Translate(liftPosition - liftFrameTargetPosition);
    }

    #endregion
}
