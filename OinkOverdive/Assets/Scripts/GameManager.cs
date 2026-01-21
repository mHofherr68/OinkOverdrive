using FishNet;
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
            return;

        // FishNet hält eine Liste der verbundenen Clients.
        // Wir nutzen die Reihenfolge aus ServerManager.Clients.
        var clients = InstanceFinder.ServerManager.Clients;

        // Wir brauchen 2 Verbindungen: Host-Client + Remote-Client.
        if (clients.Count < 2)
            return;

        // Hole zwei Verbindungen (erste zwei Einträge).
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

        // Wichtig: Ownership an die Verbindung geben
        Spawn(obj, conn);
    }
}
