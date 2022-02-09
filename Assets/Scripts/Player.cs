using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public bool DebugDraw;

    [SerializeField]
    private Vector2 facing;
    public Vector2 Facing => facing;

    private float mass;
    public float Mass => mass;

    private float speed;
    public float Speed => speed;

    private float jumpForce;
    private Vector2 wallJumpForce;

    [SerializeField]
    private bool grounded;
    public bool Grounded { get => grounded; set => grounded = value; }

    public PlayerInputInfo PlayerInputInfo;

    public CollisionInfo CollisionInfo;
 
    public float WallSlideSpeed { get; set; }
    public float WallSlideStickTime { get; set; }

    [SerializeField]
    private int wallSliding;
	public int WallSliding { get => wallSliding; set => wallSliding = value; }

    public float ClimbSpeed { get; set; }

    [SerializeField]
    private bool climbing;
    public bool Climbing { get => climbing; set => climbing = value; }

    [SerializeField]
	private Vector2 velocity;
    public Vector2 Velocity => velocity;

	private BoxCollider2D bodyBoxCollider2D;
    private BoxCollider2D handBoxCollider2D;
    private BoxCollider2D wallBoxCollider2D;
    private BoxCollider2D groundBoxCollider2D;

    public BoxShape BodyBox;
    public BoxShape HandBox;
    public BoxShape WallBox;
    public BoxShape GroundBox;

    private Animator animator;

	void Awake()
	{
        DebugDraw = false;

        PlayerInputInfo = new PlayerInputInfo();
        CollisionInfo = new CollisionInfo();

        bodyBoxCollider2D = GetComponent<BoxCollider2D>();
        handBoxCollider2D = GameObject.Find("Player/HandTrigger").GetComponent<BoxCollider2D>();
        wallBoxCollider2D = GameObject.Find("Player/WallTrigger").GetComponent<BoxCollider2D>();
        groundBoxCollider2D = GameObject.Find("Player/GroundTrigger").GetComponent<BoxCollider2D>();

        BodyBox = new BoxShape(bodyBoxCollider2D);
        HandBox = new BoxShape(handBoxCollider2D);
        WallBox = new BoxShape(wallBoxCollider2D);
        GroundBox = new BoxShape(groundBoxCollider2D);

        animator = GetComponent<Animator>();

        mass = 4;
        facing = new Vector2(1, 0);
        grounded = false;

        WallSlideSpeed = 2.4f;
        WallSlideStickTime = 0.3f;

        wallSliding = 0;

        ClimbSpeed = 3.2f;

        climbing = false;

        speed = 7f;
        jumpForce = 21f;
        wallJumpForce = new Vector2(24, 20);
	}

    public void Move(Vector3 displacement)
	{
        transform.position += displacement;

        if (transform.position.y < -30)
        {
            SetPosition(new Vector2(0, 3));
        }

        BodyBox.ResetPosition();
        HandBox.ResetPosition();
        WallBox.ResetPosition();
        GroundBox.ResetPosition();
    }

    public void SetPosition(Vector2 position)
	{
        transform.position = position;

        BodyBox.ResetPosition();
        HandBox.ResetPosition();
        WallBox.ResetPosition();
        GroundBox.ResetPosition();
    }

    public void SetVelocity(float vx, float vy)
	{
        velocity.x = vx;
        velocity.y = vy;
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
        PlayerInputInfo.Direction.y = climbInput;
	}

    public void SetJumpInput(int jumpInput)
	{
        if (grounded)
        {
            velocity.y += jumpForce;
        }
        else if (climbing)
		{
            climbing = false;
            velocity.y = jumpForce;
		}
        else if (wallSliding != 0)
		{
            wallSliding = 0;
            velocity.x = -facing.x * wallJumpForce.x;
            velocity.y = wallJumpForce.y;
		}
    }

    public void UpdateAnimation()
	{
        if (velocity.x > 0 && !(facing.x == 1))
		{
            facing.x = 1;

			Vector3 scale = transform.localScale;
			scale.x = 1;
			transform.localScale = scale;

            HandBox.ResetPosition();
			WallBox.ResetPosition();
        }
        else if (velocity.x < 0 && !(facing.x == -1))
		{
            facing.x = -1;

			Vector3 scale = transform.localScale;
			scale.x = -1;
			transform.localScale = scale;

            HandBox.ResetPosition();
			WallBox.ResetPosition();
        }

        if (climbing)
		{
            animator.Play("Base Layer.Player-Climb");

            if (velocity.y == 0)
			{
                animator.speed = 0;
			}
            else
			{
                animator.speed = 1;
			}
		}
        else if (wallSliding != 0)
		{
            animator.Play("Base Layer.Player-Slide");
		}
        else if (!grounded)
        {
            if (velocity.y >= 0)
			{
                animator.Play("Base Layer.Player-Jump");
			} 
            else if (velocity.y < 0)
			{
                animator.Play("Base Layer.Player-Fall");
			}
		}
        else if (velocity.x != 0)
		{
            animator.Play("Base Layer.Player-Run");
		}
        else
		{
            animator.Play("Base Layer.Player-Idle");
		}
    }

	void OnDrawGizmos()
	{
        if (DebugDraw)
        {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.1f);

            Gizmos.DrawWireCube(HandBox.Center, HandBox.Size);
            Gizmos.DrawWireCube(WallBox.Center, WallBox.Size);
            Gizmos.DrawWireCube(GroundBox.Center, GroundBox.Size);
        }
    }
}
