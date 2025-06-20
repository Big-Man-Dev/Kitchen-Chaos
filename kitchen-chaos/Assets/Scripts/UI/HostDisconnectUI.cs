using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour 
{
    [SerializeField] private Button playAgainButton;

    private void Start() {
        playAgainButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        NetworkManager.Singleton.OnClientStopped += NetworkManager_OnClientStopped;
        Hide();
    }
    private void OnDestroy() {
        NetworkManager.Singleton.OnClientStopped -= NetworkManager_OnClientStopped;
    }

    private void NetworkManager_OnClientStopped(bool obj) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
