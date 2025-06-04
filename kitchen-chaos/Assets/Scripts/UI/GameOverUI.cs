using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;

    private void Start() {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        gameObject.SetActive(false);
    }
    private void GameManager_OnStateChange(object sender, System.EventArgs e) {
        if (GameManager.Instance.IsGameOver()) {
            recipesDeliveredText.text = DeliveryManager.Instance.GetRecipeDeliveredCount().ToString();
            Show();
        }
        else Hide();

    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
