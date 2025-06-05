using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    private AudioSource audioSource;
    private float volume = 0.2f;
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    private void Awake() {
        Instance = this; 
        audioSource = GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.3f);
        audioSource.volume = volume;
    }
    public float GetVolume() => volume;
    public void ChangeVolume() {
        volume += 0.1f;
        if (volume > 1.01f) volume = 0f;
        else if (volume > 1.00f) volume = 1f;
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }
}
