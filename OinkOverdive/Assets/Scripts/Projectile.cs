using FishNet.Object;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    // Normalized movement direction of the projectile.
    private Vector2 _dir;

    // Movement speed of the projectile.
    private float _speed;

    // Indicates whether the projectile has been fully initialized.
    private bool _initialized;

    // OwnerId of the player who fired this projectile.
    // Used to prevent self-damage and to assign score correctly.
    private int _shooterOwnerId = -1;

    /// <summary>
    /// Initializes the projectile on the server.
    /// This method sets direction, speed, and shooter ownership
    /// and propagates the data to all observing clients.
    /// </summary>
    [Server]
    public void InitServer(Vector2 dir, float speed, int shooterOwnerId)
    {
        // Fallback direction if an invalid vector is provided.
        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.right;

        dir.Normalize();

        // Apply initialization locally on the server.
        ApplyInit(dir, speed, shooterOwnerId);

        // Synchronize initialization data to all clients.
        InitObserversRpc(dir, speed, shooterOwnerId);
    }

    /// <summary>
    /// Receives initialization data on all observing clients.
    /// BufferLast ensures late-joining clients receive the latest state.
    /// </summary>
    [ObserversRpc(BufferLast = true)]
    private void InitObserversRpc(Vector2 dir, float speed, int shooterOwnerId)
    {
        ApplyInit(dir, speed, shooterOwnerId);
    }

    /// <summary>
    /// Applies initialization values locally.
    /// This method is used by both server and clients.
    /// </summary>
    private void ApplyInit(Vector2 dir, float speed, int shooterOwnerId)
    {
        _dir = dir;
        _speed = speed;
        _shooterOwnerId = shooterOwnerId;
        _initialized = true;
    }

    private void Update()
    {
        // Do not move the projectile until it has been initialized.
        if (!_initialized)
            return;

        // Move the projectile using simple transform translation.
        transform.position += (Vector3)(_dir * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Server-authoritative collision handling only.
        // Clients do not process collision logic to avoid desynchronization.
        if (!IsServerStarted)
            return;

        // 1) Covers always take priority: destroy projectile immediately.
        if (other.CompareTag("Cover"))
        {
            Despawn();
            return;
        }

        // 2) Check for player hit.
        PlayerStats targetStats = other.GetComponent<PlayerStats>();
        if (targetStats == null)
            return;

        // Ignore self-hits.
        if (targetStats.OwnerId == _shooterOwnerId)
            return;

        // PvP scoring logic:
        // Shooter gains points, target loses points.
        PlayerStats shooterStats = FindShooterStatsOnServer(_shooterOwnerId);
        if (shooterStats != null)
            shooterStats.AddScoreServer(+10);

        targetStats.AddScoreServer(-10);

        // Despawn projectile after a successful hit.
        Despawn();
    }

    /// <summary>
    /// Finds the PlayerStats component of the shooter on the server
    /// based on the stored OwnerId.
    /// </summary>
    [Server]
    private PlayerStats FindShooterStatsOnServer(int ownerId)
    {
        var all = Object.FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        foreach (var ps in all)
        {
            if (ps.OwnerId == ownerId)
                return ps;
        }
        return null;
    }
}

