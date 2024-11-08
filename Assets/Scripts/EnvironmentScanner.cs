using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    // Ray settings.
    [Space(10)]
    public Vector3 forwardRayOffset = new(0, 0.25f, 0);
    public float forwardRayLength = 0.8f;
    public float heightRayLength = 5;
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
}

// Struct to store data about ray hits for obstacle detection.
public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
}
