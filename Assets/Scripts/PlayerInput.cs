using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
	private Player player;

	private PlayerInputActions playerInputActions;

	private InputAction runAction;
	private InputAction jumpAction;

	void Awake()
	{
		player = GetComponent<Player>();

		playerInputActions = new PlayerInputActions();

		runAction = playerInputActions.Player.Run;
		jumpAction = playerInputActions.Player.Jump;

		runAction.started += OnStartRun;
		runAction.canceled += OnCancelRun;

		jumpAction.performed += OnJump;
	}

	void OnEnable()
	{
		runAction.Enable();
		jumpAction.Enable();
	}

	void Update()
	{
		float runInput = runAction.ReadValue<float>();
		float jumpInput = jumpAction.ReadValue<float>();
	}

	void OnDisable()
	{
		runAction.Disable();
		jumpAction.Disable();
	}
	private void OnJump(InputAction.CallbackContext context)
	{
		player.Jump();
	}

	private void OnStartRun(InputAction.CallbackContext context)
	{
		player.SetRunInput(context.ReadValue<float>());
	}

	private void OnCancelRun(InputAction.CallbackContext context)
	{
		player.SetRunInput(0);
	}
}
