using FishNet.Object;
using TMPro;
using UnityEngine;

public class ScoreHUD : MonoBehaviour
{
    // UI text element displaying Player 1's score (Host).
    [SerializeField] private TMP_Text p1Text;

    // UI text element displaying Player 2's score (Client).
    [SerializeField] private TMP_Text p2Text;

    // Cached list of PlayerStats components found in the scene.
    private PlayerStats[] _players;

    private void Update()
    {
        // Lazy lookup of PlayerStats components.
        // For a fixed two-player setup, this is sufficient and lightweight.
        if (_players == null || _players.Length < 2)
            _players = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);

        if (_players == null || _players.Length == 0)
            return;

        // Assign players based on ownership:
        // OwnerId 0 = Host (Player 1)
        // OwnerId 1 = Client (Player 2)
        PlayerStats p1 = null;
        PlayerStats p2 = null;

        foreach (var ps in _players)
        {
            if (ps.OwnerId == 0)
                p1 = ps;
            else if (ps.OwnerId == 1)
                p2 = ps;
        }

        // Update Player 1 score display.
        if (p1Text != null)
            p1Text.text = p1 != null ? $"P1 Score: {p1.Score}" : "P1 Score: -";

        // Update Player 2 score display.
        if (p2Text != null)
            p2Text.text = p2 != null ? $"P2 Score: {p2.Score}" : "P2 Score: -";
    }
}
