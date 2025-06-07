using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private float burnShowProgressAmount = 0.5f;
    private AudioSource audioSource;
    private float warningSoundtimer;
    private float warningSoundtimerMax = 0.2f;
    private bool playWarningSound;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        stoveCounter.OnProgressChange += StoveCounter_OnProgressChange;
    }

    private void StoveCounter_OnProgressChange(object sender, IHasProgress.OnProgressChangeEventArgs e) {
        playWarningSound = e.progressNormalized >= burnShowProgressAmount && stoveCounter.IsFried();

    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e) {
        bool playSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        if (playSound) audioSource.Play();
        else audioSource.Pause();
    }
    private void Update() {
        if (playWarningSound == false) return; 
        warningSoundtimer -= Time.deltaTime;
        if(warningSoundtimer < 0) {
            warningSoundtimer = warningSoundtimerMax;
            SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
        }
    }
}
