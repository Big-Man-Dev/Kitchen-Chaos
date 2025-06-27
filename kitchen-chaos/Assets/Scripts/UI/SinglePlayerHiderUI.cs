using UnityEngine;

public class SinglePlayerHiderUI : MonoBehaviour
{
    private void Start() {
        if (KitchenGameMultiplayer.playMultiplayer) Destroy(gameObject);
    }
}
