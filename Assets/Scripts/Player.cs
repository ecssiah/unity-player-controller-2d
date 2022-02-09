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

    [SerializeField]
    private bool wallSliding;
	public bool WallSliding { get => wallSliding; set => wallSliding = value; }

    private float wallSlidingVelocity;
    public float WallSlidingVelocity => wallSlidingVelocity;

    [SerializeField]
	private Vector2 velocity;
    public Vector2 Velocity => velocity;

	private BoxCollider2D bodyBoxCollider2D;
    private BoxCollider2D groundBoxCollider2D;
    private BoxCollider2D handBoxCollider2D;

    public BoxShape BodyBox;
    public BoxShape GroundBox;
    public BoxShape HandBox;

    private Animator animator;

	void Awake()
	{
        DebugDraw = false;

        PlayerInputInfo = new PlayerInputInfo();
        CollisionInfo = new CollisionInfo();

        bodyBoxCollider2D = GetComponent<BoxCollider2D>();
        groundBoxCollider2D = GameObject.Find("Player/GroundTrigger").GetComponent<BoxCollider2D>();
        handBoxCollider2D = GameObject.Find("Player/HandTrigger").GetComponent<BoxCollider2D>();

        BodyBox = new BoxShape(bodyBoxCollider2D);
        GroundBox = new BoxShape(groundBoxCollider2D);
        HandBox = new BoxShape(handBoxCollider2D);

        animator = GetComponent<Animator>();

        mass = 4;
        facing = new Vector2(1, 0);
        grounded = false;

        wallSliding = false;
        wallSlidingVelocity = 2.4f;

        speed = 7f;
        jumpForce = 21f;
        wallJumpForce = new Vector2(24, 20);
	}

    public void Move(Vector3 displacement)
	{
        transform.position += displacement;

        BodyBox.Move(displacement);
        GroundBox.Move(displacement);
        HandBox.Move(displacement);

        if (transform.position.y < -30)
		{
            SetPosition(new Vector2(0, 3));
		}
    }

    public void SetPosition(Vector2 position)
	{
        transform.position = position;

        BodyBox.ResetPosition();
        GroundBox.ResetPosition();
        HandBox.ResetPosition();
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

    public void SetJumpInput(int jumpInput)
	{
        if (grounded)
        {
            velocity.y += jumpForce;
        }
        else if (wallSliding)
		{
            wallSliding = false;
            velocity.x = -facing.x * wallJumpForce.x;
            velocity.y = wallJumpForce.y;
		}
    }

    public void SetRunInput(float runInput)
	{
        PlayerInputInfo.Direction.x = runInput;
	}

    public void UpdateAnimation()
	{
        if (velocity.x > 0 && !(facing.x == 1))
		{
            facing.x = 1;

            Vector3 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;

            float handDisplacementX = BodyBox.Center.x - HandBox.Center.x;
            HandBox.Move(new Vector2(2 * handDisplacementX, 0));
		}
        else if (velocity.x < 0 && !(facing.x == -1))
		{
            facing.x = -1;

            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;

            float handDisplacementX = BodyBox.Center.x - HandBox.Center.x;
            HandBox.Move(new Vector2(2 * handDisplacementX, 0));
        }

        if (wallSliding)
		{
            animator.Play("Base Layer.Player-Slide");
		}
        else if (velocity.y != 0)
        {
            animator.Play("Base Layer.Player-Jump");
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
}
