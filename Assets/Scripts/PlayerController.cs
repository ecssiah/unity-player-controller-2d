using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    private struct RaycastOrigins
	{
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
	}

    public struct Collision
	{
        public bool top, bottom, right, left;

        public void Reset()
		{
            top = bottom = left = right = false;
		}
	}

    private Vector2 displacement;

    private const float RayDistance = 0.1f;

    private int rayCountX;
    private int rayCountY;

    private float raySpacingX;
    private float raySpacingY;

    private RaycastOrigins raycastOrigins;
    public Collision collision;

    private LayerMask surfaceMask;

    private BoxCollider2D boxCollider2D;

    void Awake()
	{
        boxCollider2D = GetComponent<BoxCollider2D>();

        surfaceMask = LayerMask.GetMask("Surface");

        UpdateRaySpacing();
	}

    public void Move(Vector2 requestedDisplacement)
    {
        displacement = requestedDisplacement;
        UpdateRaycastOrigins();
        collision.Reset();

        HandleCollisions();

        transform.Translate(displacement);

        Physics2D.SyncTransforms();

    }

    private void HandleCollisions()
	{
        if (displacement.x != 0)
		{
            HandleCollisionsX();
		}

        if (displacement.y != 0)
		{
            HandleCollisionsY();
		}
    }

    private void HandleCollisionsX()
	{
        float directionX = Mathf.Sign(displacement.x);

        for (int x = 0; x < rayCountX; x++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

            rayOrigin += Vector2.up * (raySpacingX * x);

            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin, displacement, displacement.magnitude, surfaceMask
            );

            Debug.DrawRay(rayOrigin, 10 * displacement, Color.magenta);

            if (hit)
            {
                if (hit.normal == Vector2.left || hit.normal == Vector2.right)
				{
                    displacement.x = directionX * hit.distance;

                    collision.right = directionX == 1;
                    collision.left = directionX == -1;
				}
            }
        }
    }

    private void HandleCollisionsY()
	{
        float directionY = Mathf.Sign(displacement.y);

        for (int y = 0; y < rayCountY; y++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;

            rayOrigin += Vector2.right * (raySpacingY * y + displacement.x);

            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin, displacement, displacement.magnitude, surfaceMask
            );

            Debug.DrawRay(rayOrigin, 10 * displacement, Color.magenta);

            if (hit)
            {
                if (hit.normal == Vector2.up || hit.normal == Vector2.down)
				{
                    displacement.y = directionY * hit.distance;

                    collision.top = directionY == 1;
                    collision.bottom = directionY == -1;
				}
            }
        }
    }

    private void UpdateRaycastOrigins()
	{
        Bounds bounds = boxCollider2D.bounds;

        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    private void UpdateRaySpacing()
	{
        Bounds bounds = boxCollider2D.bounds;

        rayCountX = Mathf.RoundToInt(bounds.size.y / RayDistance);
        rayCountY = Mathf.RoundToInt(bounds.size.x / RayDistance);

        raySpacingX = bounds.size.y / (rayCountX - 1);
        raySpacingY = bounds.size.x / (rayCountY - 1);
    }
}
