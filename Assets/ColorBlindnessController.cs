using UnityEngine;

public class ColorBlindnessController : MonoBehaviour
{
    [Header("References")]
    public Material filterMaterial;
    // גרור לכאן את ה-GameObject שמחזיק את כפתורי הצבעים (האדום/ירוק/כחול)
    public GameObject monoSubMenuContainer; 

    private static readonly int MatrixPropID = Shader.PropertyToID("_ColorMatrix");

    // --- מטריצות ---
    private readonly Matrix4x4 normalVision = Matrix4x4.identity;

    private readonly Matrix4x4 protanopia = new Matrix4x4(
        new Vector4(0.567f, 0.558f, 0.0f, 0.0f),
        new Vector4(0.433f, 0.442f, 0.242f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.758f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    private readonly Matrix4x4 deuteranopia = new Matrix4x4(
        new Vector4(0.625f, 0.7f, 0.0f, 0.0f),
        new Vector4(0.375f, 0.3f, 0.3f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.7f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    private readonly Matrix4x4 tritanopia = new Matrix4x4(
        new Vector4(0.95f, 0.0f, 0.0f, 0.0f),
        new Vector4(0.05f, 0.433f, 0.475f, 0.0f),
        new Vector4(0.0f, 0.567f, 0.525f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    // --- מטריצות מונוכרומטיות (גוון אחד בלבד) ---
    // הנוסחה: לוקחים את הבהירות (Luminance) ודוחפים אותה רק לערוץ אחד
    
    // אפור (Rod Monochromacy)
// אפור (Rod Monochromacy) - נשאר תקין כי כל הערוצים מקבלים אותו דבר
    private readonly Matrix4x4 monoGray = new Matrix4x4(
        new Vector4(0.299f, 0.299f, 0.299f, 0.0f), // Column 0 (Input Red contributes to R, G, B)
        new Vector4(0.587f, 0.587f, 0.587f, 0.0f), // Column 1 (Input Green contributes to R, G, B)
        new Vector4(0.114f, 0.114f, 0.114f, 0.0f), // Column 2 (Input Blue contributes to R, G, B)
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    // רק אדום (Red Cone Monochromacy)
    // אנחנו רוצים שכל המידע ילך רק ל-Output Red (שורה ראשונה במטריצה - X של הווקטורים)
    private readonly Matrix4x4 monoRed = new Matrix4x4(
        new Vector4(0.299f, 0.0f, 0.0f, 0.0f), // Column 0: Input Red only fills Output Red
        new Vector4(0.587f, 0.0f, 0.0f, 0.0f), // Column 1: Input Green only fills Output Red
        new Vector4(0.114f, 0.0f, 0.0f, 0.0f), // Column 2: Input Blue only fills Output Red
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    // רק ירוק (Green Cone Monochromacy)
    // כל המידע הולך רק ל-Output Green (שורה שנייה במטריצה - Y של הווקטורים)
    private readonly Matrix4x4 monoGreen = new Matrix4x4(
        new Vector4(0.0f, 0.299f, 0.0f, 0.0f), 
        new Vector4(0.0f, 0.587f, 0.0f, 0.0f), 
        new Vector4(0.0f, 0.114f, 0.0f, 0.0f), 
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    // רק כחול (Blue Cone Monochromacy)
    // כל המידע הולך רק ל-Output Blue (שורה שלישית במטריצה - Z של הווקטורים)
    private readonly Matrix4x4 monoBlue = new Matrix4x4(
        new Vector4(0.0f, 0.0f, 0.299f, 0.0f), 
        new Vector4(0.0f, 0.0f, 0.587f, 0.0f), 
        new Vector4(0.0f, 0.0f, 0.114f, 0.0f), 
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    public enum ColorBlindMode
    {
        Normal,
        Protanomaly,
        Deuteranomaly,
        Tritanomaly,
        Monochromacy // מצב כללי
    }

    public enum MonoType
    {
        Gray,   // Rods
        Red,    // Red Cone
        Green,  // Green Cone
        Blue    // Blue Cone
    }

    [Header("Settings")]
    public ColorBlindMode currentMode = ColorBlindMode.Normal;
    public MonoType currentMonoType = MonoType.Gray; // תת-מצב

    [Range(0f, 1f)] 
    public float severity = 1.0f; 

    void Update()
    {
        UpdateShader();
    }

    void UpdateShader()
    {
        Matrix4x4 targetMatrix = normalVision;
        float activeSeverity = severity;

        // ניהול הופעת התפריט המשני
        if(monoSubMenuContainer != null)
        {
            // התפריט יופיע רק אם אנחנו במצב Monochromacy
            bool shouldShowSubMenu = (currentMode == ColorBlindMode.Monochromacy);
            if(monoSubMenuContainer.activeSelf != shouldShowSubMenu)
            {
                monoSubMenuContainer.SetActive(shouldShowSubMenu);
            }
        }

        switch (currentMode)
        {
            case ColorBlindMode.Normal:
                targetMatrix = normalVision;
                activeSeverity = 0f;
                break;

            case ColorBlindMode.Protanomaly:
                targetMatrix = protanopia; 
                break;

            case ColorBlindMode.Deuteranomaly:
                targetMatrix = deuteranopia;
                break;

            case ColorBlindMode.Tritanomaly:
                targetMatrix = tritanopia;
                break;

            case ColorBlindMode.Monochromacy:
                activeSeverity = 1.0f; // תמיד מלא
                // כאן בודקים איזה תת-סוג נבחר
                switch (currentMonoType)
                {
                    case MonoType.Gray: targetMatrix = monoGray; break;
                    case MonoType.Red: targetMatrix = monoRed; break;
                    case MonoType.Green: targetMatrix = monoGreen; break;
                    case MonoType.Blue: targetMatrix = monoBlue; break;
                }
                break;
        }

        Matrix4x4 finalMatrix = LerpMatrix(normalVision, targetMatrix, activeSeverity);

        if (filterMaterial != null)
        {
            filterMaterial.SetMatrix(MatrixPropID, finalMatrix);
        }
    }

    // --- פונקציות לכפתורים ראשיים ---
    public void SetNormal() => currentMode = ColorBlindMode.Normal;
    public void SetProtanomaly() => currentMode = ColorBlindMode.Protanomaly;
    public void SetDeuteranomaly() => currentMode = ColorBlindMode.Deuteranomaly;
    public void SetTritanomaly() => currentMode = ColorBlindMode.Tritanomaly;
    
    // כפתור זה יפעיל את מצב המונו ויפתח את התפריט המשני
    public void SetMonochromacy() => currentMode = ColorBlindMode.Monochromacy;


    // --- פונקציות לכפתורי המשנה (צבעים) ---
    public void SetMonoGray() => currentMonoType = MonoType.Gray;
    public void SetMonoRed() => currentMonoType = MonoType.Red;
    public void SetMonoGreen() => currentMonoType = MonoType.Green;
    public void SetMonoBlue() => currentMonoType = MonoType.Blue;

    // --- Slider ---
    public void SetSeverity(float value) => severity = value;

    // --- Math ---
    private Matrix4x4 LerpMatrix(Matrix4x4 a, Matrix4x4 b, float t)
    {
        Matrix4x4 result = new Matrix4x4();
        for (int i = 0; i < 16; i++) result[i] = Mathf.Lerp(a[i], b[i], t);
        return result;
    }
}