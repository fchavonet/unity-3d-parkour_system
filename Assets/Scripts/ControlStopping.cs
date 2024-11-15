using UnityEngine;

// Controls player input during specific animation states.
public class ControlStoppingAction : StateMachineBehaviour
{
    PlayerController player;

    // Disables player control when the animation starts.
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check if the player reference is not already assigned.
        if (player == null)
        {
            // Retrieve the PlayerController component attached to the same GameObject as the Animator.
            player = animator.GetComponent<PlayerController>();
        }

        // Disable player control while this animation state is active.
        player.HasControl = false;
    }

    // Re-enables player control when the animation ends.
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Re-enable player control after the animation state finishes.
        player.HasControl = true;
    }
}
