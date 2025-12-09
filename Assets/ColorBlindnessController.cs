using UnityEngine;

public class ColorBlindnessController : MonoBehaviour
{
    [Header("References")]
    public Material filterMaterial;
    private static readonly int MatrixPropID = Shader.PropertyToID("_ColorMatrix");

    // --- הגדרות בסיס ---
    private readonly Matrix4x4 normalVision = Matrix4x4.identity;

    // --- עיוורון מלא (Opia) ---
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

    // --- עיוורון מוחלט (Monochromacy) ---
    // שימוש בנוסחת Luminance (בהירות) המקובלת: R*0.299 + G*0.587 + B*0.114
    private readonly Matrix4x4 achromatopsia = new Matrix4x4(
        new Vector4(0.299f, 0.299f, 0.299f, 0.0f),
        new Vector4(0.587f, 0.587f, 0.587f, 0.0f),
        new Vector4(0.114f, 0.114f, 0.114f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    public enum ColorBlindMode
    {
        Normal,
        Protanopia,     // אדום חסר
        Protanomaly,    // אדום חלש
        Deuteranopia,   // ירוק חסר
        Deuteranomaly,  // ירוק חלש
        Tritanopia,     // כחול חסר
        Tritanomaly,    // כחול חלש
        RodMonochromacy,  // עיוורון מוחלט (ראיית לילה/מקלות)
        ConeMonochromacy  // עיוורון מוחלט (ראיית יום/מדוכים - נדיר מאוד)
    }

    [Header("Current Settings")]
    public ColorBlindMode currentMode = ColorBlindMode.Normal;

    // רמת החומרה עבור ה-Anomalies (בין 0 ל-1. 0.6 מדמה לקות משמעותית)
    private float anomalySeverity = 0.6f;

    void Start()
    {
        SetNormal();
    }

    public void SetMode(ColorBlindMode mode)
    {
        currentMode = mode;
        Matrix4x4 selectedMatrix = normalVision;

        switch (mode)
        {
            case ColorBlindMode.Normal:
                selectedMatrix = normalVision;
                break;

            // --- סוגי האדום ---
            case ColorBlindMode.Protanopia:
                selectedMatrix = protanopia;
                break;
            case ColorBlindMode.Protanomaly:
                // ערבוב בין ראייה רגילה לעיוורון אדום
                selectedMatrix = LerpMatrix(normalVision, protanopia, anomalySeverity);
                break;

            // --- סוגי הירוק ---
            case ColorBlindMode.Deuteranopia:
                selectedMatrix = deuteranopia;
                break;
            case ColorBlindMode.Deuteranomaly:
                selectedMatrix = LerpMatrix(normalVision, deuteranopia, anomalySeverity);
                break;

            // --- סוגי הכחול ---
            case ColorBlindMode.Tritanopia:
                selectedMatrix = tritanopia;
                break;
            case ColorBlindMode.Tritanomaly:
                selectedMatrix = LerpMatrix(normalVision, tritanopia, anomalySeverity);
                break;

            // --- עיוורון מוחלט ---
            case ColorBlindMode.RodMonochromacy:
            case ColorBlindMode.ConeMonochromacy:
                // בסימולציה ויזואלית בסיסית, שניהם נראים כגווני אפור
                selectedMatrix = achromatopsia; 
                break;
        }

        if (filterMaterial != null)
        {
            filterMaterial.SetMatrix(MatrixPropID, selectedMatrix);
        }
    }

    // --- פונקציות עזר ל-UI (לחבר לכפתורים) ---
    public void SetNormal() => SetMode(ColorBlindMode.Normal);
    
    public void SetProtanopia() => SetMode(ColorBlindMode.Protanopia);
    public void SetProtanomaly() => SetMode(ColorBlindMode.Protanomaly);
    
    public void SetDeuteranopia() => SetMode(ColorBlindMode.Deuteranopia);
    public void SetDeuteranomaly() => SetMode(ColorBlindMode.Deuteranomaly);
    
    public void SetTritanopia() => SetMode(ColorBlindMode.Tritanopia);
    public void SetTritanomaly() => SetMode(ColorBlindMode.Tritanomaly);

    public void SetMonochromacy() => SetMode(ColorBlindMode.RodMonochromacy); // כפתור אחד לשחור-לבן

    // --- פונקציית עזר לחישוב מתמטי ---
    private Matrix4x4 LerpMatrix(Matrix4x4 a, Matrix4x4 b, float t)
    {
        Matrix4x4 result = new Matrix4x4();
        for (int i = 0; i < 16; i++)
        {
            result[i] = Mathf.Lerp(a[i], b[i], t);
        }
        return result;
    }

    private void OnValidate()
    {
        if(Application.isPlaying) SetMode(currentMode);
    }
}