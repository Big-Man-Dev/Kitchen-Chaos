using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObject;
    private void Start() {
        if(Player.LocalInstance != null) {
            Player.LocalInstance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
        }else {
            Player.OnAnyPlayerSpawn += Player_OnAnyPlayerSpawn;
        }

    }

    private void Player_OnAnyPlayerSpawn(object sender, System.EventArgs e) {
        if(Player.LocalInstance != null) {
            Player.LocalInstance.OnSelectedCounterChange -= Player_OnSelectedCounterChange;
            Player.LocalInstance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
        }
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
