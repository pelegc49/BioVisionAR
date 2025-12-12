using UnityEngine;

public class AppModeManager : MonoBehaviour
{
    [Header("UI Panels & Containers")]
    // כפתור 1: הפאנל שמכיל את כפתורי הפילטרים (Protanopia, Slider וכו')
    public GameObject filterMenuPanel; 

    // כפתור 2: האובייקט שמכיל את מודל העין (בעתיד)
    public GameObject eyeModelContainer; 

    // כפתור 3: האובייקט שמכיל את משחק האישיהרה (התמונה, ה-Quad וכו')
    public GameObject ishiharaGameContainer; 

    void Start()
    {
        // מצב התחלתי:
        // התפריט של הפילטרים סגור (או פתוח, איך שתרצה)
        filterMenuPanel.SetActive(false); 

        // ברירת מחדל: מציגים את מודל העין (או כלום)
        ActivateEyeModel();
    }

    // --- כפתור 1: Toggle למערכת הפילטרים ---
    public void ToggleFilterMenu()
    {
        // בודק מה המצב הנוכחי והופך אותו
        bool isActive = filterMenuPanel.activeSelf;
        filterMenuPanel.SetActive(!isActive);
        
        // הערה: פעולה זו רק מחביאה/מציגה את הכפתורים.
        // הפילטר עצמו (הצבעים במסך) נשאר פעיל כי הוא יושב על המצלמה!
    }

    // --- כפתור 2: מודל העין ---
    public void ActivateEyeModel()
    {
        // מציג את העין, מסתיר את המשחק
        eyeModelContainer.SetActive(true);
        ishiharaGameContainer.SetActive(false);
        
        // לא נוגעים ב-filterMenuPanel ולא מאפסים את הפילטרים
    }

    // --- כפתור 3: משחק אישיהרה ---
    public void ActivateIshihara()
    {
        // Toggle למצב משחק אישיהרה בלבד
        bool isActive = ishiharaGameContainer.activeSelf;

        ishiharaGameContainer.SetActive(!isActive);

        // אם עכשיו הצגנו את המשחק – נאפס אותו
        if (!isActive)
        {
            IshiharaGameController game =
                ishiharaGameContainer.GetComponent<IshiharaGameController>();

            if (game != null)
            {
                game.ResetGame();
            }
            else
            {
                Debug.LogError("IshiharaGameController not found on ishiharaGameContainer");
            }
        }
    }

}