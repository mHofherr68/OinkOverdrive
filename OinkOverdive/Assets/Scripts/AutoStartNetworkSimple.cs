using FishNet;
using UnityEngine;

public class AutoStartNetworkSimple : MonoBehaviour
{
    // Unique key used to determine whether this instance should start as Host or Client.
    // PlayerPrefs are local per editor instance, which allows reliable role assignment
    // when using Unity Multiplayer Play Mode.
    private const string KEY = "OinkOverdrive_AutoStart_Role";

    private void Start()
    {
        // Small delay to ensure that the NetworkManager is fully initialized
        // before attempting to start server or client connections.
        Invoke(nameof(StartNetwork), 0.25f);
    }

    private void StartNetwork()
    {
        // Retrieve the active NetworkManager instance.
        var nm = InstanceFinder.NetworkManager;
        if (nm == null)
        {
            Debug.LogError("No NetworkManager found in scene.");
            return;
        }

        // The first editor instance becomes Host (Server + Client),
        // all subsequent instances automatically join as Clients.
        // This works because PlayerPrefs are isolated per editor instance.
        int role = PlayerPrefs.GetInt(KEY, 0);

        if (role == 0)
        {
            // Mark this instance as the Host.
            PlayerPrefs.SetInt(KEY, 1);
            PlayerPrefs.Save();

            Debug.Log("[AutoStart] Starting HOST (Server + Client)");
            nm.ServerManager.StartConnection();
            nm.ClientManager.StartConnection();
        }
        else
        {
            // All additional instances start as Clients only.
            Debug.Log("[AutoStart] Starting CLIENT");
            nm.ClientManager.StartConnection();
        }
    }

    private void OnApplicationQuit()
    {
        // Reset the stored role on application quit to ensure
        // a clean Host/Client assignment on the next Play session.
        PlayerPrefs.DeleteKey(KEY);
        PlayerPrefs.Save();
    }
}
