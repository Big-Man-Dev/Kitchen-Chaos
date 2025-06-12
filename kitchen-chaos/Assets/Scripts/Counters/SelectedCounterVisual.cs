using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObject;
    private void Start() {
        //Player.Instance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
    }

    private void Player_OnSelectedCounterChange(object sender, Player.OnSelectedCounterChangeEventArgs e) {
        if (e.selectedCounter == baseCounter) Show();
        else Hide();
    }
    private void Show() {
        foreach (var gameObject in visualGameObject) {
            gameObject.SetActive(true);
        }
    }
    private void Hide() {
        foreach (var gameObject in visualGameObject) {
            gameObject.SetActive(false);
        }
    }
}
