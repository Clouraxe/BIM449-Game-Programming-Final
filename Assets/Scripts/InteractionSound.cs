using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSound : MonoBehaviour
{
    // Drag and drop the audio clip here in the Inspector
    public AudioClip soundClip;

    // Volume level of the sound (between 0.0 and 1.0)
    [Range(0f, 1f)]
    public float volume = 5.0f;

    void OnMouseDown()
    {
        Debug.Log("Tıklama algılandı! Ses çalınmaya çalışılıyor...");
        // Check if an audio clip is assigned
        if (soundClip != null)
        {
            // PlayClipAtPoint creates a temporary AudioSource at the specified position.
            // This ensures the sound plays completely even if this object is destroyed or disabled immediately.
            AudioSource.PlayClipAtPoint(soundClip, transform.position, volume);
        }
    }
}