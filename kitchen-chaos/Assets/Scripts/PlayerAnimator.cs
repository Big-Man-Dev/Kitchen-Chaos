using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour {

    [SerializeField] private Player player;
    private Animator animator;
    private const string IS_WALKING = "IsWalking";
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Update() {
        if (IsOwner == false) return;
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
