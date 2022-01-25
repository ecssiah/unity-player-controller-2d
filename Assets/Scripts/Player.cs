using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    private int facing;

	private float speed;
    private float jumpForce;

	private Vector2 velocity;
    public Vector2 Velocity => velocity;

    private BoxCollider2D boxCollider2D;
    public Polygon Polygon;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

	void Awake()
	{
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        boxCollider2D = GetComponent<BoxCollider2D>();
        Polygon = new Polygon(boxCollider2D);

        facing = 1;

        speed = 3.5f;
        jumpForce = 6f;

        velocity = Vector2.zero;
	}

    public void Move(Vector3 displacement)
	{
        transform.position += displacement;
        Polygon.Move(displacement);
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
        velocity.x = speed * runInput;
	}

    public void UpdateAnimation()
	{
        if (velocity.x > 0)
		{
            spriteRenderer.flipX = false;

		}
        else if (velocity.x < 0)
		{
            spriteRenderer.flipX = true;
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
}
