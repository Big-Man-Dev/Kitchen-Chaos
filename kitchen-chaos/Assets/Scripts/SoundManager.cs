using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClipRefsSO audioClipRefs;
    private float volume = 1f;
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    private void Awake() {
        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }
    private void Start() {
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.OnAnyPickedSomething += Player_OnAnyPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void Player_OnAnyPickedSomething(object sender, System.EventArgs e) {
        PlaySound(audioClipRefs.objectPickup, (sender as Player).transform.position);
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e) {
        PlaySound(audioClipRefs.trash, (sender as TrashCounter).transform.position);

    }

    private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e) {
        PlaySound(audioClipRefs.objectDrop, (sender as BaseCounter).transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e) {
        PlaySound(audioClipRefs.chop, (sender as CuttingCounter).transform.position);
    }
    private void DeliveryManager_OnRecipeSuccess() {
        PlaySound(audioClipRefs.deliverySuccess, DeliveryCounter.Instance.transform.position);
    }

    private void DeliveryManager_OnRecipeFailed() {
        PlaySound(audioClipRefs.deliveryFail, DeliveryCounter.Instance.transform.position);
    }

    public void PlaySound(AudioClip audioClip, Vector3 position, float VolumeMultiplier = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, VolumeMultiplier * volume);
    }
    public void PlaySound(AudioClip[] audioClipArray, Vector3 position, float VolumeMultiplier = 1f) {
        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0,audioClipArray.Length)], position, VolumeMultiplier * volume);
    }
    public void PlayFootstepsSound(Vector3 position, float volume) {
        PlaySound(audioClipRefs.footstep, position, volume);
    }
    public void PlayCountDownSound() {
        PlaySound(audioClipRefs.warning, Vector3.zero, volume);
    }
    public void PlayWarningSound(Vector3 position) {
        PlaySound(audioClipRefs.warning, position, volume);
    }
    public float GetVolume() => volume;
    public void ChangeVolume() {
        volume += 0.1f;
        if (volume > 1.01f) volume = 0f;
        else if (volume > 1.00f) volume = 1f;
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }
}
