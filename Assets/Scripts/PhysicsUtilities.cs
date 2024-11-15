using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtilities
{
    // Performs three raycasts (center, left, right) from the origin in the specified direction.
    public static bool ThreeRaycasts(Vector3 origin, Vector3 direction, float spacing, Transform transform, out List<RaycastHit> hits, float distance, LayerMask layer, bool debugDraw = false)
    {
        // Perform center, left, and right raycasts with spacing for obstacle detection.
        bool centerHitFound = Physics.Raycast(origin, Vector3.down, out RaycastHit centerHit, distance, layer);
        bool leftHitFound = Physics.Raycast(origin - transform.right * spacing, Vector3.down, out RaycastHit leftHit, distance, layer);
        bool rightHitFound = Physics.Raycast(origin + transform.right * spacing, Vector3.down, out RaycastHit rightHit, distance, layer);

        // Store the raycast results in a list.
        hits = new List<RaycastHit>() { centerHit, leftHit, rightHit };

        // Check if any of the raycasts found a hit.
        bool hitFound = centerHitFound || leftHitFound || rightHitFound;

        // If debugDraw is enabled, draw the rays in the Scene view.
        if (hitFound && debugDraw)
        {
            Debug.DrawLine(origin, centerHit.point, Color.red);
            Debug.DrawLine(origin - transform.right * spacing, leftHit.point, Color.red);
            Debug.DrawLine(origin + transform.right * spacing, rightHit.point, Color.red);
        }

        // Return whether any raycast hit something.
        return hitFound;
    }
}
