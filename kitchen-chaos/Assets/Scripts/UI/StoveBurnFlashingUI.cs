using UnityEngine;

public class StoveBurnFlashingUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private float burnShowProgressAmount = 0.5f;
    private Animator animator;
    private const string IS_FLASHING = "IsFlashing";
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start() {
        stoveCounter.OnProgressChange += StoveCounter_OnProgressChange;
        animator.SetBool(IS_FLASHING, false);
    }

    private void StoveCounter_OnProgressChange(object sender, IHasProgress.OnProgressChangeEventArgs e) {
        bool showFlashingAnimaton = e.progressNormalized >= burnShowProgressAmount && stoveCounter.IsFried();
        animator.SetBool(IS_FLASHING, showFlashingAnimaton);
    }
}
