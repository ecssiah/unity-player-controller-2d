using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public bool DebugDraw;

    private int facing;

	private float speed;
    private float jumpForce;

    private bool isHanging;

	private Vector2 velocity;
    public Vector2 Velocity => velocity;

    private BoxCollider2D bodyBoxCollider2D;
    private BoxCollider2D groundBoxCollider2D;
    private BoxCollider2D handBoxCollider2D;

    public Polygon BodyPolygon;
    public Polygon GroundPolygon;
    public Polygon HandPolygon;

    private Animator animator;

	void Awake()
	{
        DebugDraw = false;

        bodyBoxCollider2D = GetComponent<BoxCollider2D>();
        groundBoxCollider2D = GameObject.Find("Player/GroundTrigger").GetComponent<BoxCollider2D>();
        handBoxCollider2D = GameObject.Find("Player/HandTrigger").GetComponent<BoxCollider2D>();

        BodyPolygon = new Polygon(bodyBoxCollider2D);
        GroundPolygon = new Polygon(groundBoxCollider2D);
        HandPolygon = new Polygon(handBoxCollider2D);

        animator = GetComponent<Animator>();

        facing = 1;

        speed = 3.5f;
        jumpForce = 6f;

        isHanging = false;

        velocity = Vector2.zero;
	}

    public void Move(Vector3 displacement)
	{
        transform.position += displacement;

        BodyPolygon.Move(displacement);
        GroundPolygon.Move(displacement);
        HandPolygon.Move(displacement);
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
        velocity.y += jumpForce;
	}

    public void SetRunInput(float runInput)
	{
        if (!isHanging)
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

            float handDisplacementX = BodyPolygon.Center.x - HandPolygon.Center.x;
            HandPolygon.Move(new Vector2(2 * handDisplacementX, 0));
		}
        else if (velocity.x < 0 && !(facing == -1))
		{
            facing = -1;

            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;

            float handDisplacementX = BodyPolygon.Center.x - HandPolygon.Center.x;
            HandPolygon.Move(new Vector2(2 * handDisplacementX, 0));
        }

        if (velocity.y != 0)
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

	void OnDrawGizmos()
	{
        if (DebugDraw)
        {
            Gizmos.color = new Color(1, 0.8f, 1, 0.1f);

            Gizmos.DrawCube(BodyPolygon.Center, BodyPolygon.Size);
            Gizmos.DrawCube(GroundPolygon.Center, GroundPolygon.Size);
            Gizmos.DrawCube(HandPolygon.Center, HandPolygon.Size);
        }
    }
}
