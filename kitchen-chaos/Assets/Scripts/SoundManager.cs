using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClipRefsSO audioClipRefs;
    private void Awake() {
        Instance = this;
    }
    private void Start() {
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPickedSomething += Instance_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e) {
        PlaySound(audioClipRefs.trash, (sender as TrashCounter).transform.position);

    }

    private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e) {
        PlaySound(audioClipRefs.objectDrop, (sender as BaseCounter).transform.position);
    }

    private void Instance_OnPickedSomething(object sender, System.EventArgs e) {
        PlaySound(audioClipRefs.objectPickup, (sender as Player).transform.position);
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

    public void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
    public void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0,audioClipArray.Length)], position, volume);
    }
    public void PlayFootstepsSound(Vector3 position, float volume) {
        PlaySound(audioClipRefs.footstep, position, volume);
    }
}
