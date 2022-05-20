using System.Collections;
using UnityEngine;

namespace C0
{
	public class Player : MonoBehaviour
	{
		public Vector2 Position => transform.position;
		public float Facing => transform.localScale.x;
		public Rigidbody2D RigidBody2D => rigidBody2D;

		public float CurrentDampedVelocity;

		public bool Ducking;
		public bool Hanging;
		public bool Climbing;
		public bool WallSliding;
		public bool ClimbingLedge;

		public InputInfo InputInfo;
		public TriggerInfo TriggerInfo;

		private GameSettings gameSettings;

		private Animator animator;
		private Collider2D bodyCollider;
		private Rigidbody2D rigidBody2D;

		private GameObject cameraTarget;

		private LayerMask surfaceLayerMask;
		private LayerMask climbableLayerMask;

		private float nextClimbUpTime;
		private float wallSlideFallOffTime;

		void Awake()
		{
			gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

			Ducking = false;
			Hanging = false;
			Climbing = false;
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
				SetDucking(false);
			}
			else if (Hanging && InputInfo.Direction.y < 0)
			{
				SetHanging(false);
			}
			else if (TriggerInfo.Climb && InputInfo.Direction.y != 0)
			{
				SetClimbing(true);
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
					if (InputInfo.Direction.x == -Facing)
					{
						SetWallSlide(false);

						rigidBody2D.velocity = transform.localScale * gameSettings.WallJumpVelocity;
						rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
					}
				}
				else if (Climbing)
				{
					SetClimbing(false);

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
			if (InputInfo.Direction.y > 0 && Time.time >= nextClimbUpTime)
			{
				nextClimbUpTime = Time.time + gameSettings.HangTimeBeforeClimb;

				StartCoroutine(RunClimbLedgeAction());
			}
		}

		private IEnumerator RunClimbLedgeAction()
		{
			ClimbingLedge = true;

			bodyCollider.enabled = false;

			SetAnimation("ClimbLedge");

			yield return null;

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
			{
				cameraTarget.transform.localPosition = Vector2.Lerp(
					Vector2.zero,
					gameSettings.ClimbLedgeOffset,
					animator.GetCurrentAnimatorStateInfo(0).normalizedTime
				);

				yield return null;
			}

			SetHanging(false);
			SetAnimation("Idle");

			bodyCollider.enabled = true;
			rigidBody2D.velocity = Vector2.zero;
			rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;

			CurrentDampedVelocity = 0;

			cameraTarget.transform.localPosition = Vector2.zero;
			transform.Translate(transform.localScale * gameSettings.ClimbLedgeOffset);

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
			float horizontalOffset = Facing * (bodyCollider.bounds.extents.x + 0.05f);

			TriggerInfo.WallTopBounds = new Bounds(
				transform.position + new Vector3(horizontalOffset, 1.1f * bodyCollider.bounds.size.y),
				gameSettings.WallTriggerSize
			);

			TriggerInfo.WallTop = Physics2D.OverlapBox
			(
				TriggerInfo.WallTopBounds.center, TriggerInfo.WallTopBounds.size, 0f, surfaceLayerMask
			);

			TriggerInfo.WallMidBounds = new Bounds(
				transform.position + new Vector3(horizontalOffset, 0.8f * bodyCollider.bounds.size.y),
				gameSettings.WallTriggerSize
			);

			TriggerInfo.WallMid = Physics2D.OverlapBox
			(
				TriggerInfo.WallMidBounds.center, TriggerInfo.WallMidBounds.size, 0f, surfaceLayerMask
			);

			TriggerInfo.WallLowBounds = new Bounds(
				transform.position + new Vector3(horizontalOffset, 0.1f * bodyCollider.bounds.size.y),
				gameSettings.WallTriggerSize
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
					SetDucking(false);
				}
			}
			else
			{
				if (TriggerInfo.Ground && InputInfo.Direction.y < 0)
				{
					SetDucking(true);
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
				SetClimbing(false);
				SetHanging(true);
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
				SetClimbing(false);
			}
			else if (TriggerInfo.Ground && Mathf.Approximately(rigidBody2D.velocity.y, -gameSettings.ClimbSpeed.y))
			{
				SetClimbing(false);
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

				if (InputInfo.Direction.x == Facing)
				{
					wallSlideFallOffTime = Time.time + gameSettings.WallSlideHoldTime;
				}
				else if (InputInfo.Direction.x != Facing && Time.time >= wallSlideFallOffTime)
				{
					SetWallSlide(false);
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

		private void SetHanging(bool hanging)
		{
			Hanging = hanging;

			if (Hanging)
			{
				SetAnimation("Hang");

				rigidBody2D.gravityScale = 0;
				rigidBody2D.velocity = Vector2.zero;
				CurrentDampedVelocity = 0;

				nextClimbUpTime = Time.time + gameSettings.HangTimeBeforeClimb;

				Vector2 ledgePosition = new Vector2(
					Mathf.Round(TriggerInfo.WallMidBounds.center.x),
					Mathf.Round(TriggerInfo.WallMidBounds.center.y)
				);

				SetPosition(ledgePosition + transform.localScale * gameSettings.HangOffset);
			}
			else
			{
				rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
			}
		}

		private void SetDucking(bool ducking)
		{
			Ducking = ducking;

			if (Ducking)
			{
				SetAnimation("Duck");

				rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
			}
		}

		private void SetClimbing(bool climbing)
		{
			Climbing = climbing;

			if (Climbing)
			{
				SetAnimation("Climb");

				rigidBody2D.gravityScale = 0;
			}
			else
			{
				rigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
			}
		}

		private void SetWallSlide(bool wallSliding)
		{
			WallSliding = wallSliding;

			if (WallSliding)
			{
				wallSlideFallOffTime = Time.time + gameSettings.WallSlideHoldTime;

				rigidBody2D.velocity = Vector2.zero;
				rigidBody2D.gravityScale = gameSettings.WallSlideGravityScale;

				SetAnimation("Slide");
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
				else if (InputInfo.Direction.x != 0 && Mathf.Abs(rigidBody2D.velocity.x) > gameSettings.MinRunSpeed)
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
				transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
			}
			else if (Facing != -1 && rigidBody2D.velocity.x < -gameSettings.MinRunSpeed)
			{
				transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
			}
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