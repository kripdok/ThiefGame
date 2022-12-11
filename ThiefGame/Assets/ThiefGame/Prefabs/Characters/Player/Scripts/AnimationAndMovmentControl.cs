using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovmentControl : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private Animator _animator;
    private int _isWalkingHash;
    private int _isRuningHash;
    private Vector2 _correctMovmentInput;
    private Vector3 _correctMovment;
    private Vector3 _correctRunMovment;
    private bool _isMovmentPressed;
    private bool _isRunPressed;
    [SerializeReference] private float _rotationFactorPerFrame = 15.0f;
    [SerializeReference] private float _runMultipliare = 3.0f;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isRuningHash = Animator.StringToHash("isRuning");
        _playerInput.CharacterController.Move.started += OnMovmentInput;
        _playerInput.CharacterController.Move.canceled += OnMovmentInput;
        _playerInput.CharacterController.Move.performed += OnMovmentInput;
        _playerInput.CharacterController.Run.started += OnRun;
        _playerInput.CharacterController.Run.canceled += OnRun;
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    private void OnMovmentInput(InputAction.CallbackContext context)
    {
        _correctMovmentInput = context.ReadValue<Vector2>();
        _correctMovment.x = _correctMovmentInput.x;
        _correctMovment.z = _correctMovmentInput.y;
        _correctRunMovment.x = _correctMovmentInput.x * _runMultipliare;
        _correctRunMovment.z = _correctMovmentInput.y * _runMultipliare;
        _isMovmentPressed = _correctMovmentInput.x != 0 || _correctMovmentInput.y != 0;
    }

    private void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = _correctMovment.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _correctMovment.z;
        Quaternion correntRotation = transform.rotation;

        if (_isMovmentPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(correntRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    private void HandleAnimation()
    {
        bool isWalking = _animator.GetBool(_isWalkingHash);
        bool isRuning = _animator.GetBool(_isRuningHash);

        if (_isMovmentPressed && isWalking == false)
        {
            _animator.SetBool(_isWalkingHash, true);
        }
        else if (_isMovmentPressed == false && isWalking)
        {
            _animator.SetBool(_isWalkingHash, false);
        }

        if ((_isMovmentPressed && _isRunPressed) && isRuning == false)
        {
            _animator.SetBool(_isRuningHash, true);
        }
        else if ((_isMovmentPressed == false || _isRunPressed == false) && isRuning)
        {
            _animator.SetBool(_isRuningHash, false);
        }
    }

    private void HandleGravity()
    {
        float gravity;

        if (_characterController.isGrounded)
        {
            gravity = -0.05f;
        }
        else
        {
            gravity = -9.8f;
        }

        _correctMovment.y = gravity;
        _correctRunMovment.y = gravity;
    }

    void Update()
    {
        HandleGravity();
        HandleRotation();
        HandleAnimation();

        if (_isRunPressed)
        {
            _characterController.Move(_correctRunMovment * Time.deltaTime);
        }
        else
        {
            _characterController.Move(_correctMovment * Time.deltaTime);

        }
    }

    private void OnEnable()
    {
        _playerInput.CharacterController.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterController.Disable();
    }
}
