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

    // State flag to check if a parkour action is in progress.
    private bool inAction;

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
        if (Input.GetButton("Jump") && !inAction)
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
        if (playerController.IsOnLedge && !inAction && !hitData.forwardHitFound)
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
        inAction = true;
        playerController.SetControl(false);

        // Start the parkour animation.
        animator.SetBool("mirrorAction", action.Mirror);
        animator.CrossFade(action.animationName, 0.2f);
        yield return null;

        // Check if the correct animation has started
        var animState = animator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(action.animationName))
        {
            Debug.LogError("The parkour animation is wrong!");
        }

        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;

            // Rotate the player towards the obstacle.
            if (action.RotateToObstacle)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, action.TargetRotation, playerController.RotationSpeed * Time.deltaTime);
            }

            // Match target position if animation is correct and no transition is active.
            if (animState.IsName(action.AnimationName) && !animator.IsInTransition(0))
            {
                MatchTarget(action);
            }

            // Break out of the loop if a transition occurs and a minimum time has passed.
            if (animator.IsInTransition(0) && timer > 0.5f)
            {
                break;
            }

            yield return null;
        }

        // Wait for a delay after the action is completed.        
        yield return new WaitForSeconds(action.PostActionDelay);

        playerController.SetControl(true);
        inAction = false;
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
