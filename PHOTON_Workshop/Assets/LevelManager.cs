using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private NetworkManager _networkManager;
    public Transform spawnPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
        _networkManager._playerNetworkSetup.DisableInterface();
    }
}
