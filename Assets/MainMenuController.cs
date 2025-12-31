using UnityEngine;
using UnityEngine.SceneManagement; // חובה בשביל מעבר סצנות

public class MainMenuController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    public string arSceneName = "ARScene"; // שם הסצנה הראשית שלך (תשנה בהתאם)
    public GameObject aboutPanel; // החלונית של "אודות"

    void Start()
    {
        // מוודאים שהאודות סגור בהתחלה
        if (aboutPanel != null) aboutPanel.SetActive(false);
    }

    // כפתור התחלה
    public void StartExperience()
    {
        SceneManager.LoadScene(arSceneName);
    }

    // כפתור אודות (פותח/סוגר)
    public void ToggleAbout()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(!aboutPanel.activeSelf);
        }
    }

    // כפתור יציאה
    public void QuitApp()
    {
        Debug.Log("Quitting Application..."); // עובד רק ב-Editor
        Application.Quit();
    }
}