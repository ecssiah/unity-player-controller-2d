using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public bool DebugDraw;

    public float Mass;
    public float Speed;
    public Vector2 Position;
    public Vector2 Velocity;
    
    public int Facing;
    public bool Grounded;
    
    private float hangTimer;
    public bool Hanging;

    public bool Climbing;
    public Vector2 ClimbSpeed;
    public bool ClimbingLedge;

    private float wallSlideTimer;
    public int WallSliding;

    public bool LedgeContact => !TriggerInfo.Top && TriggerInfo.Mid;
    public bool WallContact => TriggerInfo.Top && TriggerInfo.Mid && TriggerInfo.Low;

    public PlayerInputInfo PlayerInputInfo;
    public CollisionInfo CollisionInfo;
    public TriggerInfo TriggerInfo;

    private float jumpForce;
    private Vector2 wallJumpForce;

    private GameSettings gameSettings;

    private Animator animator;

    public RectShape BodyRectShape;
    public RectShape WallTopRectShape;
    public RectShape WallMidRectShape;
    public RectShape WallLowRectShape;
    public RectShape GroundRectShape;

    void Awake()
    {
        gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

        DebugDraw = false;

        Mass = 4;
        Speed = 7f;
        Velocity = Vector2.zero;

        Facing = 1;
        WallSliding = 0;

        Grounded = false;

        hangTimer = gameSettings.HangTime;
        Hanging = false;
        
        Climbing = false;
        ClimbSpeed = new Vector2(1.8f, 3.2f);
        ClimbingLedge = false;

        PlayerInputInfo = new PlayerInputInfo();
        CollisionInfo = new CollisionInfo();

        jumpForce = 21f;
        wallJumpForce = new Vector2(24, 20);

        animator = GetComponent<Animator>();

        BodyRectShape = transform.Find("Body").GetComponent<RectShape>();
        WallTopRectShape = transform.Find("WallTop").GetComponent<RectShape>();
        WallMidRectShape = transform.Find("WallMid").GetComponent<RectShape>();
        WallLowRectShape = transform.Find("WallLow").GetComponent<RectShape>();
        GroundRectShape = transform.Find("Ground").GetComponent<RectShape>();
    }

    public void Move(Vector2 displacement)
	{
        transform.position += new Vector3(displacement.x, displacement.y, 0);

        if (transform.position.y < -30)
        {
            SetPosition(new Vector2(0, 3));
        }

        Position = transform.position;
    }

    public void SetPosition(Vector2 position)
	{
        transform.position = position;

        Position = transform.position;
    }

    public void SetVelocity(float vx, float vy)
	{
        Velocity = new Vector2(vx, vy);
	}

    public void SetVelocity(Vector2 newVelocity)
	{
        SetVelocity(newVelocity.x, newVelocity.y);
	}

    public void AttemptClimb()
	{
        if (PlayerInputInfo.Direction.y != 0)
		{
            Climbing = true;

            SetAnimation("Climb");
            SetVelocity(0, 0);
        }
    }

    public void AttemptWallSlide()
	{
        if (Hanging || Climbing || Grounded)
        {
            SetWallSlide(0);
            return;
        }

        if (WallContact)
        {
            if (CollisionInfo.Left || CollisionInfo.Right)
            {
                SetWallSlide((int)PlayerInputInfo.Direction.x);
            }
        }

        if (WallSliding != 0)
        {
            if (!WallContact)
            {
                SetWallSlide(0);
            }
            else if (PlayerInputInfo.Direction.x != WallSliding)
            {
                UpdateWallSlide();
            }
        }
    }

    public void AttemptLedgeGrab()
	{
        if (LedgeContact && PlayerInputInfo.Direction.y > 0)
		{
            Hanging = true;
            Climbing = false;

            Vector2 position = Position;

            if (Facing == 1)
            {
                position = TriggerInfo.Mid.BodyRect.TopLeft;
                position.x -= gameSettings.HangPositionOffset.x;
                position.y += gameSettings.HangPositionOffset.y;
            }
            else if (Facing == -1)
            {
                position = TriggerInfo.Mid.BodyRect.TopRight;
                position.x += gameSettings.HangPositionOffset.x;
                position.y += gameSettings.HangPositionOffset.y;
            }

            SetPosition(position);
            SetAnimation("Hang");
            SetVelocity(Vector2.zero);
		}
    }

    public void SetWallSlide(int slideDirection)
	{
        wallSlideTimer = gameSettings.WallSlideHoldTime;
        WallSliding = slideDirection;

        if (slideDirection != 0)
		{
            SetAnimation("Slide");
            SetVelocity(Velocity.x, 0);
        }
	}

    public void UpdateWallSlide()
	{
        wallSlideTimer -= Time.deltaTime;

        if (wallSlideTimer <= 0)
        {
            SetWallSlide(0);
        }
    }

    public void AttemptLedgeClimb()
    {
        if (hangTimer <= 0)
        {
            if (PlayerInputInfo.Direction.y > 0)
            {
                Hanging = false;
                ClimbingLedge = true;
                SetAnimation("LedgeClimb");

                hangTimer = gameSettings.HangTime;
            }
        }
        else
        {
            hangTimer -= Time.deltaTime;
        }
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
        }
        else if (Velocity.x < 0 && !(Facing == -1))
        {
            Facing = -1;

            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
    }

    public void UpdateLedgeClimb()
	{
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Player-LedgeClimb"))
		{
            bool climbLedgeAnimationFinished = animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;

            if (climbLedgeAnimationFinished)
            {
                SetAnimation("Idle");

                ClimbingLedge = false;

                Vector2 climbLedgePosition = new Vector2(
                    gameSettings.ClimbLedgeOffset.x, 
                    gameSettings.ClimbLedgeOffset.y
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
            
            Gizmos.DrawWireCube(BodyRectShape.Center, BodyRectShape.Size);
            Gizmos.DrawWireCube(WallTopRectShape.Center, WallTopRectShape.Size);
            Gizmos.DrawWireCube(WallMidRectShape.Center, WallMidRectShape.Size);
            Gizmos.DrawWireCube(WallLowRectShape.Center, WallLowRectShape.Size);
            Gizmos.DrawWireCube(GroundRectShape.Center, GroundRectShape.Size);
        }
    }
}
