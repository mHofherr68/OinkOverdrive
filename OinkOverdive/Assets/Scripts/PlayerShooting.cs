using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private ProjectileSpawner spawner;
    [SerializeField] private float fireCooldown = 0.25f;

    private float _nextFireTime;

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Nur Owner verarbeitet Input
        if (!IsOwner)
            enabled = false;
    }

    // PlayerInput (Behavior: Send Messages) ruft diese Methode auf,
    // wenn deine Action "Fire" ausgelöst wird.
    public void OnAttack(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (Time.time < _nextFireTime)
            return;

        _nextFireTime = Time.time + fireCooldown;

        Vector2 dir = (OwnerId == 0) ? Vector2.right : Vector2.left; // Host rechts, Client links
        Vector2 pos = (Vector2)transform.position + dir * 0.6f;      // vor dem Player spawnen

        spawner.SpawnProjectileServerRpc(pos, dir);
    }
}
