using UnityEngine;
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
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionsButton.onClick.AddListener(() => {
            OptionsUI.Instance.Show();
        });
    }
    private void Start() {
        GameManager.Instance.OnGamePause += GameManger_OnGamePause;
        GameManager.Instance.OnGameUnpause += GameManager_OnGameUnpause;
        Hide();
    }
    private void GameManger_OnGamePause(object sender, System.EventArgs e) {
        Show();
    }
    private void GameManager_OnGameUnpause(object sender, System.EventArgs e) {
        Hide();
    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
