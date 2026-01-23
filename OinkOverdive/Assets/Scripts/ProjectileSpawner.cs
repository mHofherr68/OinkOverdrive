using System.Collections;
using FishNet.Object;
using UnityEngine;

public class ProjectileSpawner : NetworkBehaviour
{
    [Header("Prefab (muss registriert sein)")]
    [SerializeField] private NetworkObject projectilePrefab;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float despawnDelay = 3f;

    [ServerRpc]
    public void SpawnProjectileServerRpc(Vector2 position, Vector2 direction)
    {
        if (!IsServerInitialized)
            return;

        if (projectilePrefab == null)
            return;

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.right;

        direction.Normalize();

        // 1) Instantiate
        NetworkObject proj = Instantiate(projectilePrefab, position, Quaternion.identity);

        // 2) Spawn (WICHTIG: erst jetzt existiert es networkweit)
        Spawn(proj);

        // 3) Jetzt initialisieren (ObserversRpc ist ab jetzt zuverlässig)
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
            p.InitServer(direction, projectileSpeed);

        // 4) Despawn-Timer
        StartCoroutine(DespawnAfterDelay(proj, despawnDelay));
    }

    private IEnumerator DespawnAfterDelay(NetworkObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null && obj.IsSpawned)
            Despawn(obj);
    }
}

