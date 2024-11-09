using UnityEngine;

// Create a ScriptableObject for defining new parkour actions.
[CreateAssetMenu(menuName = "Parkour System/New parkour action")]

public class ParkourAction : ScriptableObject
{
    [Space(10)]
    // Name of the animation for this action.
    public string animationName;

    // Height range required for this action.
    public float minHeight;
    public float maxHeight;

    // If true, player will rotate to face the obstacle.
    public bool rotateToObstacle;

    // Delay after the action is completed.
    public float postActionDelay;

    [Space(10)]
    // Target matching settings.
    public bool enableTargetMatching = true;
    public AvatarTarget matchBodyPart;
    public float matchStartTime;
    public float matchTargetTime;
    public Vector3 matchPositionWeight = new(0, 1, 0);

    // Rotation towards the obstacle.
    public Quaternion TargetRotation { get; set; }
    // Position to match during action.
    public Vector3 MatchPos { get; set; }

    // Method to check if this parkour action is possible based on obstacle height.
    public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        // Calculate height difference.
        float height = hitData.heightHit.point.y - player.position.y;

        // Return false if height is outside min or max range.
        if (height >= minHeight && height <= maxHeight)
        {
            // Set rotation to face the obstacle if enabled.
            if (rotateToObstacle)
            {
                TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);
            }

            // Set match position if target matching is enabled.
            if (enableTargetMatching)
            {
                MatchPos = hitData.heightHit.point;
            }
            return true;
        }
        return false;
    }

    // Properties for accessing action settings.
    public string AnimationName => animationName;
    public bool RotateToObstacle => rotateToObstacle;
    public float PostActionDelay => postActionDelay;

    // Target matching settings.
    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
    public Vector3 MatchPositionWeight => matchPositionWeight;
}
