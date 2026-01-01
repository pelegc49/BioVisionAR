using UnityEngine;
using UnityEngine.UI;

public class MenuStyler : MonoBehaviour
{
    [Header("Configuration")]
    public Button[] menuButtons; // רשימת כל הכפתורים בתפריט
    
    [Header("Colors")]
    public Color normalColor = Color.white;      // צבע של כפתור כבוי
    public Color activeColor = new Color(0.6f, 0.6f, 0.6f); // צבע של כפתור דלוק (אפור כהה)

    void Start()
    {
        // ברירת מחדל: הכפתור הראשון דלוק
        if (menuButtons.Length > 0)
        {
            SetButtonActive(menuButtons[0]);
        }
    }

    public void OnButtonClicked(Button clickedButton)
    {
        // כאן היה הרטט - ומחקנו אותו.
        // נשארנו רק עם הלוגיקה של הצבעים:
        SetButtonActive(clickedButton);
    }

    void SetButtonActive(Button activeBtn)
    {
        foreach (Button btn in menuButtons)
        {
            Image btnImage = btn.GetComponent<Image>();
            
            if (btnImage != null)
            {
                if (btn == activeBtn)
                {
                    // הכפתור שנבחר - כהה
                    btnImage.color = activeColor;
                }
                else
                {
                    // האחרים - רגיל
                    btnImage.color = normalColor;
                }
            }
        }
    }
}