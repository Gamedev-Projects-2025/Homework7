using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This component moves a player controlled with a CharacterController using the keyboard and allows jumping.
 */
[RequireComponent(typeof(CharacterController))]
public class CharacterKeyboardMover : MonoBehaviour
{
    [Tooltip("Speed of player keyboard-movement, in meters/second")]
    [SerializeField] float speed = 3.5f;
    [Tooltip("Gravity force applied to the player, in meters/second^2")]
    [SerializeField] float gravity = 9.81f;
    [Tooltip("Initial jump velocity in meters/second")]
    [SerializeField] float jumpSpeed = 5.0f;

    private CharacterController cc;

    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction jumpAction;

    private Vector3 velocity = Vector3.zero;

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void OnValidate()
    {
        if (moveAction == null)
        {
            moveAction = new InputAction(type: InputActionType.Button);
            if (moveAction.bindings.Count == 0)
                moveAction.AddCompositeBinding("2DVector")
                    .With("Up", "<Keyboard>/upArrow")
                    .With("Down", "<Keyboard>/downArrow")
                    .With("Left", "<Keyboard>/leftArrow")
                    .With("Right", "<Keyboard>/rightArrow");
        }

        if (jumpAction == null)
        {
            jumpAction = new InputAction(type: InputActionType.Button);
            if (jumpAction.bindings.Count == 0)
                jumpAction.AddBinding("<Keyboard>/space");
        }
    }

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (cc.isGrounded)
        {
            Vector2 movementInput = moveAction.ReadValue<Vector2>();
            velocity.x = movementInput.x * speed;
            velocity.z = movementInput.y * speed;

            if (jumpAction.triggered)
            {
                velocity.y = jumpSpeed;
            }
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        Vector3 moveDirection = transform.TransformDirection(velocity);
        cc.Move(moveDirection * Time.deltaTime);
    }
}
