using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class ARContentPlacer : MonoBehaviour
{
    [Header("AR Components")]
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    [Header("UI")]
    public GameObject instructionText;

    // האובייקט הנוכחי שאנחנו מנסים למקם (משתנה דינמית)
    private GameObject currentObjectToPlace;
    
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isPlaced = false;

    void Start()
    {
        if (instructionText != null) instructionText.SetActive(false);
    }

    // --- השינוי העיקרי: הפונקציה מקבלת את האובייקט ---
    public void StartPlacementProcess(GameObject targetObject)
    {
        Debug.Log("--- AR Placement Process Started ---");
        
        // 1. שומרים איזה אובייקט אנחנו רוצים למקם הפעם
        currentObjectToPlace = targetObject;
        
        // 2. מאפסים משתנים
        isPlaced = false;
        
        // 3. מכבים את האובייקט (כדי שלא ירחף לפני המיקום)
        if(currentObjectToPlace != null) 
            currentObjectToPlace.SetActive(false);
        
        // 4. מציגים הוראות ומפעילים סריקה
        if (instructionText != null) instructionText.SetActive(true);
        
        planeManager.enabled = true;
        SetPlanesActive(true);
    }

    void Update()
    {
        if (isPlaced || currentObjectToPlace == null) return;

        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            Vector2 touchPosition;
            if (Input.touchCount > 0) touchPosition = Input.GetTouch(0).position;
            else touchPosition = Input.mousePosition;

            bool isTouchBegan = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
            bool isMouseClick = (Input.touchCount == 0 && Input.GetMouseButtonDown(0));

            if (isTouchBegan || isMouseClick)
            {
                if (IsPointerOverUI(touchPosition)) return;

                if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    PlaceObject(hits[0].pose);
                }
            }
        }
    }

    void PlaceObject(Pose hitPose)
    {
        isPlaced = true;
        if (instructionText != null) instructionText.SetActive(false);

        // מפעילים וממקמים את האובייקט שנבחר
        currentObjectToPlace.SetActive(true);
        currentObjectToPlace.transform.position = hitPose.position;

        // רוטציה - מתאים לקירות (Canvas) וגם למודלים
        currentObjectToPlace.transform.rotation = Quaternion.LookRotation(hitPose.up, Vector3.up);

        SetPlanesActive(false);
        planeManager.enabled = false;

        // בדיקה ספציפית: אם האובייקט הוא המשחק, נפעיל אותו
        // (אם זה המודל של העין, הפקודה הזו פשוט לא תעשה כלום וזה בסדר גמור)
        var gameManager = currentObjectToPlace.GetComponentInChildren<IshiharaGameManager>();
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
    }

    bool IsPointerOverUI(Vector2 pos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    void SetPlanesActive(bool isActive)
    {
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(isActive);
        }
    }
}