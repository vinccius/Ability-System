using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 7f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private InputActionAsset characterActions;

    private Animator anim;
    private InputAction moveAction;
    private void OnEnable()
    {
        characterActions.Enable();
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        moveAction = characterActions.FindAction("Move");
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    private void OnDisable()
    {
        characterActions.Disable();
    }

    void PlayerMove()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        anim.SetBool("IsWalk", input != Vector2.zero);

        Vector3 move = new Vector3(input.x, 0f, input.y);
        transform.position += move * _movementSpeed * Time.deltaTime;

        if (input == Vector2.zero)
            return;

        float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationSpeed, 0.15f);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}
