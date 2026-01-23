using System.Collections;
using FishNet.Object;
using UnityEngine;

public class ProjectileSpawner : NetworkBehaviour
{
    [Header("Prefab (must be registered)")]
    // NetworkObject prefab used for spawning projectiles.
    // This prefab must be registered as a spawnable in FishNet.
    [SerializeField] private NetworkObject projectilePrefab;

    [Header("Settings")]
    // Movement speed applied to spawned projectiles.
    [SerializeField] private float projectileSpeed = 10f;

    // Time in seconds after which a projectile is despawned.
    [SerializeField] private float despawnDelay = 3f;

    /// <summary>
    /// Pattern 1: Spawns a single projectile.
    /// This method is executed on the server via ServerRpc.
    /// </summary>
    [ServerRpc]
    public void SpawnProjectileServerRpc(Vector2 position, Vector2 direction)
    {
        // Ensure this logic only runs on the server.
        if (!IsServerInitialized)
            return;

        // Safety check to avoid spawning without a valid prefab.
        if (projectilePrefab == null)
            return;

        // Fallback direction if an invalid vector is provided.
        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.right;

        direction.Normalize();

        SpawnOne(position, direction);
    }

    /// <summary>
    /// Pattern 2: Spawns a three-projectile spread shot.
    /// Projectiles are fired at negative, zero, and positive spread angles.
    /// </summary>
    [ServerRpc]
    public void SpawnSpreadServerRpc(Vector2 position, Vector2 direction, float spreadDegrees)
    {
        // Ensure this logic only runs on the server.
        if (!IsServerInitialized)
            return;

        // Safety check to avoid spawning without a valid prefab.
        if (projectilePrefab == null)
            return;

        // Fallback direction if an invalid vector is provided.
        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.right;

        direction.Normalize();

        // Spawn three projectiles:
        // left spread, center, and right spread.
        SpawnOne(position, Rotate(direction, -spreadDegrees));
        SpawnOne(position, direction);
        SpawnOne(position, Rotate(direction, spreadDegrees));
    }

    /// <summary>
    /// Instantiates and spawns a single projectile on the server,
    /// initializes its movement, and schedules despawning.
    /// </summary>
    private void SpawnOne(Vector2 position, Vector2 direction)
    {
        NetworkObject proj = Instantiate(projectilePrefab, position, Quaternion.identity);

        // IMPORTANT:
        // The projectile must be spawned before any server-side initialization
        // to ensure correct network timing and synchronization.
        Spawn(proj);

        // Initialize projectile movement on the server.
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
            p.InitServer(direction, projectileSpeed, OwnerId);

        // Schedule automatic despawn after a fixed delay.
        StartCoroutine(DespawnAfterDelay(proj, despawnDelay));
    }

    /// <summary>
    /// Rotates a direction vector by a given angle (in degrees).
    /// Used to calculate spread shot directions.
    /// </summary>
    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    /// <summary>
    /// Despawns the given NetworkObject after a specified delay.
    /// This operation is executed on the server and synchronized
    /// to all connected clients.
    /// </summary>
    private IEnumerator DespawnAfterDelay(NetworkObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null && obj.IsSpawned)
            Despawn(obj);
    }
}
