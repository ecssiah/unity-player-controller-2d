using System.Collections;
using UnityEngine;

namespace C0
{
	public class Player : MonoBehaviour
	{
		public Vector2 Position => transform.position;

		public int Facing;

		public bool Ducking;

		private float hangTimer;
		public bool Hanging;

		public bool Climbing;

		private float wallSlideTimer;
		public bool WallSliding;
		
		public bool ClimbingLedge;

		public InputInfo InputInfo;
		public TriggerInfo TriggerInfo;

		private GameSettings gameSettings;

		private Animator animator;
		private Collider2D bodyCollider;
		private Rigidbody2D rigidBody2D;

		public Rigidbody2D RigidBody2D => rigidBody2D;

		private GameObject cameraTarget;

		private LayerMask surfaceLayerMask;
		private LayerMask climbableLayerMask;

		void Awake()
		{
			gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

			Facing = 1;

			Ducking = false;

			hangTimer = gameSettings.HangTime;
			Hanging = false;

			Climbing = false;

			wallSlideTimer = gameSettings.WallSlideHoldTime;
			WallSliding = false;

			ClimbingLedge = false;

			animator = GetComponent<Animator>();

			bodyCollider = GetComponent<Collider2D>();
			rigidBody2D = GetComponent<Rigidbody2D>();

			cameraTarget = transform.Find("Target").gameObject;

			surfaceLayerMask = LayerMask.GetMask("Surface");
			climbableLayerMask = LayerMask.GetMask("Climbable");

			SetPosition(gameSettings.StartPosition);
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

			if (Ducking && InputInfo.Direction.y == 0)
			{
				Ducking = false;
			}
			else if (Hanging && InputInfo.Direction.y < 0)
			{
				Hanging = false;
				rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
			}
			else if (TriggerInfo.Climb && InputInfo.Direction.y != 0)
			{
				Climbing = true;
				SetAnimation("Climb");
				rigidBody2D.gravityScale = 0;
			}
			else if (Climbing && InputInfo.Direction.y == 0)
			{
				animator.speed = 0;
			}
		}

		public void SetJumpInput(float jumpInput)
		{
			InputInfo.Jump = jumpInput;

			if (InputInfo.Jump == 1)
			{
				if (WallSliding)
				{
					if (Facing == 1 && InputInfo.Direction.x == -1)
					{
						rigidBody2D.velocity = gameSettings.WallJumpVelocityRight;
						rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;

						SetWallSlide(false);
					}
					else if (Facing == -1 && InputInfo.Direction.x == 1)
					{
						rigidBody2D.velocity = gameSettings.WallJumpVelocityLeft;
						rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;

						SetWallSlide(false);
					}
				}
				else if (Climbing)
				{
					Climbing = false;

					rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, gameSettings.JumpVelocity);
					rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
				}
				else if (TriggerInfo.Ground)
				{
					rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, gameSettings.JumpVelocity);
				}
			}
			else if (InputInfo.Jump == 0)
			{
				if (!Climbing && rigidBody2D.velocity.y > 0)
				{
					rigidBody2D.gravityScale = gameSettings.FallingGravityScale;
				}
			}
		}

		public void ClimbLedgeCheck()
		{
			if (hangTimer > 0)
			{
				hangTimer -= Time.deltaTime;
			}
			else if (InputInfo.Direction.y > 0)
			{
				StartCoroutine(RunClimbLedgeAction());
			}
		}

		private IEnumerator RunClimbLedgeAction()
		{
			ClimbingLedge = true;

			SetAnimation("ClimbLedge");
			
			Hanging = false;
			bodyCollider.enabled = false;

			yield return null;

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
			{
				cameraTarget.transform.localPosition = Vector2.Lerp(
					Vector2.zero,
					gameSettings.ClimbLedgeOffsetRight,
					animator.GetCurrentAnimatorStateInfo(0).normalizedTime
				);

				yield return null;
			}

			bodyCollider.enabled = true;
			rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;

			cameraTarget.transform.localPosition = Vector2.zero;

			SetAnimation("Idle");

			if (Facing == 1)
			{
				transform.Translate(gameSettings.ClimbLedgeOffsetRight);
			}
			else if (Facing == -1)
			{
				transform.Translate(gameSettings.ClimbLedgeOffsetLeft);
			}

			ClimbingLedge = false;
		}

		public void UpdateState()
		{
			if (Hanging)
			{
				ClimbLedgeCheck();
			}
			else
			{
				TriggerUpdate();

				DuckUpdate();
				HangUpdate();
				ClimbUpdate();
				WallSlideUpdate();

				UpdateAnimation();
				UpdateOrientation();
			}
		}

		private void TriggerUpdate()
		{
			TriggerInfo.Reset();

			GroundCheck();
			ClimbCheck();
			WallCheck();
		}

		private void GroundCheck()
		{
			TriggerInfo.GroundBounds = new Bounds(
				transform.position + 0.025f * Vector3.down,
				new Vector2(bodyCollider.bounds.size.x - 0.02f, 0.05f)
			);

			TriggerInfo.Ground = Physics2D.OverlapBox
			(
				TriggerInfo.GroundBounds.center, TriggerInfo.GroundBounds.size, 0f, surfaceLayerMask
			);
		}

		private void ClimbCheck()
		{
			TriggerInfo.ClimbBounds = new Bounds(
				transform.position + 0.6f * Vector3.up,
				new Vector2(bodyCollider.bounds.size.x - 0.02f, 0.4f)
			);

			TriggerInfo.Climb = Physics2D.OverlapBox
			(
				TriggerInfo.ClimbBounds.center, TriggerInfo.ClimbBounds.size, 0f, climbableLayerMask
			);
		}

		private void WallCheck()
		{
			float wallTriggerXOffset = Facing * (bodyCollider.bounds.extents.x + 0.05f);
			Vector2 wallTriggerSize = new Vector2(0.1f, 0.2f);

			TriggerInfo.WallTopBounds = new Bounds(
				transform.position + new Vector3(wallTriggerXOffset, 1.1f * bodyCollider.bounds.size.y),
				wallTriggerSize
			);

			TriggerInfo.WallTop = Physics2D.OverlapBox
			(
				TriggerInfo.WallTopBounds.center, TriggerInfo.WallTopBounds.size, 0f, surfaceLayerMask
			);

			TriggerInfo.WallMidBounds = new Bounds(
				transform.position + new Vector3(wallTriggerXOffset, 0.8f * bodyCollider.bounds.size.y),
				wallTriggerSize
			);

			TriggerInfo.WallMid = Physics2D.OverlapBox
			(
				TriggerInfo.WallMidBounds.center, TriggerInfo.WallMidBounds.size, 0f, surfaceLayerMask
			);

			TriggerInfo.WallLowBounds = new Bounds(
				transform.position + new Vector3(wallTriggerXOffset, 0.1f * bodyCollider.bounds.size.y),
				wallTriggerSize
			);

			TriggerInfo.WallLow = Physics2D.OverlapBox
			(
				TriggerInfo.WallLowBounds.center, TriggerInfo.WallLowBounds.size, 0f, surfaceLayerMask
			);
		}

		private void DuckUpdate()
		{
			if (Ducking)
			{
				if (!TriggerInfo.Ground)
				{
					Ducking = false;
				}
			}
			else
			{
				if (TriggerInfo.Ground && InputInfo.Direction.y < 0)
				{
					Ducking = true;
					SetAnimation("Duck");
					rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
				}
			}
		}

		private void HangUpdate()
		{
			if (Ducking)
			{
				return;
			}

			if (TriggerInfo.Ledge && InputInfo.Direction.y > 0)
			{
				Hanging = true;
				Climbing = false;

				hangTimer = gameSettings.HangTime;

				rigidBody2D.gravityScale = 0;
				rigidBody2D.velocity = Vector2.zero;

				Vector2 position = new Vector2(
					Mathf.Round(TriggerInfo.WallMidBounds.center.x),
					Mathf.Round(TriggerInfo.WallMidBounds.center.y)
				);

				if (Facing == 1)
				{
					position += gameSettings.HangPositionOffsetRight;
				}
				else if (Facing == -1)
				{
					position += gameSettings.HangPositionOffsetLeft;
				}

				SetAnimation("Hang");
				SetPosition(position);
			}
		}

		private void ClimbUpdate()
		{
			if (!Climbing)
			{
				return;
			}

			if (!TriggerInfo.Climb)
			{
				Climbing = false;
				rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
			}
			else if (TriggerInfo.Ground && rigidBody2D.velocity.y == -gameSettings.ClimbSpeed.y)
			{
				Climbing = false;
				rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
			}
		}

		private void WallSlideUpdate()
		{
			if (Ducking || Hanging || Climbing)
			{
				return;
			}

			if (WallSliding)
			{
				if (TriggerInfo.Ground || !TriggerInfo.Wall)
				{
					SetWallSlide(false);
				}
				else if (InputInfo.Direction.x != Facing)
				{
					UpdateWallSlideTimer();
				}
			}
			else
			{
				if (!TriggerInfo.Ground && TriggerInfo.Wall && InputInfo.Direction.x == Facing)
				{
					SetWallSlide(true);
				}
			}
		}

		private void SetWallSlide(bool wallSliding)
		{
			WallSliding = wallSliding;

			if (WallSliding)
			{
				wallSlideTimer = gameSettings.WallSlideHoldTime;

				rigidBody2D.velocity = Vector2.zero;
				rigidBody2D.gravityScale = gameSettings.WallSlideGravityScale;

				SetAnimation("Slide");
			}
		}

		private void UpdateWallSlideTimer()
		{
			wallSlideTimer -= Time.deltaTime;

			if (wallSlideTimer <= 0)
			{
				SetWallSlide(false);
			}
		}

		private void SetAnimation(string stateName)
		{
			animator.speed = 1;
			animator.Play($"Base Layer.Player-{stateName}");
		}

		private void UpdateAnimation()
		{
			if (!Ducking && !Hanging && !Climbing && !WallSliding)
			{
				if (rigidBody2D.velocity.y > gameSettings.MinJumpSpeed)
				{
					SetAnimation("Jump");
				}
				else if (rigidBody2D.velocity.y < -gameSettings.MinFallSpeed)
				{
					SetAnimation("Fall");
				}
				else if (InputInfo.Direction.x != 0 && Mathf.Abs(rigidBody2D.velocity.x) > 0)
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
			if (Facing != 1 && rigidBody2D.velocity.x > gameSettings.MinRunSpeed)
			{
				Facing = 1;
			}
			else if (Facing != -1 && rigidBody2D.velocity.x < -gameSettings.MinRunSpeed)
			{
				Facing = -1;
			}

			transform.localScale = new Vector3(Facing, transform.localScale.y, transform.localScale.z);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = new Color(1, 0, 1, 0.4f);

			Gizmos.DrawWireCube(TriggerInfo.GroundBounds.center, TriggerInfo.GroundBounds.size);
			Gizmos.DrawWireCube(TriggerInfo.ClimbBounds.center, TriggerInfo.ClimbBounds.size);

			Gizmos.DrawWireCube(TriggerInfo.WallTopBounds.center, TriggerInfo.WallTopBounds.size);
			Gizmos.DrawWireCube(TriggerInfo.WallMidBounds.center, TriggerInfo.WallMidBounds.size);
			Gizmos.DrawWireCube(TriggerInfo.WallLowBounds.center, TriggerInfo.WallLowBounds.size);
		}
	}
}