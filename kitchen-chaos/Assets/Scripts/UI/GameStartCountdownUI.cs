using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start() {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        gameObject.SetActive(false);
    }
    private void Update() {
        countdownText.text = Mathf.Ceil(GameManager.Instance.GetCountDownToStartTimer()).ToString();
    }
    private void GameManager_OnStateChange(object sender, System.EventArgs e) {
        if (GameManager.Instance.IsCountdownToStartActive()) Show();
        else Hide();
    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
