using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class IshiharaGameManager : MonoBehaviour
{
    [System.Serializable]
    public struct QuestionData
    {
        public Sprite image;
        public int correctAnswer;
    }

    [Header("Main Panels")]
    public GameObject questionsContainer; // (לשעבר Game Panel) הפאנל עם השאלות
    public GameObject resultsPanel;       // הפאנל עם התוצאות

    [Header("Game Elements")]
    public Image questionDisplay;
    public Button[] answerButtons;
    public Slider progressBar;           // --- חדש: סרגל ההתקדמות ---

    [Header("Results Elements")]
    public TextMeshProUGUI resultsText;
    public Button playAgainButton;       // --- חדש: כפתור שחק שוב ---

    [Header("Data")]
    public QuestionData[] questions;

    // --- Internal State ---
    private int currentQuestionIndex = 0;
    private int correctAnswersCount = 0;
    private float startTime;
    private bool isGameActive = false;

    void Start()
    {
        // אם הכפתור מוגדר, נקשר אותו לפונקציה RestartGame
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(RestartGame);
        }
    }

    // פונקציה שנקראת מה-AR Placer או מהכפתור Restart
    public void StartGame()
    {
        currentQuestionIndex = 0;
        correctAnswersCount = 0;
        startTime = Time.time;
        isGameActive = true;

        // איפוס ה-UI
        questionsContainer.SetActive(true);
        resultsPanel.SetActive(false);

        // --- איפוס Progress Bar ---
        if (progressBar != null)
        {
            progressBar.maxValue = questions.Length;
            progressBar.value = 0;
        }

        LoadQuestion();
    }

    public void RestartGame()
    {
        // פשוט קוראים ל-StartGame מחדש. 
        // בגלל שהקנבס כבר ממוקם ב-World Space, לא צריך את ה-AR Placer שוב.
        StartGame();
    }

    void LoadQuestion()
    {
        // עדכון הסרגל לפני בדיקת הסיום
        if (progressBar != null)
        {
            progressBar.value = currentQuestionIndex;
        }

        if (currentQuestionIndex >= questions.Length)
        {
            EndGame();
            return;
        }

        QuestionData currentQ = questions[currentQuestionIndex];
        questionDisplay.sprite = currentQ.image;

        // הכנת התשובות
        List<int> options = new List<int>();
        options.Add(currentQ.correctAnswer);

        while (options.Count < 4)
        {
            int randomNum = Random.Range(1, 100);
            if (!options.Contains(randomNum)) options.Add(randomNum);
        }

        ShuffleList(options);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int number = options[i];
            TextMeshProUGUI btnText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = number.ToString();

            answerButtons[i].onClick.RemoveAllListeners();
            int selectedNum = number;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(selectedNum));
        }
    }

    void OnAnswerSelected(int selectedNumber)
    {
        if (!isGameActive) return;

        int correctNum = questions[currentQuestionIndex].correctAnswer;
        if (selectedNumber == correctNum)
        {
            correctAnswersCount++;
        }

        currentQuestionIndex++;
        LoadQuestion();
    }

    void EndGame()
    {
        isGameActive = false;
        
        // עדכון סופי של הסרגל שיהיה מלא
        if (progressBar != null) progressBar.value = questions.Length;

        float totalTime = Time.time - startTime;
        string timeStr = totalTime.ToString("F1");

        questionsContainer.SetActive(false); // מכבים את השאלות
        resultsPanel.SetActive(true);        // מדליקים את התוצאות

        resultsText.text = "<b>Game Over!</b>\n\n" +
                           $"Correct Answers: {correctAnswersCount} / {questions.Length}\n" +
                           $"Time: {timeStr}s\n\n" +
                           ((correctAnswersCount >= 8) ? "Great Vision!" : "Consult a doctor :)");
    }

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