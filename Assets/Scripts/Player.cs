using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C0
{
	public class Player : MonoBehaviour
	{
		public Vector3 Position => transform.position;
		public Vector2 Velocity => rigidBody2D.velocity;
		public Bounds Bounds => bodyCollider.bounds;
		public float Facing => transform.localScale.x;
		public Vector3 Scale => transform.localScale;

		public PlayerState State { get; private set; }
		public PlayerStateType StateType;

		private Dictionary<PlayerStateType, PlayerState> playerStates;

		private GameSettings settings;

		private TriggerInfo triggerInfo;

		private Animator animator;
		private Collider2D bodyCollider;
		private Rigidbody2D rigidBody2D;

		private GameObject cameraTarget;

		public void AwakeManaged()
		{
			settings = Resources.Load<GameSettings>("Settings/GameSettings");

			triggerInfo = GetComponent<TriggerInfo>();

			animator = GetComponent<Animator>();
			bodyCollider = GetComponent<Collider2D>();
			rigidBody2D = GetComponent<Rigidbody2D>();

			cameraTarget = transform.Find("Target").gameObject;
		}

		public void StartManaged()
		{
			playerStates = new Dictionary<PlayerStateType, PlayerState>
			{
				[PlayerStateType.Move] = new MoveState(settings, this),
				[PlayerStateType.Duck] = new DuckState(settings, this),
				[PlayerStateType.Hang] = new HangState(settings, this),
				[PlayerStateType.Climb] = new ClimbState(settings, this),
				[PlayerStateType.ClimbLedge] = new ClimbLedgeState(settings, this),
				[PlayerStateType.WallSlide] = new WallSlideState(settings, this),
			};

			SetState(PlayerStateType.Move);
			SetPosition(settings.StartPosition);
		}

		public void UpdateManaged()
		{
			State.UpdateManaged();
		}

		public void FixedUpdateManaged()
		{
			State.FixedUpdateManaged();
		}

		public void SetState(PlayerStateType stateType)
		{
			StateType = stateType;
			State = playerStates[stateType];

			State.Init();
		}

		public void SetFacing(float facing)
		{
			transform.localScale = new Vector3(facing, transform.localScale.y, transform.localScale.z);
		}

		public void SetPosition(float x, float y)
		{
			transform.position = new Vector2(x, y);
		}

		public void SetPosition(Vector2 position)
		{
			SetPosition(position.x, position.y);
		}

		public void SetVelocity(float x, float y)
		{
			if (x == 0)
			{
				State.ResetVelocityXDamping();
			}
			else if (Mathf.Abs(x) < settings.MinMoveSpeed)
			{
				x = 0;
				State.ResetVelocityXDamping();
			}

			rigidBody2D.velocity = new Vector2(x, Mathf.Max(y, settings.MaxFallSpeed));
		}

		public void SetVelocity(Vector2 velocity)
		{
			SetVelocity(velocity.x, velocity.y);
		}

		public void SetGravityScale(float gravityScale)
		{
			rigidBody2D.gravityScale = gravityScale;
		}

		public void RunClimbLedgeAction()
		{
			StartCoroutine(ClimbLedgeCoroutine());
		}

		private IEnumerator ClimbLedgeCoroutine()
		{
			yield return null;

			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
			{
				cameraTarget.transform.localPosition = Vector2.Lerp(
					Vector2.zero,
					settings.ClimbLedgeOffset,
					animator.GetCurrentAnimatorStateInfo(0).normalizedTime
				);

				yield return null;
			}

			cameraTarget.transform.localPosition = Vector2.zero;

			SetPosition(Position + Vector3.Scale(transform.localScale, settings.ClimbLedgeOffset));
			SetVelocity(Vector2.zero);
			SetState(PlayerStateType.Move);
		}

		public void SetAnimation(string stateName)
		{
			SetAnimationSpeed(1);

			animator.Play($"Base Layer.Player-{stateName}");
		}

		public void SetAnimationSpeed(float speed)
		{
			animator.speed = speed;
		}

		public void UpdateAnimation()
		{
			if (rigidBody2D.velocity.y > settings.MinJumpSpeed)
			{
				SetAnimation("Jump");
			}
			else if (rigidBody2D.velocity.y < -settings.MinFallSpeed)
			{
				SetAnimation("Fall");
			}
			else if (Mathf.Abs(rigidBody2D.velocity.x) > settings.MinRunSpeed)
			{
				SetAnimation("Run");
			}
			else
			{
				SetAnimation("Idle");
			}
		}

		public void UpdateFacing()
		{
			if (Facing != 1 && rigidBody2D.velocity.x > settings.MinRunSpeed)
			{
				SetFacing(1);
			}
			else if (Facing != -1 && rigidBody2D.velocity.x < -settings.MinRunSpeed)
			{
				SetFacing(-1);
			}
		}

		void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				Gizmos.color = new Color(1, 0, 1, 0.4f);

				Gizmos.DrawWireCube(triggerInfo.GroundBounds.center, triggerInfo.GroundBounds.size);
				Gizmos.DrawWireCube(triggerInfo.ClimbBounds.center, triggerInfo.ClimbBounds.size);

				Gizmos.DrawWireCube(triggerInfo.WallTopBounds.center, triggerInfo.WallTopBounds.size);
				Gizmos.DrawWireCube(triggerInfo.WallMidBounds.center, triggerInfo.WallMidBounds.size);
				Gizmos.DrawWireCube(triggerInfo.WallLowBounds.center, triggerInfo.WallLowBounds.size);
			}
		}
	}
}