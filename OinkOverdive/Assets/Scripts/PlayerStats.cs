using FishNet.Object;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    // Initial score assigned to the player when the object is spawned on the server.
    [SerializeField] private int startScore = 100;

    // Current score of the player.
    // This value is synchronized manually via RPCs.
    public int Score { get; private set; }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Initialize the player's score on the server.
        SetScoreServer(startScore);
    }

    /// <summary>
    /// Adds or subtracts score on the server.
    /// This method must only be called by server-side logic.
    /// </summary>
    [Server]
    public void AddScoreServer(int delta)
    {
        SetScoreServer(Score + delta);
    }

    /// <summary>
    /// Sets the player's score on the server and propagates
    /// the updated value to all observing clients.
    /// Also performs the server-side Game Over check.
    /// </summary>
    [Server]
    private void SetScoreServer(int value)
    {
        Score = value;

        // Synchronize the updated score to all clients.
        ScoreObserversRpc(Score);

        // === GAME OVER CHECK (server-authoritative) ===
        // The server decides when a player has lost the game.
        if (Score <= 0)
        {
            var ui = FindFirstObjectByType<GameOverUI>();
            if (ui != null && !ui.IsGameOver)
            {
                ui.ShowGameOverObserversRpc($"PLAYER {OwnerId + 1} LOST");
            }
        }
    }

    /// <summary>
    /// Receives score updates on all clients.
    /// BufferLast ensures that late-joining clients
    /// receive the most recent score state.
    /// </summary>
    [ObserversRpc(BufferLast = true)]
    private void ScoreObserversRpc(int newScore)
    {
        Score = newScore;

        // Debug output for the local player only.
        if (IsOwner)
            Debug.Log($"[LOCAL] Score = {Score}");
    }
}
