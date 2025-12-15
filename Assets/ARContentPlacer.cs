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

    [Header("Content")]
    public GameObject objectToPlace;
    public GameObject instructionText;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isPlaced = false;

    void Start()
    {
        if (objectToPlace != null) objectToPlace.SetActive(false);
        if (instructionText != null) instructionText.SetActive(false);
    }

    public void StartPlacementProcess()
    {
        Debug.Log("--- AR Placement Process Started ---"); // לוג התחלה
        isPlaced = false;
        if(objectToPlace != null) objectToPlace.SetActive(false);
        if (instructionText != null) instructionText.SetActive(true);
        
        planeManager.enabled = true;
        SetPlanesActive(true);
    }

    void Update()
    {
        if (isPlaced) return;

        // בדיקה משולבת: גם מגע (טלפון) וגם עכבר (מחשב/Editor)
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            Vector2 touchPosition;

            // בדיקה האם זה מגע או עכבר
            if (Input.touchCount > 0)
                touchPosition = Input.GetTouch(0).position;
            else
                touchPosition = Input.mousePosition;

            // לוג לוודא שיוניטי בכלל מזהה לחיצה
            // אם אתה לא רואה את השורה הזו ב-Console כשאתה לוחץ, הבעיה היא ב-Project Settings (שלב 1)
            Debug.Log("Input Detected at: " + touchPosition);

            // טיפול בלחיצה רק בהתחלה (Began / MouseDown)
            bool isTouchBegan = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
            bool isMouseClick = (Input.touchCount == 0 && Input.GetMouseButtonDown(0));

            if (isTouchBegan || isMouseClick)
            {
                if (IsPointerOverUI(touchPosition))
                {
                    Debug.Log("Blocked by UI");
                    return;
                }

                if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    Debug.Log("HIT WALL! Placing object...");
                    PlaceObject(hits[0].pose);
                }
                else
                {
                    Debug.Log("Raycast failed - No plane hit at this position.");
                }
            }
        }
    }

    // void PlaceObject(Pose hitPose)
    // {
    //     isPlaced = true;
    //     if (instructionText != null) instructionText.SetActive(false);

    //     objectToPlace.SetActive(true);
    //     objectToPlace.transform.position = hitPose.position;
    //     objectToPlace.transform.rotation = hitPose.rotation;

    //     SetPlanesActive(false);
    //     planeManager.enabled = false;

    //     var gameManager = objectToPlace.GetComponentInChildren<IshiharaGameManager>();
    //     if (gameManager != null)
    //     {
    //         gameManager.StartGame();
    //     }
    // }

void PlaceObject(Pose hitPose)
    {
        isPlaced = true;
        if (instructionText != null) instructionText.SetActive(false);

        objectToPlace.SetActive(true);
        objectToPlace.transform.position = hitPose.position;

        // --- התיקון הגדול ---
        // במקום להשתמש ב-hitPose.rotation, אנחנו משתמשים ב-hitPose.up
        // ב-AR Planes, ה-Up הוא הנורמל (החץ שיוצא מהקיר החוצה)
        // הפקודה הזו אומרת: "כוון את ציר ה-Z (קדימה) שיהיה מקביל לנורמל של הקיר, ושמור על ציר ה-Y כלפי מעלה העולם"
        objectToPlace.transform.rotation = Quaternion.LookRotation(hitPose.up, Vector3.up);

        // הסתרת משטחים והתחלת משחק
        SetPlanesActive(false);
        planeManager.enabled = false;

        var gameManager = objectToPlace.GetComponentInChildren<IshiharaGameManager>();
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