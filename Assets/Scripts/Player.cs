using System.Collections;
using UnityEngine;

namespace C0
{
	public class Player : MonoBehaviour
	{
		public Vector2 Position => transform.position;
		public Vector2 Velocity;

		public Vector2 CameraTarget;

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
		private Collider2D bodyCollider;
		private Rigidbody2D rigidBody2D;

		public Rigidbody2D RigidBody2D => rigidBody2D;

		private LayerMask surfaceLayerMask;
		private LayerMask climbableLayerMask;

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

			bodyCollider = GetComponent<Collider2D>();
			rigidBody2D = GetComponent<Rigidbody2D>();

			surfaceLayerMask = LayerMask.GetMask("Surface");
			climbableLayerMask = LayerMask.GetMask("Climbable");
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

			if (Hanging && InputInfo.Direction.y < 0)
			{
				Hanging = false;
				rigidBody2D.gravityScale = 2f;
			}
			else if (TriggerInfo.Climb && InputInfo.Direction.y != 0)
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

			if (InputInfo.Jump == 1)
			{
				if (WallSliding != 0)
				{
					if (Facing == 1 && InputInfo.Direction.x == -1)
					{
						SetWallSlide(0);
						rigidBody2D.AddForce(gameSettings.WallJumpForce, ForceMode2D.Impulse);
					}
					else if (Facing == -1 && InputInfo.Direction.x == 1)
					{
						SetWallSlide(0);
						rigidBody2D.AddForce(
							Vector2.Scale(gameSettings.WallJumpForce, new Vector2(-1, 1)), 
							ForceMode2D.Impulse
						);
					}
				}
				else if (Climbing)
				{
					Climbing = false;
					rigidBody2D.AddForce(gameSettings.JumpForce, ForceMode2D.Impulse);
				}
				else if (TriggerInfo.Ground)
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

			bodyCollider.enabled = false;

			Transform targetTransform = transform.Find("Target").gameObject.transform;

			Vector2 startPosition = targetTransform.position;
			Vector2 endPosition = startPosition;

			if (Facing == 1)
			{
				endPosition += gameSettings.ClimbLedgeOffsetRight;
			}
			else if (Facing == -1)
			{
				endPosition += gameSettings.ClimbLedgeOffsetLeft;
			}

			yield return null;

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
			{
				targetTransform.position = Vector2.Lerp(
					startPosition,
					endPosition,
					animator.GetCurrentAnimatorStateInfo(0).normalizedTime
				);

				yield return null;
			}

			ClimbingLedge = false;
			bodyCollider.enabled = true;
			rigidBody2D.gravityScale = 2f;

			targetTransform.position = startPosition;

			SetAnimation("Idle");
			SetPosition(endPosition);
		}

		public void UpdateState()
		{
			CheckTriggers();
			CheckHang();
			
			ClimbUpdate();
			WallSlideUpdate();

			Velocity = rigidBody2D.velocity;

			UpdateAnimation();
			UpdateOrientation();
		}

		private void CheckTriggers()
		{
			if (ClimbingLedge)
			{
				return;
			}

			TriggerInfo.Reset();

			TriggerInfo.GroundBounds = new Bounds(
				transform.position + 0.025f * Vector3.down, 
				new Vector2(bodyCollider.bounds.size.x - 0.02f, 0.05f)
			);

			TriggerInfo.ClimbBounds = new Bounds(
				transform.position + 0.6f * Vector3.up,
				new Vector2(bodyCollider.bounds.size.x - 0.02f, 0.4f)
			);
			
			TriggerInfo.Ground = Physics2D.OverlapBox
			(
				TriggerInfo.GroundBounds.center, TriggerInfo.GroundBounds.size, 0f, surfaceLayerMask
			);

			TriggerInfo.Climb = Physics2D.OverlapBox
			(
				TriggerInfo.ClimbBounds.center, TriggerInfo.ClimbBounds.size, 0f, climbableLayerMask
			);

			float wallTriggerXOffset = Facing * (bodyCollider.bounds.extents.x + 0.05f);
			Vector2 wallTriggerSize = new Vector2(0.1f, 0.2f);

			TriggerInfo.WallTopBounds = new Bounds(
				transform.position + new Vector3(wallTriggerXOffset, 1.0f * bodyCollider.bounds.size.y),
				wallTriggerSize
			);

			TriggerInfo.WallMidBounds = new Bounds(
				transform.position + new Vector3(wallTriggerXOffset, 0.8f * bodyCollider.bounds.size.y),
				wallTriggerSize
			);

			TriggerInfo.WallLowBounds = new Bounds(
				transform.position + new Vector3(wallTriggerXOffset, 0.1f * bodyCollider.bounds.size.y),
				wallTriggerSize
			);
			
			TriggerInfo.WallTop = Physics2D.OverlapBox
			(
				TriggerInfo.WallTopBounds.center, TriggerInfo.WallTopBounds.size, 0f, surfaceLayerMask
			);

			TriggerInfo.WallMid = Physics2D.OverlapBox
			(
				TriggerInfo.WallMidBounds.center, TriggerInfo.WallMidBounds.size, 0f, surfaceLayerMask
			);

			TriggerInfo.WallLow = Physics2D.OverlapBox
			(
				TriggerInfo.WallLowBounds.center, TriggerInfo.WallLowBounds.size, 0f, surfaceLayerMask
			);
		}


		private void CheckHang()
		{
			if (TriggerInfo.Ledge && InputInfo.Direction.y > 0)
			{
				Hanging = true;
				hangTimer = gameSettings.HangTime;

				Climbing = false;

				rigidBody2D.gravityScale = 0;
				rigidBody2D.velocity = Vector2.zero;

				Vector2 position = new Vector2(
					Mathf.Round(TriggerInfo.WallMidBounds.center.x),
					Mathf.Round(TriggerInfo.WallMidBounds.center.y)
				);

				if (Facing == 1)
				{
					position += gameSettings.HangPositionOffset;
				}
				else if (Facing == -1)
				{
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

			if (!TriggerInfo.Climb)
			{
				Climbing = false;
			}
			else if (TriggerInfo.Ground && rigidBody2D.velocity.y < 0)
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
				if (!TriggerInfo.Ground && TriggerInfo.Wall && InputInfo.Direction.x == Facing)
				{
					SetWallSlide((int)InputInfo.Direction.x);
				}
			}
			else
			{
				if (TriggerInfo.Ground || !TriggerInfo.Wall)
				{
					SetWallSlide(0);
				}
				else if (InputInfo.Direction.x != WallSliding)
				{
					UpdateWallSlideTimer();
				}

				rigidBody2D.AddForce(new Vector2(0, 1.2f));
			}
		}

		private void SetWallSlide(int slideDirection)
		{
			WallSliding = slideDirection;

			if (WallSliding != 0)
			{
				wallSlideTimer = gameSettings.WallSlideHoldTime;

				rigidBody2D.velocity = Vector2.zero;
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