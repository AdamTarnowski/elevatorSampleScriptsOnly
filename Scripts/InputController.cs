using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    #region MEMBERS

    [SerializeField] private CharacterController playerCharacterController;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float movementSpeed = 6.0f;
    [SerializeField] private float mouseSpeed = 100.0f;
    [SerializeField] private float gravity = 9.81f;
    private Vector3 MovementDelta;
    private Vector3 MovementDirection;
    private float BodyRotationDelta;
    private float HeadRotation;
    private Vector2 HeadRotationRange = new Vector2(-80.0f, 80.0f);
    private RaycastHit InteractionRaycastHitData;
    [SerializeField] float interactionRaycastMaxDistance = 2.0f;
    private RaycastHit GroundRaycastHitData;
    private bool GroundedOnMover;
    private Transform AttachedMover;
    private LayerMask InteractionLayerMask;


    #endregion

    #region PROPERTIES

    private CharacterController PlayerCharacterController { get { return playerCharacterController; } }
    private Transform CameraTransform { get { return cameraTransform; } }
    private float MovementSpeed { get { return movementSpeed; } }
    private float MouseSpeed { get { return mouseSpeed; } }
    private float Gravity { get { return gravity; } }
    float InteractionRaycastMaxDistance { get { return interactionRaycastMaxDistance; } }

    #endregion

    #region UNITY_EVENTS

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InteractionLayerMask = LayerMask.GetMask("Interaction");
    }

    private void Update()
    {
        TryPressButton();
        RaycastGround();
        UpdateBodyPosition();
        UpdateBodyRotation();
        UpdateHeadRotation();
    }

    #endregion

    #region FUNCTIONS

    private void UpdateMovementDirection()
    {
        if (PlayerCharacterController.isGrounded == false)
            return;

        MovementDirection.x = Input.GetAxis("Horizontal");
        MovementDirection.y = 0.0f;
        MovementDirection.z = Input.GetAxis("Vertical");

        MovementDirection = transform.TransformDirection(MovementDirection);
        MovementDirection.Normalize();
        MovementDirection *= MovementSpeed;
    }

    private void ApplyGravity()
    {
        MovementDirection.y -= Gravity * Time.deltaTime;
    }

    private void UpdateBodyPosition()
    {
        UpdateMovementDirection();
        ApplyGravity();

        MovementDelta = MovementDirection * Time.deltaTime;
        PlayerCharacterController.Move(MovementDelta);
    }

    private void UpdateBodyRotation()
    {
        BodyRotationDelta = Input.GetAxis("Mouse X") * MouseSpeed * Time.deltaTime;
        transform.Rotate(0.0f, BodyRotationDelta, 0.0f);
    }

    private void UpdateHeadRotation()
    {
        HeadRotation -= Input.GetAxis("Mouse Y") * MouseSpeed * Time.deltaTime;
        HeadRotation = Mathf.Clamp(HeadRotation, HeadRotationRange.x, HeadRotationRange.y);

        Quaternion newRotation = CameraTransform.localRotation;
        newRotation = Quaternion.Euler(HeadRotation, newRotation.eulerAngles.y, newRotation.eulerAngles.z);
        CameraTransform.localRotation = newRotation;
    }

    private void TryPressButton()
    {
        if (Input.GetButtonDown("Fire1") == false)
            return;

        if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out InteractionRaycastHitData, InteractionRaycastMaxDistance, InteractionLayerMask, QueryTriggerInteraction.Collide) == true)
        {
            ButtonController controller = InteractionRaycastHitData.collider.GetComponent<ButtonController>();

            if (controller != null)
                controller.Press();
        }
    }

    private void RaycastGround()
    {
        if (Physics.Raycast(transform.position, -transform.up, out GroundRaycastHitData, float.MaxValue, 1) == true)
        {
            GroundedOnMover = GroundRaycastHitData.collider.gameObject.CompareTag("Mover");

            if (GroundedOnMover == true && transform.parent == null)
                AttachToMover(GroundRaycastHitData.collider.transform);
            else if (GroundedOnMover == false && transform.parent != null)
                DetachFromMover();
        }
    }

    private void AttachToMover(Transform mover)
    {
        transform.parent = mover;
    }

    private void DetachFromMover()
    {
        transform.parent = null;
    }

    #endregion
}