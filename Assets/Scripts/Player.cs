using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public bool DebugDraw;

    [SerializeField]
    private int facing;
    public int Facing => facing;

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

    [SerializeField]
    private bool hanging;
    public bool Hanging { get => hanging; set => hanging = value; }

    public float ClimbSpeed { get; set; }
    
    [SerializeField]
    private bool climbing;
    public bool Climbing { get => climbing; set => climbing = value; }

    [SerializeField]
    private bool climbingLedge;
    public bool ClimbingLedge { get => climbingLedge; set => climbingLedge = value; }

    public float WallSlideSpeed { get; set; }
    public float WallSlideStickTime { get; set; }

    [SerializeField]
    private int wallSliding;
	public int WallSliding { get => wallSliding; set => wallSliding = value; }

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
        facing = 1;
        grounded = false;

        hanging = false;

        ClimbSpeed = 3.2f;

        climbing = false;
        climbingLedge = false;

        WallSlideSpeed = 2.4f;
        WallSlideStickTime = 0.3f;

        wallSliding = 0;

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
        if (hanging && climbInput < 0)
		{
            hanging = false;
		}

        if (!climbingLedge)
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
        if (grounded)
        {
            velocity.y += jumpForce;
        }
        else if (hanging)
		{
            hanging = false;
            velocity.y += jumpForce;
		}
        else if (climbingLedge)
		{
            climbingLedge = false;
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
            velocity.x = -facing * wallJumpForce.x;
            velocity.y = wallJumpForce.y;
		}
    }

    public void SetAnimation(string stateName)
	{
        animator.speed = 1;
        animator.Play($"Base Layer.Player-{stateName}");
	}

	public void UpdateOrientation()
	{
        if (velocity.x > 0 && !(facing == 1))
        {
            facing = 1;

            Vector3 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;

            HandBox.ResetPosition();
            WallBox.ResetPosition();
        }
        else if (velocity.x < 0 && !(facing == -1))
        {
            facing = -1;

            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;

            HandBox.ResetPosition();
            WallBox.ResetPosition();
        }
    }

    public void ClimbLedge()
	{
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) 
        {
            climbingLedge = false;

            if (facing == 1)
			{
                Move(new Vector3(0.54f, 1.28f, 0));
			}
            else if (facing == -1)
			{
                Move(new Vector3(-0.54f, 1.28f, 0));
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
