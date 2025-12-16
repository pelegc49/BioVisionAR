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
    public GameObject questionsContainer; 
    public GameObject resultsPanel;       

    [Header("Game Elements")]
    public Image questionDisplay;
    public Button[] answerButtons;
    public Slider progressBar;           

    [Header("Results Elements")]
    public TextMeshProUGUI resultsText;
    public Button playAgainButton;       

    [Header("Data")]
    public QuestionData[] allQuestions; // שיניתי את השם ל-allQuestions כדי למנוע בלבול

    // --- Internal State ---
    private List<QuestionData> shuffledQuestions; // הרשימה המעורבבת למשחק הנוכחי
    private int currentQuestionIndex = 0;
    private int correctAnswersCount = 0;
    private float startTime;
    private bool isGameActive = false;

    void Start()
    {
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(RestartGame);
        }
    }

    public void StartGame()
    {
        currentQuestionIndex = 0;
        correctAnswersCount = 0;
        startTime = Time.time;
        isGameActive = true;

        // --- ערבוב השאלות (החלק החדש) ---
        // 1. יצירת רשימה חדשה מהמערך המקורי
        shuffledQuestions = new List<QuestionData>(allQuestions);
        
        // 2. ערבוב הרשימה
        ShuffleList(shuffledQuestions);

        // --- איפוס ה-UI ---
        questionsContainer.SetActive(true);
        resultsPanel.SetActive(false);

        if (progressBar != null)
        {
            progressBar.maxValue = shuffledQuestions.Count;
            progressBar.value = 0;
        }

        LoadQuestion();
    }

    public void RestartGame()
    {
        StartGame();
    }

    void LoadQuestion()
    {
        if (progressBar != null)
        {
            progressBar.value = currentQuestionIndex;
        }

        // בדיקה אם סיימנו את כל השאלות ברשימה המעורבבת
        if (currentQuestionIndex >= shuffledQuestions.Count)
        {
            EndGame();
            return;
        }

        // --- שימוש בשאלה מהרשימה המעורבבת ---
        QuestionData currentQ = shuffledQuestions[currentQuestionIndex];
        
        questionDisplay.sprite = currentQ.image;

        // הכנת התשובות
        List<int> options = new List<int>();
        options.Add(currentQ.correctAnswer);

        // יצירת מסיחים
        while (options.Count < 4)
        {
            int randomNum = Random.Range(1, 100);
            if (!options.Contains(randomNum)) options.Add(randomNum);
        }

        // ערבוב מיקום התשובות על הכפתורים
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

        // בדיקה מול השאלה הנוכחית ברשימה המעורבבת
        int correctNum = shuffledQuestions[currentQuestionIndex].correctAnswer;
        
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
        
        if (progressBar != null) progressBar.value = shuffledQuestions.Count;

        float totalTime = Time.time - startTime;
        string timeStr = totalTime.ToString("F1");

        questionsContainer.SetActive(false); 
        resultsPanel.SetActive(true);        

        resultsText.text = "<b>Game Over!</b>\n\n" +
                           $"Correct Answers: {correctAnswersCount} / {shuffledQuestions.Count}\n" +
                           $"Time: {timeStr}s\n\n" +
                           ((correctAnswersCount >= 8) ? "Great Vision!" : "Consult a doctor :)");
    }

    // פונקציית הערבוב הגנרית (עובדת גם על שאלות וגם על תשובות)
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