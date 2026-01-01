using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageTrackerControl : MonoBehaviour
{
    [Header("AR Components")]
    public ARTrackedImageManager imageManager;
    public ARPlaneManager planeManager;

    [Header("Content")]
    public GameObject objectToControl;
    public GameObject instructionText;

    [Header("Settings")]
    public Vector3 rotationOffset = new Vector3(0, 180, 0); // סיבוב (במעלות)
    public Vector3 positionOffset = Vector3.zero;           // מיקום (במטרים)

    void OnEnable()
    {
        if (imageManager != null) imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        if (imageManager != null) imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        
        if (instructionText != null) instructionText.SetActive(false);
    }

    public void StartScanning(GameObject targetObject)
    {
        objectToControl = targetObject;
        if(objectToControl != null) objectToControl.SetActive(false);
        if(instructionText != null) instructionText.SetActive(true);

        imageManager.enabled = true;

        if (planeManager != null)
        {
            planeManager.enabled = false;
            foreach (var plane in planeManager.trackables) plane.gameObject.SetActive(false);
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added) UpdateModelPosition(trackedImage);
        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking) UpdateModelPosition(trackedImage);
        }
    }

    void UpdateModelPosition(ARTrackedImage trackedImage)
    {
        if (objectToControl == null) return;

        objectToControl.SetActive(true);
        if (instructionText != null) instructionText.SetActive(false);

        // --- חישוב המיקום החדש (עם Offset) ---
        // אנחנו לוקחים את הכיוון של התמונה ומכפילים ב-Offset כדי שזה יהיה יחסי לדף
        // לדוגמה: אם Y=0.05, המודל "ירחף" 5 ס"מ מעל הדף
        Vector3 finalPosition = trackedImage.transform.position + (trackedImage.transform.rotation * positionOffset);
        objectToControl.transform.position = finalPosition;

        // --- חישוב הרוטציה החדשה (עם Offset) ---
        Quaternion offset = Quaternion.Euler(rotationOffset);
        objectToControl.transform.rotation = trackedImage.transform.rotation * offset;
    }
}