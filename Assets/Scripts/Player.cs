using System.Collections;
using UnityEngine;

namespace C0
{
	public class Player : MonoBehaviour
	{
		public Vector2 Position => transform.position;
		public Vector2 Velocity;

		public int Facing;

		private float hangTimer;
		public bool Hanging;

		public bool Climbing;
		public bool ClimbingLedge;

		private float wallSlideTimer;
		public int WallSliding;

		public InputInfo InputInfo;
		public TriggerInfo TriggerInfo;

		private GameSettings gameSettings;

		private Animator animator;
		private CapsuleCollider2D capsuleCollider2D;
		private Rigidbody2D rigidBody2D;

		public Rigidbody2D RigidBody2D => rigidBody2D;

		private LayerMask surfaceLayerMask;

		void Awake()
		{
			gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

			Facing = 1;

			hangTimer = gameSettings.HangTime;
			Hanging = false;

			Climbing = false;
			ClimbingLedge = false;

			wallSlideTimer = gameSettings.WallSlideHoldTime;
			WallSliding = 0;

			animator = GetComponent<Animator>();

			foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
			{
				if (clip.name == "Player-ClimbLedge")
				{
					clip.wrapMode = WrapMode.Once;
				}
			}

			capsuleCollider2D = GetComponent<CapsuleCollider2D>();
			rigidBody2D = GetComponent<Rigidbody2D>();

			surfaceLayerMask = LayerMask.GetMask("Surface");
		}

		public void SetPosition(float x, float y)
		{
			transform.position = new Vector2(x, y);
		}

		public void SetPosition(Vector2 position)
		{
			SetPosition(position.x, position.y);
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
			else if (TriggerInfo.Climbable && TriggerInfo.Grounded && inputValue == 1)
			{
				Climbing = true;

				SetPosition(Position + new Vector2(0, 0.03f));
				SetAnimation("Climb");
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
			InputInfo.Jump = jumpInput;

			if (ClimbingLedge)
			{
				return;
			}

			if (jumpInput == 1)
			{
				if (WallSliding != 0)
				{
					if (Facing == 1 && InputInfo.Direction.x == -1)
					{
						SetWallSlide(0);
					}
					else if (Facing == -1 && InputInfo.Direction.x == 1)
					{
						SetWallSlide(0);
					}
				}
				else if (Climbing)
				{
					Climbing = false;
				}
				else if (IsGrounded())
				{
					rigidBody2D.AddForce(gameSettings.JumpForce, ForceMode2D.Impulse);
				}
			}
		}

		public void ClimbLedgeCheck()
		{
			if (hangTimer <= 0)
			{
				if (InputInfo.Direction.y > 0)
				{
					StartCoroutine(RunClimbLedgeAction());
				}
			}
			else
			{
				hangTimer -= Time.deltaTime;
			}
		}

		private IEnumerator RunClimbLedgeAction()
		{
			Hanging = false;
			ClimbingLedge = true;
			SetAnimation("ClimbLedge");

			Vector2 startPosition = capsuleCollider2D.offset;
			Vector2 endPosition = startPosition + gameSettings.ClimbLedgeOffset;

			yield return null;

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
			{
				capsuleCollider2D.offset = Vector2.Lerp(
					startPosition,
					endPosition,
					animator.GetCurrentAnimatorStateInfo(0).normalizedTime
				);

				yield return null;
			}

			ClimbingLedge = false;
			capsuleCollider2D.offset = startPosition;
			SetAnimation("Idle");

			if (Facing == 1)
			{
				SetPosition(Position + gameSettings.ClimbLedgeOffset);
			}
			else if (Facing == -1)
			{
				SetPosition(Position + Vector2.Scale(gameSettings.ClimbLedgeOffset, new Vector2(-1, 1)));
			}
		}

		private bool IsGrounded()
		{
			Collider2D colliderHit = Physics2D.OverlapBox
			(
				transform.position + 0.05f * Vector3.down,
				new Vector2(capsuleCollider2D.bounds.size.x - 0.01f, 0.1f),
				0f,
				surfaceLayerMask
			);

			return colliderHit != null;
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
				hangTimer = gameSettings.HangTime;

				Climbing = false;

				Vector2 position = Position;

				if (Facing == 1)
				{
					position = new Vector2(
						TriggerInfo.WallMid.bounds.center.x - TriggerInfo.WallMid.bounds.extents.x,
						TriggerInfo.WallMid.bounds.center.y + TriggerInfo.WallMid.bounds.extents.y
					);
					position += gameSettings.HangPositionOffset;
				}
				else if (Facing == -1)
				{
					position = new Vector2(
						TriggerInfo.WallMid.bounds.center.x + TriggerInfo.WallMid.bounds.extents.x,
						TriggerInfo.WallMid.bounds.center.y + TriggerInfo.WallMid.bounds.extents.y
					);
					position += Vector2.Scale(gameSettings.HangPositionOffset, new Vector2(-1, 1));
				}

				SetPosition(position);
				SetAnimation("Hang");
			}
		}

		private void ClimbUpdate()
		{
			if (!Climbing)
			{
				return;
			}

			if (!TriggerInfo.Climbable)
			{
				Climbing = false;
			}
			else if (TriggerInfo.Grounded)
			{
				Climbing = false;
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
				if (TriggerInfo.Grounded || !TriggerInfo.Wall)
				{
					SetWallSlide(0);
				}
				else if (InputInfo.Direction.x != WallSliding)
				{
					UpdateWallSlideTimer();
				}
			}
		}

		private void SetWallSlide(int slideDirection)
		{
			WallSliding = slideDirection;

			if (WallSliding != 0)
			{
				wallSlideTimer = gameSettings.WallSlideHoldTime;

				SetAnimation("Slide");
			}
		}

		private void UpdateWallSlideTimer()
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
				if (rigidBody2D.velocity.y > gameSettings.MinSpeed)
				{
					SetAnimation("Jump");
				}
				else if (rigidBody2D.velocity.y < -gameSettings.MinSpeed)
				{
					SetAnimation("Fall");
				}
				else if (Mathf.Abs(rigidBody2D.velocity.x) > gameSettings.MinSpeed)
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
			if (Facing != 1 && rigidBody2D.velocity.x > gameSettings.MinSpeed)
			{
				Facing = 1;

				transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
			}
			else if (Facing != -1 && rigidBody2D.velocity.x < -gameSettings.MinSpeed)
			{
				Facing = -1;

				transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
			}
		}
	}
}