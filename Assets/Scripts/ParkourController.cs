using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    // References to necessary components.
    Animator animator;
    EnvironmentScanner environmentScanner;
    PlayerController playerController;

    [Space(10)]
    // List of possible parkour actions.
    public List<ParkourAction> parkourActions;
    public ParkourAction jumpDownAction;
    public float autoDropHeightLimit = 1;

    // Initialize component references.
    private void Awake()
    {
        animator = GetComponent<Animator>();
        environmentScanner = GetComponent<EnvironmentScanner>();
        playerController = GetComponent<PlayerController>();
    }

    // Update method to check for parkour input and initiate actions.
    private void Update()
    {
        var hitData = environmentScanner.ObstacleCheck();

        // If the jump button is pressed and no action is currently active.
        if (Input.GetButton("Jump") && !playerController.InAction)
        {
            // If an obstacle is detected in front.
            if (hitData.forwardHitFound)
            {
                // Loop through available actions to see if any are possible with the detected obstacle.
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        // Start the parkour action.
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }
        }

        // Check if the player is on a ledge, not performing an action, and there is no obstacle ahead.
        if (playerController.IsOnLedge && !playerController.InAction && !hitData.forwardHitFound)
        {
            bool shouldJump = true;

            // Prevents auto-jumping if the ledge is too high and the Jump button is not pressed.
            if (playerController.LedgeData.height > autoDropHeightLimit && !Input.GetButton("Jump"))
            {
                shouldJump = false;
            }

            if (shouldJump && playerController.LedgeData.angle <= 50)
            {
                // Perform a jump down action if the ledge angle is within the threshold.
                playerController.IsOnLedge = false;
                StartCoroutine(DoParkourAction(jumpDownAction));
            }
        }
    }

    // Coroutine to execute a parkour action.
    private IEnumerator DoParkourAction(ParkourAction action)
    {
        // Disable player control during the action.
        playerController.SetControl(false);

        // Prepare matching parameters if enabled for the action.
        MatchTargetParameters matchParams = null;
        if (action.EnableTargetMatching)
        {
            matchParams = new MatchTargetParameters()
            {
                position = action.MatchPos,
                bodyPart = action.MatchBodyPart,
                positionWeight = action.MatchPositionWeight,
                startTime = action.MatchStartTime,
                targetTime = action.MatchTargetTime
            };
        }

        // Execute the parkour action animation.
        yield return playerController.DoAction(action.AnimationName, matchParams, action.TargetRotation, action.RotateToObstacle, action.PostActionDelay, action.Mirror);

        // Restore player control after the action.
        playerController.SetControl(true);
    }

    // Match target position for more accurate animation alignment.
    void MatchTarget(ParkourAction action)
    {
        // Exit if already matching or in transition.
        if (animator.isMatchingTarget || animator.IsInTransition(0))
        {
            return;
        }

        // Set target matching parameters.
        animator.MatchTarget(action.MatchPos, transform.rotation, action.MatchBodyPart, new MatchTargetWeightMask(action.MatchPositionWeight, 0), action.MatchStartTime, action.MatchTargetTime);
    }
}
