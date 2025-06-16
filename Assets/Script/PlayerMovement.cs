using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    public bool autoMoveForward = true;
    [SerializeField] private float speedIncreaseInterval = 5f; // Time between speed boosts
    [SerializeField] private float speedIncreaseAmount = 2f; // How much speed increases
    [SerializeField] private float maxSpeed = 20f; // Maximum speed limit

    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    private PlayerControls _inputActions;
    private Transform _model;
    private Vector3 _baseMovementDirection = Vector3.forward;
    private float _speedTimer = 0f;
    private float _currentMoveSpeed;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _model = transform;
        _inputActions = new PlayerControls();
        _inputActions.Gameplay.Enable();
        _currentMoveSpeed = moveSpeed; // Initialize current speed
    }

    void Update()
    {
        // Handle speed increase timer
        _speedTimer += Time.deltaTime;
        if (_speedTimer >= speedIncreaseInterval)
        {
            _speedTimer = 0f;
            _currentMoveSpeed = Mathf.Min(_currentMoveSpeed + speedIncreaseAmount, maxSpeed);
        }

        // Ground check
        if (_controller.isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        // Read input
        _moveInput = _inputActions.Gameplay.Move.ReadValue<Vector2>();

        // Calculate movement direction
        Vector3 inputDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
        Vector3 combinedDirection = _baseMovementDirection + inputDirection;
        Vector3 moveDirection = combinedDirection.normalized;
        
        // Apply movement using current speed
        Vector3 moveVelocity = moveDirection * _currentMoveSpeed;
        _controller.Move(moveVelocity * Time.deltaTime);

        // Rotation
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            _model.rotation = Quaternion.Slerp(
                _model.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            
            if (autoMoveForward) 
                _baseMovementDirection = moveDirection;
        }

        // Gravity
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    void OnEnable() => _inputActions.Gameplay.Enable();
    void OnDisable() => _inputActions.Gameplay.Disable();

    // Optional: Add method to reset speed
    public void ResetSpeed()
    {
        _currentMoveSpeed = moveSpeed;
        _speedTimer = 0f;
    }
}