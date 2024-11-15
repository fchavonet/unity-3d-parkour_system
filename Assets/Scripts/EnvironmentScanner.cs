using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    // Ray settings.
    [Space(10)]
    public Vector3 forwardRayOffset = new(0, 0.25f, 0);
    public float forwardRayLength = 0.8f;
    public float heightRayLength = 5;
    public float ledgeRayLength = 10;
    public float ledgeHeightThreshold = 0.75f;
    public LayerMask obstacleLayer;

    public ObstacleHitData ObstacleCheck()
    {
        var hitData = new ObstacleHitData();

        // Calculate the starting position for the forward ray.
        var forwardOrigin = transform.position + forwardRayOffset;
        // Cast a forward ray and store if it hits an obstacle.
        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, transform.forward, out hitData.forwardHit, forwardRayLength, obstacleLayer);

        // Draw the forward ray in the Scene view, color based on hit detection.
        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound) ? Color.red : Color.white);

        // If an obstacle was detected with the forward ray.
        if (hitData.forwardHitFound)
        {
            // Calculate the starting position for the height check ray.
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            // Cast a downward ray to detect height or top of the obstacle.
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightHit, heightRayLength, obstacleLayer);

            // Draw the height ray in the Scene view, color based on hit detection.
            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);
        }

        return hitData;
    }

    // Checks if there is a ledge in the specified movement direction and gathers information about it.
    public bool LedgeCheck(Vector3 moveDirection, out LedgeData ledgeData)
    {
        ledgeData = new LedgeData();

        // Return false if no movement direction is provided.
        if (moveDirection == Vector3.zero)
        {
            return false;
        }

        // Calculate the origin for the ledge detection rays.
        float originOffest = 0.5f;
        var origin = transform.position + moveDirection * originOffest + Vector3.up;

        // Perform multiple downward raycasts to detect ledge positions.
        if (PhysicsUtilities.ThreeRaycasts(origin, Vector3.down, 0.25f, transform, out List<RaycastHit> hits, ledgeRayLength, obstacleLayer, true))
        {
            // Filter hits based on height threshold.
            var validHits = hits.Where(h => transform.position.y - h.point.y > ledgeHeightThreshold).ToList();

            if (validHits.Count > 0)
            {
                // Set up a ray from the surface point back to the character.
                var surfaceRayOrigin = validHits[0].point;
                surfaceRayOrigin.y = transform.position.y - 0.1f;

                // Check for a surface within a short distance.
                if (Physics.Raycast(surfaceRayOrigin, transform.position - surfaceRayOrigin, out RaycastHit surfaceHit, 2, obstacleLayer))
                {
                    Debug.DrawLine(surfaceRayOrigin, transform.position, Color.cyan);

                    float height = transform.position.y - validHits[0].point.y;

                    // Calculate the ledge angle and store the data.
                    ledgeData.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
                    ledgeData.height = height;
                    ledgeData.surfaceHit = surfaceHit;

                    return true;

                }
            }
        }

        return false;
    }
}

// Struct to store data about ray hits for obstacle detection.
public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
}

// Struct to store data about detected ledges.
public struct LedgeData
{
    public float height;
    public float angle;
    public RaycastHit surfaceHit;
}
