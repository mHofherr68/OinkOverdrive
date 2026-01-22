using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D _rb;

    // Nur Y Input (W/S)
    private float _inputY;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Nur der Owner darf Input lesen
        var pi = GetComponent<PlayerInput>();
        if (pi != null)
            pi.enabled = IsOwner;
    }

    // Wird vom PlayerInput (Behavior: Send Messages) aufgerufen
    public void OnMove(InputValue value)
    {
        if (!IsOwner)
            return;

        Vector2 v = value.Get<Vector2>();
        float y = Mathf.Clamp(v.y, -1f, 1f);

        // Nur hoch/runter
        _inputY = y;

        // Input an Server schicken
        SetInputServerRpc(_inputY);
    }

    [ServerRpc]
    private void SetInputServerRpc(float y)
    {
        _inputY = Mathf.Clamp(y, -1f, 1f);
    }

    private void FixedUpdate()
    {
        // Nur der Server bewegt (server-authoritativ)
        if (!IsServerStarted)
            return;

        _rb.linearVelocity = new Vector2(0f, _inputY * moveSpeed);
    }
}
