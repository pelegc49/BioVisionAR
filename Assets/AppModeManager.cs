using UnityEngine;

public class AppModeManager : MonoBehaviour
{
    [Header("UI Panels & Containers")]
    public GameObject filterMenuPanel; 
    public GameObject eyeModelContainer; // המודל של העין
    public GameObject ishiharaGameContainer; // המשחק

    [Header("AR Controller")]
    public ARContentPlacer arPlacer; // הוספנו הפניה לסקריפט המיקום

    // כפתור 1
    public void ToggleFilterMenu()
    {
        bool isActive = filterMenuPanel.activeSelf;
        filterMenuPanel.SetActive(!isActive);
    }

    // כפתור 2: מודל העין
    public void ActivateEyeModel()
    {
        // 1. מכבים את המודלים האחרים
        filterMenuPanel.SetActive(false);
        ishiharaGameContainer.SetActive(false); 
        
        // 2. מכבים זמנית את העין (ה-Placer ידליק אותה כשנמקם)
        eyeModelContainer.SetActive(false);

        // 3. מתחילים תהליך מיקום עבור העין
        if (arPlacer != null)
        {
            arPlacer.enabled = true;
            // שולחים את המודל של העין למיקום!
            arPlacer.StartPlacementProcess(eyeModelContainer); 
        }
    }

    // כפתור 3: משחק אישיהרה
    public void ActivateIshihara()
    {
        filterMenuPanel.SetActive(false);
        eyeModelContainer.SetActive(false);
        ishiharaGameContainer.SetActive(false);

        // מתחילים תהליך מיקום עבור המשחק
        if (arPlacer != null)
        {
            arPlacer.enabled = true;
            // שולחים את המשחק למיקום!
            arPlacer.StartPlacementProcess(ishiharaGameContainer);
        }
    }
}