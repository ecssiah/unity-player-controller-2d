using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    private PlayerController playerController;

    private float gravity;
    private float terminalVelocity;

    private Vector2 velocity;

    void Awake()
	{
        playerController = GetComponent<PlayerController>();

        gravity = -0.0f;
        terminalVelocity = -53f;

        velocity = Vector2.zero;
	}

    void Update()
    {
        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, terminalVelocity);

        playerController.Move(velocity * Time.deltaTime);
    }
}
