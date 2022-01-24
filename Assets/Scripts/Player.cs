using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    private PlayerController playerController;

    private float gravity;
    private float terminalVelocity;

    private Vector2 jumpVelocity;

    private float speed;
    private Vector2 velocity;

    void Awake()
	{
        playerController = GetComponent<PlayerController>();

        gravity = -9.8f;
        terminalVelocity = -53f;

        jumpVelocity = new Vector2(0, 8);

        speed = 1;
        velocity = Vector2.zero;
	}

    void Update()
    {
        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, terminalVelocity);

        playerController.Move(velocity * Time.deltaTime);

        if (playerController.collision.top || playerController.collision.bottom)
		{
            velocity.y = 0;
		}
    }

    public void Jump()
	{
        velocity += jumpVelocity;
	}

    public void SetRunInput(float runInput)
	{
        velocity.x = speed * runInput;
	}
}
