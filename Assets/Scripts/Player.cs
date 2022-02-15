using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
	public bool DebugDraw;

	public Vector2 Position;
	public Vector2 Velocity;

	public float CurrentHorizontalSpeed;

	public int Facing;

	private float hangTimer;
	public bool Hanging;

	public bool Climbing;
	public bool ClimbingLedge;

	private float wallSlideTimer;
	public int WallSliding;

	public InputInfo InputInfo;
	public TriggerInfo TriggerInfo;
	public CollisionInfo CollisionInfo;

	private GameSettings gameSettings;

	private Animator animator;

	public RectShape BodyRectShape;
	public RectShape WallTopRectShape;
	public RectShape WallMidRectShape;
	public RectShape WallLowRectShape;
	public RectShape GroundRectShape;

	void Awake()
	{
		DebugDraw = false;

		gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

		Position = transform.position;
		Velocity = Vector2.zero;

		Facing = 1;
		WallSliding = 0;

		hangTimer = gameSettings.HangTime;
		Hanging = false;

		Climbing = false;
		ClimbingLedge = false;

		animator = GetComponent<Animator>();

		BodyRectShape = transform.Find("Body").GetComponent<RectShape>();
		WallTopRectShape = transform.Find("WallTop").GetComponent<RectShape>();
		WallMidRectShape = transform.Find("WallMid").GetComponent<RectShape>();
		WallLowRectShape = transform.Find("WallLow").GetComponent<RectShape>();
		GroundRectShape = transform.Find("Ground").GetComponent<RectShape>();
	}

	public void SetPosition(float x, float y)
	{
		transform.position = new Vector2(x, y);

		Position = transform.position;
	}

	public void SetPosition(Vector2 position)
	{
		SetPosition(position.x, position.y);
	}

	public void Move(Vector2 displacement)
	{
		SetPosition((Vector2)transform.position + displacement);
	}

	public void SetVelocity(float vx, float vy)
	{
		Velocity = new Vector2(vx, vy);
	}

	public void SetVelocity(Vector2 velocity)
	{
		SetVelocity(velocity.x, velocity.y);
	}

	public void SetHorizontalInput(float inputValue)
	{
		InputInfo.Direction.x = inputValue;
	}

	public void SetVerticalInput(float inputValue)
	{
		InputInfo.Direction.y = inputValue;

		if (ClimbingLedge)
		{
			return;
		}

		if (Hanging && inputValue < 0)
		{
			Hanging = false;
		}
		else if (TriggerInfo.Climbable && inputValue != 0)
		{
			Climbing = true;

			SetAnimation("Climb");
		}
		else if (Climbing && InputInfo.Direction.y == 0)
		{
			animator.speed = 0;
		}
	}

	public void SetJumpInput(float jumpInput)
	{
		Vector2 jumpForce = Vector2.zero;

		if (jumpInput == 1)
		{
			if (WallSliding != 0)
			{
				SetWallSlide(0);

				if (Facing == 1)
				{
					jumpForce = gameSettings.WallJumpVelocity;
					jumpForce.x *= -1;
				}
				else if (Facing == -1)
				{
					jumpForce = gameSettings.WallJumpVelocity;
				}
			}
			else if (TriggerInfo.Grounded || Climbing)
			{
				Hanging = false;
				Climbing = false;

				jumpForce = gameSettings.JumpVelocity;
			}
		}
		else
		{
			if (Velocity.y > gameSettings.MinJumpSpeed)
			{
				Velocity.y = gameSettings.MinJumpSpeed;
			}
		}

		Velocity += jumpForce;
	}

	public void ClimbLedgeCheck()
	{
		if (hangTimer <= 0)
		{
			if (InputInfo.Direction.y > 0)
			{
				Hanging = false;
				ClimbingLedge = true;
				SetAnimation("LedgeClimb");

				StartCoroutine(FollowLedgeClimb());

				hangTimer = gameSettings.HangTime;
			}
		}
		else
		{
			hangTimer -= Time.deltaTime;
		}
	}

	private IEnumerator FollowLedgeClimb()
	{
		yield return null;

		RectTransform rectTransform = transform.Find("Body").GetComponent<RectTransform>();

		Vector3 ledgeOffset = gameSettings.ClimbLedgeOffset;
		Vector3 startPosition = rectTransform.localPosition;
		Vector3 endPosition = rectTransform.localPosition + ledgeOffset;

		while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
		{
			rectTransform.localPosition = Vector3.Lerp(
				startPosition,
				endPosition,
				animator.GetCurrentAnimatorStateInfo(0).normalizedTime
			);

			yield return null;
		}

		rectTransform.localPosition = startPosition;

		yield return null;
	}

	public void ClimbLedgeUpdate()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Player-LedgeClimb"))
		{
			bool climbLedgeAnimationFinished = animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;

			if (climbLedgeAnimationFinished)
			{
				SetAnimation("Idle");

				ClimbingLedge = false;

				Vector2 climbLedgePosition = new Vector2(
					gameSettings.ClimbLedgeOffset.x,
					gameSettings.ClimbLedgeOffset.y
				);

				if (Facing == 1)
				{
					Move(climbLedgePosition);
				}
				else if (Facing == -1)
				{
					climbLedgePosition.x = -climbLedgePosition.x;
					Move(climbLedgePosition);
				}
			}
		}
	}

	public void UpdateState()
	{
		HangUpdate();
		ClimbUpdate();
		WallSlideUpdate();

		UpdateAnimation();
		UpdateOrientation();
	}

	private void HangUpdate()
	{
		if (TriggerInfo.Ledge && InputInfo.Direction.y > 0)
		{
			Hanging = true;
			Climbing = false;

			Vector2 position = Position;

			if (Facing == 1)
			{
				position = TriggerInfo.Mid.BodyRect.TopLeft;
				position.x -= gameSettings.HangPositionOffset.x;
				position.y += gameSettings.HangPositionOffset.y;
			}
			else if (Facing == -1)
			{
				position = TriggerInfo.Mid.BodyRect.TopRight;
				position.x += gameSettings.HangPositionOffset.x;
				position.y += gameSettings.HangPositionOffset.y;
			}

			SetPosition(position);
			SetVelocity(0, 0);
			SetAnimation("Hang");
		}
	}

	private void ClimbUpdate()
	{
		if (Hanging || WallSliding != 0)
		{
			return;
		}
		else if (Climbing)
		{
			if (TriggerInfo.Grounded && InputInfo.Direction.y < 0)
			{
				Climbing = false;
			}
			else if (!TriggerInfo.Climbable)
			{
				Climbing = false;
			}
		}
	}

	private void WallSlideUpdate()
	{
		if (Hanging || Climbing)
		{
			return;
		}

		if (WallSliding == 0)
		{
			if (!TriggerInfo.Grounded && TriggerInfo.Wall && InputInfo.Direction.x == Facing)
			{
				SetWallSlide((int)InputInfo.Direction.x);
			}
		}
		else
		{
			if (Climbing || TriggerInfo.Grounded || !TriggerInfo.Wall)
			{
				SetWallSlide(0);
			}
			else if (InputInfo.Direction.x != WallSliding)
			{
				UpdateWallSlide();
			}
		}
	}

	private void SetWallSlide(int slideDirection)
	{
		WallSliding = slideDirection;

		if (WallSliding == 0)
		{
			wallSlideTimer = gameSettings.WallSlideHoldTime;
		}
		else
		{
			SetAnimation("Slide");
			SetVelocity(0, 0);
		}
	}

	private void UpdateWallSlide()
	{
		wallSlideTimer -= Time.deltaTime;

		if (wallSlideTimer <= 0)
		{
			SetWallSlide(0);
		}
	}


	private void SetAnimation(string stateName)
	{
		animator.speed = 1;
		animator.Play($"Base Layer.Player-{stateName}");
	}

	private void UpdateAnimation()
	{
		if (!Hanging && !Climbing && WallSliding == 0)
		{
			if (Velocity.y > 0)
			{
				SetAnimation("Jump");
			}
			else if (Velocity.y < 0)
			{
				SetAnimation("Fall");
			}
			else if (Mathf.Abs(Velocity.x) > gameSettings.MinRunSpeed)
			{
				SetAnimation("Run");
			}
			else
			{
				SetAnimation("Idle");
			}
		}
	}

	private void UpdateOrientation()
	{
		if (Velocity.x > 0 && Facing != 1)
		{
			Facing = 1;

			transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
		}
		else if (Velocity.x < 0 && Facing != -1)
		{
			Facing = -1;

			transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
		}
	}

	void OnDrawGizmos()
	{
		if (DebugDraw)
		{
			Gizmos.color = new Color(1.0f, 0.0f, 1.0f, 0.2f);

			Gizmos.DrawWireCube(BodyRectShape.Center, BodyRectShape.Size);
			Gizmos.DrawWireCube(WallTopRectShape.Center, WallTopRectShape.Size);
			Gizmos.DrawWireCube(WallMidRectShape.Center, WallMidRectShape.Size);
			Gizmos.DrawWireCube(WallLowRectShape.Center, WallLowRectShape.Size);
			Gizmos.DrawWireCube(GroundRectShape.Center, GroundRectShape.Size);
		}
	}
}
