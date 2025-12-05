using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorBlindnessController : MonoBehaviour
{
    [Header("References")]
    // גרור לכאן את המטריאל שיצרת (ColorBlindMat)
    public Material filterMaterial; 

    // שמות המשתנים בשיידר
    private static readonly int MatrixPropID = Shader.PropertyToID("_ColorMatrix");

    // --- הגדרת המטריצות (מבוסס על מחקרים אופטיים) ---

    // ראייה רגילה
    private readonly Matrix4x4 normalVision = Matrix4x4.identity;

    // Protanopia (אדום חסר)
    private readonly Matrix4x4 protanopia = new Matrix4x4(
        new Vector4(0.567f, 0.558f, 0.0f, 0.0f), // Column 0 (Red input gets mixed)
        new Vector4(0.433f, 0.442f, 0.242f, 0.0f), // Column 1
        new Vector4(0.0f, 0.0f, 0.758f, 0.0f), // Column 2
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)  // Column 3
    );

    // Deuteranopia (ירוק חסר - הנפוץ ביותר)
    private readonly Matrix4x4 deuteranopia = new Matrix4x4(
        new Vector4(0.625f, 0.7f, 0.0f, 0.0f),
        new Vector4(0.375f, 0.3f, 0.3f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.7f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    // Tritanopia (כחול חסר)
    private readonly Matrix4x4 tritanopia = new Matrix4x4(
        new Vector4(0.95f, 0.0f, 0.0f, 0.0f),
        new Vector4(0.05f, 0.433f, 0.475f, 0.0f),
        new Vector4(0.0f, 0.567f, 0.525f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    public enum ColorBlindMode
    {
        Normal,
        Protanopia,
        Deuteranopia,
        Tritanopia
    }

    [Header("Current Settings")]
    public ColorBlindMode currentMode = ColorBlindMode.Normal;

    void Start()
    {
        // התחלה במצב רגיל
        SetMode(ColorBlindMode.Normal);
    }

    // פונקציה לקריאה מכפתורי UI
    public void SetMode(ColorBlindMode mode)
    {
        currentMode = mode;
        Matrix4x4 selectedMatrix = normalVision;

        switch (mode)
        {
            case ColorBlindMode.Protanopia:
                selectedMatrix = protanopia;
                break;
            case ColorBlindMode.Deuteranopia:
                selectedMatrix = deuteranopia;
                break;
            case ColorBlindMode.Tritanopia:
                selectedMatrix = tritanopia;
                break;
        }

        // שליחת המטריצה לשיידר
        if (filterMaterial != null)
        {
            filterMaterial.SetMatrix(MatrixPropID, selectedMatrix);
        }
    }
    
    // פונקציה שתאפשר לך לבדוק דרך ה-Inspector בזמן אמת
    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            SetMode(currentMode);
        }
    }
}