using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation; // חובה בשביל ARSession
using System.Collections; // חובה בשביל Coroutines

public class AppModeManager : MonoBehaviour
{
    [Header("General AR References")]
    public ARSession arSession; // <--- הוסף את המשתנה הזה
    [Header("UI Panels & Containers")]
    public GameObject filterMenuPanel; 
    public GameObject eyeModelContainer; // The eye model
    public GameObject ishiharaGameContainer; // The game

    [Header("AR Controller")]
    public ARContentPlacer arPlacer; // Reference to the placement script
    public ARImageTrackerControl imageTracker;

    // Button 1
    public void ToggleFilterMenu()
    {
        bool isActive = filterMenuPanel.activeSelf;
        filterMenuPanel.SetActive(!isActive);
    }

    // Button 2: Eye Model
    public void ActivateEyeModel()
    {
        // ניקוי מסך
        filterMenuPanel.SetActive(false);
        ishiharaGameContainer.SetActive(false); 
        eyeModelContainer.SetActive(false);

        // כיבוי ה-Placer הישן (של הקירות)
        if (arPlacer != null) arPlacer.enabled = false;

        // הפעלת ה-Image Tracker החדש
        if (imageTracker != null)
        {
            imageTracker.enabled = true;
            imageTracker.StartScanning(eyeModelContainer);
        }
    }

    // Button 3: Ishihara Game
    public void ActivateIshihara()
    {
        filterMenuPanel.SetActive(false);
        eyeModelContainer.SetActive(false);
        ishiharaGameContainer.SetActive(false);

        // Start placement process for the game
        if (arPlacer != null)
        {
            arPlacer.enabled = true;
            // Send the game for placement!
            arPlacer.StartPlacementProcess(ishiharaGameContainer);
        }
        
        if (imageTracker != null) imageTracker.enabled = false;

        // הפעלת ה-Placer הרגיל
        if (arPlacer != null)
        {
            arPlacer.enabled = true;
            arPlacer.StartPlacementProcess(ishiharaGameContainer);
        }
    }

    public void BackToMainMenu()
    {
        StartCoroutine(SwitchSceneRoutine());
    }

    // תהליך מסודר למעבר סצנה
    IEnumerator SwitchSceneRoutine()
    {
        // 1. קודם כל מכבים את מוח ה-AR
        if (arSession != null)
        {
            arSession.Reset(); // מאפס את ה-AR
            arSession.enabled = false; // מכבה אותו לגמרי
        }

        // 2. מחכים פריים אחד כדי לוודא שיוניטי עיבדה את הכיבוי
        yield return null;

        // 3. עכשיו בטוח לעבור סצנה
        SceneManager.LoadScene("IntroScene");
    }
}