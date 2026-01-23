using FishNet.Object;
using TMPro;
using UnityEngine;

public class GameOverUI : NetworkBehaviour
{
    // Root GameObject of the Game Over UI overlay.
    // This object is enabled when the game reaches a Game Over state.
    [SerializeField] private GameObject overlayRoot; // GameOverOverlay

    // Text element used to display the Game Over message.
    [SerializeField] private TMP_Text gameOverText;  // GameOverText

    // Internal state flag to indicate whether the game has ended.
    private bool _isGameOver;

    // Public read-only access to the Game Over state.
    public bool IsGameOver => _isGameOver;

    /// <summary>
    /// Displays the Game Over UI on all connected clients.
    /// This method is invoked by the server and synchronized
    /// to all observers.
    /// </summary>
    [ObserversRpc(BufferLast = true)]
    public void ShowGameOverObserversRpc(string message)
    {
        _isGameOver = true;

        // Enable the Game Over overlay.
        if (overlayRoot != null)
            overlayRoot.SetActive(true);

        // Update the Game Over message text.
        if (gameOverText != null)
            gameOverText.text = message;

        // Disable player input for the local player only.
        DisableLocalPlayerControls();
    }

    /// <summary>
    /// Disables movement and shooting input for the local player instance.
    /// This ensures that each client only disables its own controls
    /// while still allowing the Game Over UI to remain visible.
    /// </summary>
    private void DisableLocalPlayerControls()
    {
        // Disable movement scripts for the local player only.
        var moves = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
        foreach (var m in moves)
        {
            if (m != null && m.IsOwner)
                m.enabled = false;
        }

        // Disable shooting scripts for the local player only.
        var shoots = FindObjectsByType<PlayerShooting>(FindObjectsSortMode.None);
        foreach (var s in shoots)
        {
            if (s != null && s.IsOwner)
                s.enabled = false;
        }
    }
}
