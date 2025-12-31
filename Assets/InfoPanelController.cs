using UnityEngine;
using TMPro; // חובה לטקסט

public class InfoPanelController : MonoBehaviour
{
    [Header("References")]
    public GameObject infoPanel;            // הפאנל עצמו
    public TextMeshProUGUI titleText;       // מקום לכותרת
    public TextMeshProUGUI bodyText;        // מקום להסבר
    public ColorBlindnessController controller; // חיבור לבקר הראשי

    // משתנה כדי לזכור מה היה המצב בפריים הקודם
    private ColorBlindnessController.ColorBlindMode lastMode;

    void Start()
    {
        // התחלה: החלונית סגורה
        if(infoPanel != null) infoPanel.SetActive(false);
        
        // נשמור את המצב הנוכחי
        if(controller != null) lastMode = controller.currentMode;
    }

    void Update()
    {
        // אם החלונית סגורה, אין טעם לבדוק ולבזבז משאבים
        if (infoPanel.activeSelf == false) return;

        // בדיקה: האם המצב השתנה מאז הפעם האחרונה שבדקנו?
        if (controller != null && controller.currentMode != lastMode)
        {
            // כן, המצב השתנה! בוא נעדכן את הטקסט
            UpdateContent();
            
            // נעדכן את ה"זיכרון" שלנו למצב החדש
            lastMode = controller.currentMode;
        }
    }

    // פונקציה לפתיחה/סגירה של החלונית (לחבר לכפתור Info)
    public void ToggleInfoPanel()
    {
        bool isActive = !infoPanel.activeSelf;
        infoPanel.SetActive(isActive);

        if (isActive)
        {
            // אם הרגע פתחנו, נעדכן מיד את הטקסט שיהיה מעודכן
            UpdateContent();
            // ונעדכן את הזיכרון
            if(controller != null) lastMode = controller.currentMode;
        }
    }

    // פונקציה לסגירה בלבד (לחבר לכפתור ה-X)
    public void ClosePanel()
    {
        infoPanel.SetActive(false);
    }

    // הלב של הסקריפט: בחירת הטקסט לפי המצב
    void UpdateContent()
    {
        if (controller == null) return;

        string title = "";
        string description = "";

        switch (controller.currentMode)
        {
            case ColorBlindnessController.ColorBlindMode.Normal:
                title = "ראייה תקינה (Normal Vision)";
                description = "מצב זה מדמה ראייה אנושית סטנדרטית, הכוללת רגישות מלאה לשלושת צבעי היסוד: אדום, ירוק וכחול (Trichromacy).";
                break;

            case ColorBlindnessController.ColorBlindMode.Protanomaly:
                title = "פרוטנומליה (Protanomaly)";
                description = "לקות בראיית הצבע האדום. המדוכים האדומים קיימים אך רגישותם פחותה. אדום, כתום וצהוב נראים פחות בהירים ונוטים לגוון ירקרק. זהו סוג נפוץ יחסית של עיוורון צבעים.";
                break;

            case ColorBlindnessController.ColorBlindMode.Deuteranomaly:
                title = "דאוטרנומליה (Deuteranomaly)";
                description = "הסוג הנפוץ ביותר של עיוורון צבעים (כ-5% מהגברים). המדוכים הירוקים קיימים אך תפקודם לקוי. קיים קושי להבדיל בין גוונים של ירוק ואדום, והעולם נראה מעט יותר 'דהוי'.";
                break;

            case ColorBlindnessController.ColorBlindMode.Tritanomaly:
                title = "טריטנומליה (Tritanomaly)";
                description = "מצב נדיר המשפיע על ראיית הצבע הכחול. קיים קושי להבדיל בין כחול לירוק ובין צהוב לאדום. הכחול עשוי להיראות ירקרק והצהוב עשוי להיראות ורדרד.";
                break;

            case ColorBlindnessController.ColorBlindMode.Monochromacy:
                title = "מונוכרומטיות (Monochromacy)";
                description = "עיוורון צבעים מוחלט. במצב זה המדוכים (Cones) אינם מתפקדים כלל, והראייה מסתמכת רק על הקנים (Rods). העולם נראה בגווני אפור (שחור-לבן), בדומה לראיית לילה.";
                break;
                
            // הוספתי גם את המצבים האחרים למקרה שתשתמש בהם בעתיד
            default:
                title = controller.currentMode.ToString();
                description = "אין מידע זמין עבור מצב זה.";
                break;
        }

        titleText.text = title;
        bodyText.text = description;
    }
}