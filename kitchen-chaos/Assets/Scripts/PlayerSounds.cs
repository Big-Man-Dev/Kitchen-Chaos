using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private Player player;
    private float footStepTimer;
    private float footStepTimerMax = 0.1f;
    private void Awake() {
        player = GetComponent<Player>();
    }
    private void Update() {
        footStepTimer += Time.deltaTime;
        if(footStepTimer >= footStepTimerMax) {
            footStepTimer = 0;
            if(player.IsWalking()) SoundManager.Instance.PlayFootstepsSound(player.transform.position, 1f);
        }
    }
}
