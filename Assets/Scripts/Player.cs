using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public bool DebugDraw;

    public float Mass;

    public Vector2 Position;
	public Vector2 Velocity;

    public float Speed;
    public float ClimbSpeed;
    public float WallSlideSpeed;
    public float WallSlideStickTime;

    public int Facing;
    public int WallSliding;

    public bool Grounded;
    public bool Hanging;
    public bool Climbing;
    public bool ClimbingLedge;

    public PlayerInputInfo PlayerInputInfo;
    public CollisionInfo CollisionInfo;

    private float jumpForce;
    private Vector2 wallJumpForce;

    private PhysicsSettings physicsSettings;

    private Animator animator;

	private BoxCollider2D bodyBoxCollider2D;
    private BoxCollider2D handBoxCollider2D;
    private BoxCollider2D wallBoxCollider2D;
    private BoxCollider2D groundBoxCollider2D;

    public BoxShape BodyBox;
    public BoxShape HandBox;
    public BoxShape WallBox;
    public BoxShape GroundBox;

	void Awake()
	{
        
        DebugDraw = false;

        Mass = 4;
        Speed = 7f;
        Velocity = Vector2.zero;

        Facing = 1;
        WallSliding = 0;

        Grounded = false;
        Hanging = false;
        Climbing = false;
        ClimbingLedge = false;

        ClimbSpeed = 3.2f;
        WallSlideSpeed = 2.4f;
        WallSlideStickTime = 0.3f;

        PlayerInputInfo = new PlayerInputInfo();
        CollisionInfo = new CollisionInfo();

        jumpForce = 21f;
        wallJumpForce = new Vector2(24, 20);

        physicsSettings = Resources.Load<PhysicsSettings>("Settings/PhysicsSettings");

        animator = GetComponent<Animator>();

        bodyBoxCollider2D = GetComponent<BoxCollider2D>();
        handBoxCollider2D = GameObject.Find("Player/HandTrigger").GetComponent<BoxCollider2D>();
        wallBoxCollider2D = GameObject.Find("Player/WallTrigger").GetComponent<BoxCollider2D>();
        groundBoxCollider2D = GameObject.Find("Player/GroundTrigger").GetComponent<BoxCollider2D>();

        BodyBox = new BoxShape(bodyBoxCollider2D);
        HandBox = new BoxShape(handBoxCollider2D);
        WallBox = new BoxShape(wallBoxCollider2D);
        GroundBox = new BoxShape(groundBoxCollider2D);
	}

    public void Move(Vector2 displacement)
	{
        transform.position += new Vector3(displacement.x, displacement.y, 0);

        if (transform.position.y < -30)
        {
            SetPosition(new Vector2(0, 3));
        }

        Position = transform.position;

        BodyBox.ResetPosition();
        HandBox.ResetPosition();
        WallBox.ResetPosition();
        GroundBox.ResetPosition();
    }

    public void SetPosition(Vector2 position)
	{
        transform.position = position;

        Position = transform.position;

        BodyBox.ResetPosition();
        HandBox.ResetPosition();
        WallBox.ResetPosition();
        GroundBox.ResetPosition();
    }

    public void SetVelocity(float vx, float vy)
	{
        Velocity = new Vector2(vx, vy);
	}

    public void SetVelocity(Vector2 newVelocity)
	{
        SetVelocity(newVelocity.x, newVelocity.y);
	}

    public void SetRunInput(float runInput)
	{
        PlayerInputInfo.Direction.x = runInput;
	}

    public void SetClimbInput(float climbInput)
	{
        if (Hanging && climbInput < 0)
		{
            Hanging = false;
		}

        if (Climbing)
		{
            if (climbInput == 0)
		    {
                animator.speed = 0;
		    }
            else
		    {
                animator.speed = 1;
		    }
        }

        PlayerInputInfo.Direction.y = climbInput;
	}

    public void SetJumpInput(int jumpInput)
	{
        if (Grounded)
        {
            Velocity.y += jumpForce;
        }
        else if (Hanging)
		{
            Hanging = false;
            Velocity.y += jumpForce;
		}
        else if (Climbing)
		{
            Climbing = false;
            Velocity.y = jumpForce;
		}
        else if (WallSliding != 0)
		{
            WallSliding = 0;
            Velocity += new Vector2(-Facing * wallJumpForce.x, wallJumpForce.y);
        }
    }

    public void SetAnimation(string stateName)
	{
        animator.speed = 1;
        animator.Play($"Base Layer.Player-{stateName}");
	}

    public void UpdateAnimation()
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
            else if (Velocity.x != 0)
            {
                SetAnimation("Run");
            }
            else
            {
               SetAnimation("Idle");
            }
        }
    }

	public void UpdateOrientation()
	{
        if (Velocity.x > 0 && !(Facing == 1))
        {
            Facing = 1;

            Vector3 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;

            HandBox.ResetPosition();
            WallBox.ResetPosition();
        }
        else if (Velocity.x < 0 && !(Facing == -1))
        {
            Facing = -1;

            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;

            HandBox.ResetPosition();
            WallBox.ResetPosition();
        }
    }

    public void ClimbLedge()
	{
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Player-LedgeClimb"))
		{
            bool climbLedgeAnimationFinished = animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;

            if (climbLedgeAnimationFinished)
            {
                SetAnimation("Idle");

                ClimbingLedge = false;

                Vector2 climbLedgePosition = new Vector2(
                    physicsSettings.ClimbLedgePosition.x, 
                    physicsSettings.ClimbLedgePosition.y
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

	void OnDrawGizmos()
	{
        if (DebugDraw)
        {
            Gizmos.color = new Color(1.0f, 0.0f, 1.0f, 0.2f);

            Gizmos.DrawWireCube(HandBox.Center, HandBox.Size);
            Gizmos.DrawWireCube(WallBox.Center, WallBox.Size);
            Gizmos.DrawWireCube(GroundBox.Center, GroundBox.Size);
        }
    }
}
