using UnityEngine;

public class CoverMover : MonoBehaviour
{
    // Minimum vertical position the cover is allowed to reach.
    [SerializeField] private float minY = -4f;

    // Maximum vertical position the cover is allowed to reach.
    [SerializeField] private float maxY = 4f;

    // Movement speed of the cover along the Y-axis.
    [SerializeField] private float speed = 2f;

    // Current movement direction:
    // 1 = moving up, -1 = moving down.
    private int _dir = 1;

    private void Update()
    {
        // Calculate the new vertical position based on speed and direction.
        Vector3 p = transform.position;
        p.y += _dir * speed * Time.deltaTime;

        // Reverse direction when reaching the upper boundary.
        if (p.y >= maxY)
        {
            p.y = maxY;
            _dir = -1;
        }
        // Reverse direction when reaching the lower boundary.
        else if (p.y <= minY)
        {
            p.y = minY;
            _dir = 1;
        }

        // Apply the updated position.
        transform.position = p;
    }
}
