using UnityEngine;

public class ColorBlindnessController : MonoBehaviour
{
    [Header("References")]
    public Material filterMaterial;
    private static readonly int MatrixPropID = Shader.PropertyToID("_ColorMatrix");

    // --- הגדרת המטריצות (המטרות הסופיות) ---
    private readonly Matrix4x4 normalVision = Matrix4x4.identity;

    private readonly Matrix4x4 protanopia = new Matrix4x4( // אדום חסר
        new Vector4(0.567f, 0.558f, 0.0f, 0.0f),
        new Vector4(0.433f, 0.442f, 0.242f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.758f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    private readonly Matrix4x4 deuteranopia = new Matrix4x4( // ירוק חסר
        new Vector4(0.625f, 0.7f, 0.0f, 0.0f),
        new Vector4(0.375f, 0.3f, 0.3f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.7f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    private readonly Matrix4x4 tritanopia = new Matrix4x4( // כחול חסר
        new Vector4(0.95f, 0.0f, 0.0f, 0.0f),
        new Vector4(0.05f, 0.433f, 0.475f, 0.0f),
        new Vector4(0.0f, 0.567f, 0.525f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    private readonly Matrix4x4 achromatopsia = new Matrix4x4( // שחור לבן
        new Vector4(0.299f, 0.299f, 0.299f, 0.0f),
        new Vector4(0.587f, 0.587f, 0.587f, 0.0f),
        new Vector4(0.114f, 0.114f, 0.114f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    // --- המצבים המצומצמים ---
    public enum ColorBlindMode
    {
        Normal,            // רגיל (מתעלם מהסליידר)
        Protanomaly,       // אדום (מושפע מהסליידר)
        Deuteranomaly,     // ירוק (מושפע מהסליידר)
        Tritanomaly,       // כחול (מושפע מהסליידר)
        RodMonochromacy    // שחור לבן (מתעלם מהסליידר - תמיד מלא)
    }

    [Header("Settings")]
    public ColorBlindMode currentMode = ColorBlindMode.Normal;

    [Range(0f, 1f)] 
    public float severity = 1.0f; // הערך הזה ישתנה על ידי הסליידר ב-UI

    void Update()
    {
        // חישוב המטריצה בכל פריים מאפשר תגובה חלקה להזזת הסליידר
        UpdateShader();
    }

    void UpdateShader()
    {
        Matrix4x4 targetMatrix = normalVision;
        float activeSeverity = severity; // משתנה מקומי לחישוב

        switch (currentMode)
        {
            case ColorBlindMode.Normal:
                targetMatrix = normalVision;
                activeSeverity = 0f; // במצב רגיל אין השפעה
                break;

            case ColorBlindMode.Protanomaly:
                targetMatrix = protanopia; 
                // משתמשים ב-severity מהסליידר
                break;

            case ColorBlindMode.Deuteranomaly:
                targetMatrix = deuteranopia;
                // משתמשים ב-severity מהסליידר
                break;

            case ColorBlindMode.Tritanomaly:
                targetMatrix = tritanopia;
                // משתמשים ב-severity מהסליידר
                break;

            case ColorBlindMode.RodMonochromacy:
                targetMatrix = achromatopsia;
                activeSeverity = 1.0f; // במצב הזה תמיד 100% (מתעלם מהסליידר)
                break;
        }

        // כאן הקסם קורה: אינטרפולציה בין ראייה רגילה למטריצת היעד לפי העוצמה
        Matrix4x4 finalMatrix = LerpMatrix(normalVision, targetMatrix, activeSeverity);

        if (filterMaterial != null)
        {
            filterMaterial.SetMatrix(MatrixPropID, finalMatrix);
        }
    }

    // --- פונקציות לחיבור לכפתורי ה-UI ---
    
    public void SetNormal() => currentMode = ColorBlindMode.Normal;
    
    public void SetProtanomaly() => currentMode = ColorBlindMode.Protanomaly;
    
    public void SetDeuteranomaly() => currentMode = ColorBlindMode.Deuteranomaly;
    
    public void SetTritanomaly() => currentMode = ColorBlindMode.Tritanomaly;
    
    public void SetRodMonochromacy() => currentMode = ColorBlindMode.RodMonochromacy;

    // פונקציה לחיבור ל-Slider (OnValueChanged)
    public void SetSeverity(float value)
    {
        severity = value;
    }

    // --- מתמטיקה ---
    private Matrix4x4 LerpMatrix(Matrix4x4 a, Matrix4x4 b, float t)
    {
        Matrix4x4 result = new Matrix4x4();
        for (int i = 0; i < 16; i++)
        {
            result[i] = Mathf.Lerp(a[i], b[i], t);
        }
        return result;
    }
}