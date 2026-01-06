using UnityEngine;
using UnityEngine.InputSystem; // חובה למערכת החדשה

public class GlobalSoundManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip clickClip; // גרור לכאן את הקובץ שהורדת
    [Range(0.1f, 1f)]
    public float volume = 0.5f; // עוצמת שמע

    private AudioSource audioSource;

    void Start()
    {
        // יצירה אוטומטית של רכיב השמע אם לא קיים
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // הגדרות בסיסיות למקור השמע
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        bool isPressed = false;

        // 1. בדיקת מגע (לנייד)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            isPressed = true;
        }
        // 2. בדיקת עכבר (למחשב)
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isPressed = true;
        }

        // אם זוהתה לחיצה - נגן צליל
        if (isPressed)
        {
            PlayClickSound();
        }
    }

    void PlayClickSound()
    {
        if (clickClip == null) return;

        // טריק קטן: שינוי רנדומלי עדין של ה-Pitch (בין 0.95 ל-1.05)
        // זה גורם לצליל להרגיש יותר "אורגני" ופחות חופר
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        
        // PlayOneShot מאפשר לנגן את הצליל גם אם הקודם עדיין לא נגמר (לחיצות מהירות)
        audioSource.PlayOneShot(clickClip, volume);
    }
}