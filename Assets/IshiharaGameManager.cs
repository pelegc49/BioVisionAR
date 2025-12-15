using UnityEngine;
using UnityEngine.UI;
using TMPro; // חובה בשביל הטקסטים החדשים
using System.Collections.Generic; // בשביל רשימות

public class IshiharaGameManager : MonoBehaviour
{
    [System.Serializable]
    public struct QuestionData
    {
        public Sprite image;      // התמונה של האישיהרה
        public int correctAnswer; // המספר שמוחבא בתמונה
    }

    [Header("UI References")]
    public Image questionDisplay;          // הרכיב שמציג את התמונה
    public Button[] answerButtons;         // מערך של 4 הכפתורים
    public GameObject gamePanel;           // הפאנל הראשי של המשחק
    public GameObject resultsPanel;        // פאנל הסיום
    public TextMeshProUGUI resultsText;    // הטקסט של הסיכום

    [Header("Data")]
    public QuestionData[] questions;       // כאן נגרור את התמונות והתשובות ב-Inspector

    // --- משתנים לניהול המשחק ---
    private int currentQuestionIndex = 0;
    private int correctAnswersCount = 0;
    private float startTime;
    private bool isGameActive = false;

    void Start()
    {
        // אנחנו נפעיל את המשחק ידנית דרך ה-AppManager, אז לא צריך כלום ב-Start
    }

    // פונקציה שנקראת כשלוחצים על כפתור התחלת המשחק (Mode3)
    public void StartGame()
    {
        currentQuestionIndex = 0;
        correctAnswersCount = 0;
        startTime = Time.time;
        isGameActive = true;

        gamePanel.SetActive(true);
        resultsPanel.SetActive(false);

        LoadQuestion();
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex >= questions.Length)
        {
            EndGame();
            return;
        }

        // 1. טעינת התמונה
        QuestionData currentQ = questions[currentQuestionIndex];
        questionDisplay.sprite = currentQ.image;

        // 2. הכנת התשובות (1 נכונה + 3 שגויות)
        List<int> options = new List<int>();
        options.Add(currentQ.correctAnswer);

        // יצירת 3 מסיחים אקראיים (שלא שווים לתשובה הנכונה וללא כפילויות)
        while (options.Count < 4)
        {
            // טווח מספרים הגיוני (למשל בין 1 ל-99)
            int randomNum = Random.Range(1, 100); 
            if (!options.Contains(randomNum))
            {
                options.Add(randomNum);
            }
        }

        // 3. ערבוב התשובות (Shuffle) כדי שהנכונה לא תהיה תמיד הראשונה
        ShuffleList(options);

        // 4. הצבת המספרים על הכפתורים
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int number = options[i];
            
            // שינוי הטקסט של הכפתור
            TextMeshProUGUI btnText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = number.ToString();

            // ניקוי מאזינים קודמים (חשוב!)
            answerButtons[i].onClick.RemoveAllListeners();

            // הוספת מאזין לחיצה חדש
            // אנחנו משתמשים במשתנה זמני כדי שה-Lambda תתפוס את הערך הנכון
            int selectedNum = number; 
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(selectedNum));
        }
    }

    void OnAnswerSelected(int selectedNumber)
    {
        if (!isGameActive) return;

        // בדיקה אם צדק
        int correctNum = questions[currentQuestionIndex].correctAnswer;
        if (selectedNumber == correctNum)
        {
            correctAnswersCount++;
            Debug.Log("Correct!");
        }
        else
        {
            Debug.Log("Wrong! Chose " + selectedNumber + ", was " + correctNum);
        }

        // מעבר לשאלה הבאה
        currentQuestionIndex++;
        LoadQuestion();
    }

    void EndGame()
    {
        isGameActive = false;
        float totalTime = Time.time - startTime;

        // עיצוב זמן (למשל: 12.5 שניות)
        string timeStr = totalTime.ToString("F1");

        // הצגת סיכום
        gamePanel.SetActive(false); // מסתירים את השאלות
        resultsPanel.SetActive(true); // מציגים את התוצאות

        resultsText.text = "<b>Game Over!</b>\n\n" +
                           $"Correct Answers: {correctAnswersCount} / {questions.Length}\n" +
                           $"Total Time: {timeStr}s\n\n" +
                           ((correctAnswersCount >= 8) ? "Great Vision!" : "Consult a doctor :)");
    }

    // פונקציית עזר לערבוב רשימה (Fisher-Yates Shuffle)
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}