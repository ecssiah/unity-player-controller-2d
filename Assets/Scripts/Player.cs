using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public bool DebugDraw;

    private int facing;
    public int Facing => facing;

	private float speed;
    private float jumpForce;

    [SerializeField]
    private bool grounded;
	public bool Grounded { get => grounded; set => grounded = value; }

    private bool hanging;
    public bool Hanging => hanging;

    private bool wallSliding;
	public bool WallSliding { get => wallSliding; set => wallSliding = value; }

    private float wallSlidingVelocity;

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

        bodyBoxCollider2D = GetComponent<BoxCollider2D>();
        groundBoxCollider2D = GameObject.Find("Player/GroundTrigger").GetComponent<BoxCollider2D>();
        handBoxCollider2D = GameObject.Find("Player/HandTrigger").GetComponent<BoxCollider2D>();

        BodyBox = new BoxShape(bodyBoxCollider2D);
        GroundBox = new BoxShape(groundBoxCollider2D);
        HandBox = new BoxShape(handBoxCollider2D);

        animator = GetComponent<Animator>();

        facing = 1;
        hanging = false;
        grounded = false;

        wallSliding = false;
        wallSlidingVelocity = 4f;

        speed = 3.5f;
        jumpForce = 6f;

        velocity = Vector2.zero;
	}

    public void Move(Vector3 displacement)
	{
        transform.position += displacement;

        BodyBox.Move(displacement);
        GroundBox.Move(displacement);
        HandBox.Move(displacement);
    }

    public void SetPosition(Vector2 position)
	{
        transform.position = position;

        BodyBox.SetPosition(position);
        GroundBox.SetPosition(position);
        HandBox.SetPosition(position);
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

    public void Jump()
	{
        if (grounded)
        {
            hanging = false;
            velocity.y += jumpForce;
        }
    }

    public void HangOn(BoxShape ledgePolygon)
	{
        hanging = true;
        velocity = Vector2.zero;

        SetPosition(ledgePolygon.Center + new Vector2(0, -BodyBox.Size.y));
    }

    public void SetRunInput(float runInput)
	{
        if (!hanging)
		{
            velocity.x = speed * runInput;
		}
	}

    public void UpdateAnimation()
	{
        if (velocity.x > 0 && !(facing == 1))
		{
            facing = 1;

            Vector3 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;

            float handDisplacementX = BodyBox.Center.x - HandBox.Center.x;
            HandBox.Move(new Vector2(2 * handDisplacementX, 0));
		}
        else if (velocity.x < 0 && !(facing == -1))
		{
            facing = -1;

            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;

            float handDisplacementX = BodyBox.Center.x - HandBox.Center.x;
            HandBox.Move(new Vector2(2 * handDisplacementX, 0));
        }

        if (velocity.y != 0)
        {
            animator.Play("Base Layer.Player-Jump");
        }
        else if (velocity.x != 0)
		{
            animator.Play("Base Layer.Player-Run");
		}
        else if (hanging)
        {
            animator.Play("Base Layer.Player-Hang");
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
            Gizmos.color = new Color(1, 0.8f, 1, 0.1f);

            Gizmos.DrawCube(BodyBox.Center, BodyBox.Size);
            Gizmos.DrawCube(GroundBox.Center, GroundBox.Size);
            Gizmos.DrawCube(HandBox.Center, HandBox.Size);
        }
    }
}
