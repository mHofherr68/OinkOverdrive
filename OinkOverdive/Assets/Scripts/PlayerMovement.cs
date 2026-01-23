using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : NetworkBehaviour
{
    // Movement speed applied to the player along the Y-axis.
    [SerializeField] private float moveSpeed = 5f;

    // Cached reference to the Rigidbody2D component.
    private Rigidbody2D _rb;

    // Cached vertical input value (W/S only).
    private float _inputY;

    private void Awake()
    {
        // Cache Rigidbody2D and configure physics settings.
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Only the owning client is allowed to read and process input.
        var pi = GetComponent<PlayerInput>();
        if (pi != null)
            pi.enabled = IsOwner;
    }

    /// <summary>
    /// Called by the PlayerInput component (Behavior: Send Messages)
    /// whenever movement input is received.
    /// Only vertical input (W/S) is processed.
    /// </summary>
    public void OnMove(InputValue value)
    {
        // Safety check to ensure only the owning client sends input.
        if (!IsOwner)
            return;

        Vector2 v = value.Get<Vector2>();
        float y = Mathf.Clamp(v.y, -1f, 1f);

        // Store vertical movement input only.
        _inputY = y;

        // Send input to the server for authoritative movement handling.
        SetInputServerRpc(_inputY);
    }

    /// <summary>
    /// Receives movement input on the server.
    /// The server stores the input value and uses it
    /// to apply authoritative movement.
    /// </summary>
    [ServerRpc]
    private void SetInputServerRpc(float y)
    {
        _inputY = Mathf.Clamp(y, -1f, 1f);
    }

    private void FixedUpdate()
    {
        // Server-authoritative movement:
        // Only the server applies velocity changes to the Rigidbody.
        if (!IsServerStarted)
            return;

        _rb.linearVelocity = new Vector2(0f, _inputY * moveSpeed);
    }
}
