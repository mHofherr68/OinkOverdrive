using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : NetworkBehaviour
{
    [Header("Refs")]
    // Reference to the projectile spawner responsible for server-authoritative projectile spawning.
    [SerializeField] private ProjectileSpawner spawner;

    [Header("Fire Settings (smaller = faster)")]
    // Minimum time between shots in seconds.
    [SerializeField] private float fireCooldown = 0.15f;

    [Header("Pattern 2 Unlock")]
    // Score threshold at which the secondary bullet pattern becomes available.
    [SerializeField] private int pattern2ScoreThreshold = 500;

    // Spread angle (in degrees) used for the secondary bullet pattern.
    [SerializeField] private float spreadDegrees = 12f;

    // Timestamp (Time.time) when the next shot is allowed.
    private float _nextFireTime;

    // Cached reference to the player's stats (used for score-based unlocks).
    private PlayerStats _stats;

    private void Awake()
    {
        // Cache PlayerStats for efficient access during gameplay.
        _stats = GetComponent<PlayerStats>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Only the owning client may process input for this NetworkObject.
        // Non-owners disable this component to prevent controlling remote players.
        if (!IsOwner)
            enabled = false;
    }

    private void Update()
    {
        // Safety check: only the owner should ever reach this point.
        if (!IsOwner)
            return;

        // Input handling (Unity Input System):
        // Continuous fire while the Space key is held down.
        var kb = Keyboard.current;
        if (kb == null || !kb.spaceKey.isPressed)
            return;

        // Enforce fire rate / cooldown.
        if (Time.time < _nextFireTime)
            return;

        _nextFireTime = Time.time + fireCooldown;

        // Determine fire direction based on the owning connection:
        // OwnerId 0 = Host (shoots to the right), OwnerId 1 = Client (shoots to the left).
        Vector2 dir = (OwnerId == 0) ? Vector2.right : Vector2.left;

        // Spawn the projectile slightly in front of the player to avoid immediate overlap.
        Vector2 pos = (Vector2)transform.position + dir * 0.6f;

        // Unlock logic for the secondary bullet pattern.
        bool unlocked = (_stats != null && _stats.Score >= pattern2ScoreThreshold);

        // Request server-side spawning via ServerRpc.
        if (unlocked)
            spawner.SpawnSpreadServerRpc(pos, dir, spreadDegrees);
        else
            spawner.SpawnProjectileServerRpc(pos, dir);
    }
}
