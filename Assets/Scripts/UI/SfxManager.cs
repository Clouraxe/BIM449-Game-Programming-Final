using UnityEngine;

public class SfxManager : MonoBehaviour
{

    private AudioSource audioSource;

    public AudioClip winSfx;
    public AudioClip gameOverSfx;
    public AudioClip caughtSfx;
    public AudioClip cheatCompleteSfx;
    public AudioClip cheatFailSfx;
    
    enum SfxType
    {
        Win,
        GameOver,
        Caught,
        CheatComplete,
        CheatFail
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    
    void PlaySfx(AudioClip clip)
    {
        
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}
