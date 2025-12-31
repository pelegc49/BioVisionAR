using UnityEngine;
using UnityEngine.UI; // חובה בשביל לשלוט על Material של תמונה

public class IntroBackgroundEffect : MonoBehaviour
{
    [Header("Settings")]
    public float switchInterval = 3.0f; // כל כמה שניות להחליף
    public float transitionDuration = 1.0f; // כמה זמן לוקח המעבר

    private Image backgroundImage;
    private Material bgMaterial;
    private static readonly int MatrixPropID = Shader.PropertyToID("_ColorMatrix");

    // --- הגדרת המטריצות (אותן מטריצות מהקוד הקודם) ---
    private readonly Matrix4x4 normal = Matrix4x4.identity;
    
    private readonly Matrix4x4 protanopia = new Matrix4x4(
        new Vector4(0.567f, 0.558f, 0.0f, 0.0f), new Vector4(0.433f, 0.442f, 0.242f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.758f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

    private readonly Matrix4x4 deuteranopia = new Matrix4x4(
        new Vector4(0.625f, 0.7f, 0.0f, 0.0f), new Vector4(0.375f, 0.3f, 0.3f, 0.0f),
        new Vector4(0.0f, 0.0f, 0.7f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

    private readonly Matrix4x4 tritanopia = new Matrix4x4(
        new Vector4(0.95f, 0.0f, 0.0f, 0.0f), new Vector4(0.05f, 0.433f, 0.475f, 0.0f),
        new Vector4(0.0f, 0.567f, 0.525f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

    private Matrix4x4[] modes;
    private int currentIndex = 0;
    private float timer = 0;
    private bool isTransitioning = false;
    private float transitionTimer = 0;

    void Start()
    {
        backgroundImage = GetComponent<Image>();
        
        // אנחנו משתמשים במופע (Instance) של המטריאל כדי לא לשנות את הקובץ המקורי
        bgMaterial = new Material(backgroundImage.material);
        backgroundImage.material = bgMaterial;

        // סדר המעבר: רגיל -> אדום -> ירוק -> כחול
        modes = new Matrix4x4[] { normal, protanopia, deuteranopia, tritanopia };
    }

    void Update()
    {
        timer += Time.deltaTime;

        // אם הגיע הזמן להחליף...
        if (!isTransitioning && timer >= switchInterval)
        {
            isTransitioning = true;
            transitionTimer = 0;
            timer = 0;
            
            // קידום לאינדקס הבא (בצורה מעגלית)
            currentIndex = (currentIndex + 1) % modes.Length;
        }

        // ביצוע המעבר החלק
        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;
            float t = transitionTimer / transitionDuration; // ערך בין 0 ל-1

            // מאיזה מצב אנחנו יוצאים? (הקודם)
            int prevIndex = (currentIndex - 1 + modes.Length) % modes.Length;
            
            // חישוב המטריצה בנקודת הזמן הזו
            Matrix4x4 currentMatrix = LerpMatrix(modes[prevIndex], modes[currentIndex], t);
            
            bgMaterial.SetMatrix(MatrixPropID, currentMatrix);

            // סיום המעבר
            if (transitionTimer >= transitionDuration)
            {
                isTransitioning = false;
                bgMaterial.SetMatrix(MatrixPropID, modes[currentIndex]); // קיבוע סופי
            }
        }
    }

    private Matrix4x4 LerpMatrix(Matrix4x4 a, Matrix4x4 b, float t)
    {
        Matrix4x4 result = new Matrix4x4();
        for (int i = 0; i < 16; i++) result[i] = Mathf.Lerp(a[i], b[i], t);
        return result;
    }
}