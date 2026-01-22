/*using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Prefabs (muss als Spawnable registriert sein)")]
    [SerializeField] private NetworkObject playerPrefab;

    [Header("SpawnPoints")]
    [SerializeField] private Transform spawnPointP1;
    [SerializeField] private Transform spawnPointP2;

    private bool _spawnedP1;
    private bool _spawnedP2;

    private void Update()
    {
        if (!IsServerStarted)
            return;

        TrySpawnPlayers();
    }

    [Server]
    private void TrySpawnPlayers()
    {
        if (playerPrefab == null || spawnPointP1 == null || spawnPointP2 == null)
        {
            // Nicht spammen; einmal reicht
            return;
        }

        var clients = InstanceFinder.ServerManager.Clients;

        // Spawne erst, wenn 2 Clients verbunden sind (Host-Client + Remote-Client)
        if (clients.Count < 2)
            return;

        NetworkConnection conn1 = null;
        NetworkConnection conn2 = null;

        int i = 0;
        foreach (var kv in clients)
        {
            if (i == 0) conn1 = kv.Value;
            else if (i == 1) conn2 = kv.Value;
            else break;
            i++;
        }

        if (!_spawnedP1 && conn1 != null)
        {
            SpawnFor(conn1, spawnPointP1.position);
            _spawnedP1 = true;
        }

        if (!_spawnedP2 && conn2 != null)
        {
            SpawnFor(conn2, spawnPointP2.position);
            _spawnedP2 = true;
        }
    }

    [Server]
    private void SpawnFor(NetworkConnection conn, Vector3 pos)
    {
        var obj = Instantiate(playerPrefab, pos, Quaternion.identity);
        Spawn(obj, conn); // Ownership an diese Verbindung
    }
}*/


//->
using FishNet.Object;

public class GameManager : NetworkBehaviour
{
    // Später: Boss/Waves/Score
}
