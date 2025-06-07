using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Animator animator;
    private int previousCountDownNumber;
    private const string NUMBER_POPUP = "NumberPopUp";
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start() {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        gameObject.SetActive(false);
    }
    private void Update() {
        int countDownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountDownToStartTimer());
        if (previousCountDownNumber != countDownNumber) {
            previousCountDownNumber = countDownNumber;
            SoundManager.Instance.PlayCountDownSound();
            animator.SetTrigger(NUMBER_POPUP);
        }
        countdownText.text = countDownNumber.ToString();
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
