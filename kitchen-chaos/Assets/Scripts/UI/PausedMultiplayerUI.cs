using UnityEngine;

public class PausedMultiplayerUI : MonoBehaviour
{
    private void Start() {
        GameManager.Instance.OnMultiplayerGamePause += GameManager_OnMultiplayerGamePause;
        GameManager.Instance.OnMultiplayerGameUnpause += GameManager_OnMultiplayerGameUnpause;
        Hide();
    }

    private void GameManager_OnMultiplayerGameUnpause(object sender, System.EventArgs e) {
        Hide();
    }

    private void GameManager_OnMultiplayerGamePause(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
