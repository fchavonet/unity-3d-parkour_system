using UnityEngine;

// Create a ScriptableObject for defining new parkour actions.
[CreateAssetMenu(menuName = "Parkour System/New parkour action")]

public class ParkourAction : ScriptableObject
{
    [Space(10)]
    // Animation name for this parkour action.
    public string animationName;

    // Applicable height range for this action.
    public float minHeight;
    public float maxHeight;

    // Method to check if this parkour action is possible based on obstacle height.
    public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        // Calculate height difference
        float height = hitData.heightHit.point.y - player.position.y;

        // Return false if height is outside min or max range.
        if (height < minHeight || height > maxHeight)
        {
            return false;
        }

        // Action is possible if height is within range.
        return true;
    }

    public string AnimationName => animationName;
}
