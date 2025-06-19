using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;
    private void Awake() {
        resumeButton.onClick.AddListener(() => {
            GameManager.Instance.TogglePauseGame();
        });
        mainMenuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionsButton.onClick.AddListener(() => {
            Hide();
            OptionsUI.Instance.Show(Show); 
        });
    }
    private void Start() {
        GameManager.Instance.OnLocalGamePause += GameManger_OnLocalGamePause;
        GameManager.Instance.OnLocalGameUnpause += GameManager_OnLocalGameUnpause;
        Hide();
    }
    private void GameManger_OnLocalGamePause(object sender, System.EventArgs e) {
        Show();
    }
    private void GameManager_OnLocalGameUnpause(object sender, System.EventArgs e) {
        Hide();
    }
    private void Show() {
        gameObject.SetActive(true);
        if(Gamepad.all.Count > 0) resumeButton.Select();
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
