using System.Collections;
using UnityEngine;

namespace C0
{
	public class Player : MonoBehaviour
	{
		public bool DebugDraw;

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

		public RectShape BodyRectShape { get; private set; }
		public RectShape WallTopRectShape { get; private set; }
		public RectShape WallMidRectShape { get; private set; }
		public RectShape WallLowRectShape { get; private set; }
		public RectShape GroundRectShape { get; private set; }

		void Awake()
		{
			DebugDraw = false;

			gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

			Velocity = Vector2.zero;

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

			BodyRectShape = transform.Find("Body").GetComponent<RectShape>();
			WallTopRectShape = transform.Find("WallTop").GetComponent<RectShape>();
			WallMidRectShape = transform.Find("WallMid").GetComponent<RectShape>();
			WallLowRectShape = transform.Find("WallLow").GetComponent<RectShape>();
			GroundRectShape = transform.Find("Ground").GetComponent<RectShape>();

			BodyRectShape.Static = false;
			WallTopRectShape.Static = false;
			WallMidRectShape.Static = false;
			WallLowRectShape.Static = false;
			GroundRectShape.Static = false;
		}

		public void SetPosition(float x, float y)
		{
			transform.position = new Vector2(x, y);
		}

		public void SetPosition(Vector2 position)
		{
			SetPosition(position.x, position.y);
		}

		public void Move(float dx, float dy)
		{
			SetPosition(Position.x + dx, Position.y + dy);
		}

		public void Move(Vector2 displacement)
		{
			SetPosition(Position + displacement);
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
			else if (TriggerInfo.Climbable && TriggerInfo.Grounded && inputValue == 1)
			{
				Climbing = true;

				Move(0, 0.03f);
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
			if (ClimbingLedge)
			{
				return;
			}

			Vector2 jumpForce = Vector2.zero;

			if (jumpInput == 1)
			{
				if (WallSliding != 0)
				{
					if (Facing == 1 && InputInfo.Direction.x == -1)
					{
						SetWallSlide(0);
						jumpForce = Vector2.Scale(gameSettings.WallJumpVelocity, new Vector2(-1, 1));
					}
					else if (Facing == -1 && InputInfo.Direction.x == 1)
					{
						SetWallSlide(0);
						jumpForce = gameSettings.WallJumpVelocity;
					}
				}
				else if (Climbing)
				{
					Climbing = false;

					jumpForce = 0.8f * gameSettings.JumpVelocity;
				}
				else if (TriggerInfo.Grounded)
				{
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

					SetAnimation("ClimbLedge");

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
			BoxCollider2D boxCollider2D = transform.Find("Body").GetComponent<BoxCollider2D>();

			Vector2 startPosition = boxCollider2D.offset;
			Vector2 endPosition = startPosition + gameSettings.ClimbLedgeOffset;

			do
			{
				yield return null;

				boxCollider2D.offset = Vector2.Lerp(
					startPosition,
					endPosition,
					animator.GetCurrentAnimatorStateInfo(0).normalizedTime
				);
			}
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1);

			yield return null;

			SetAnimation("Idle");
			ClimbingLedge = false;
			boxCollider2D.offset = startPosition;

			if (Facing == 1)
			{
				Move(gameSettings.ClimbLedgeOffset);
			}
			else if (Facing == -1)
			{
				Move(Vector2.Scale(gameSettings.ClimbLedgeOffset, new Vector2(-1, 1)));
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
				hangTimer = gameSettings.HangTime;
				Hanging = true;

				Climbing = false;

				Vector2 position = Position;

				if (Facing == 1)
				{
					position = TriggerInfo.WallMid.TopLeft;
					position += gameSettings.HangPositionOffset;
				}
				else if (Facing == -1)
				{
					position = TriggerInfo.WallMid.TopRight;
					position += Vector2.Scale(gameSettings.HangPositionOffset, new Vector2(-1, 1)); ;
				}

				SetPosition(position);
				SetVelocity(0, 0);
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

				SetVelocity(0, 0);
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
}