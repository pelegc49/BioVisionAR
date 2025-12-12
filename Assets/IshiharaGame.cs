using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; 


public class IshiharaGameController : MonoBehaviour
{
    public GameObject rewardPrefab;
    public RectTransform spawnArea;


    [Header("Reset Button")]
    public GameObject resetButton;

    [Header("Progress UI")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    [Header("Images")]
    public GameObject[] images;

    [Header("Buttons")]
    public Button ans1;
    public Button ans2;
    public Button ans3;

    [Header("Button Texts")]
    public TextMeshProUGUI ans1Text;
    public TextMeshProUGUI ans2Text;
    public TextMeshProUGUI ans3Text;

    [Header("Counter Text")]
    public TextMeshProUGUI counterText;

    [Header("Game Rules")]
    public int maxFails = 3;

    // אחוזי התקדמות
    int progressPercent = 0;

    // מספר נכון לכל תמונה (10 תמונות)
    public int[] correctNumbers =
    {
        74, 6, 16, 2, 29,
        7, 45, 5, 97, 8
    };

    int currentIndex = 0;
    int failCount = 0;
    int correctButtonIndex = 0;

    void Start()
    {
        ResetGame();
    }

    void ShowImage(int index)
    {
        for (int i = 0; i < images.Length; i++)
            images[i].SetActive(i == index);
    }

    void SetupAnswers()
    {
        if (currentIndex >= correctNumbers.Length)
            return;

        int correctNumber = correctNumbers[currentIndex];
        correctButtonIndex = Random.Range(0, 3);

        int wrong1 = correctNumber + Random.Range(1, 5);
        int wrong2 = correctNumber + Random.Range(6, 10);

        string[] texts = new string[3];
        texts[correctButtonIndex] = correctNumber.ToString();

        int w = 0;
        for (int i = 0; i < 3; i++)
        {
            if (i == correctButtonIndex) continue;
            texts[i] = (w == 0 ? wrong1 : wrong2).ToString();
            w++;
        }

        ans1Text.text = texts[0];
        ans2Text.text = texts[1];
        ans3Text.text = texts[2];
    }

    void UpdateCounter()
    {
        counterText.text = $"Fails: {failCount} / {maxFails}";
    }

    // 👇 הפונקציה שהכפתורים קוראים לה
    public void AnswerPressed(int buttonIndex)
    {
        if (buttonIndex == correctButtonIndex)
        {
            progressPercent += 10;

            if (progressBar != null)
                progressBar.value = progressPercent;

            if (progressText != null)
                progressText.text = progressPercent + "%";

            currentIndex++;

            if (currentIndex >= images.Length)
            {
                EndGame("YOU WIN!");
                return;
            }

            ShowImage(currentIndex);
            SetupAnswers();
        }
        else
        {
            failCount++;
            UpdateCounter();

            if (failCount >= maxFails)
            {
                EndGame("GAME OVER");
            }
        }
    }

    void EndGame(string message)
    {
        counterText.text = message;

        ans1.gameObject.SetActive(false);
        ans2.gameObject.SetActive(false);
        ans3.gameObject.SetActive(false);

        if (progressBar != null)
            progressBar.gameObject.SetActive(false);

        if (progressText != null)
            progressText.gameObject.SetActive(false);

        if (resetButton != null)
            resetButton.SetActive(true);

        if (message == "YOU WIN!")
        {
            SpawnRewards();
        }

    }

    public void ResetGame()
    {
        currentIndex = 0;
        failCount = 0;
        progressPercent = 0;

        ShowImage(0);
        SetupAnswers();
        UpdateCounter();

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0;
        }

        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = "0%";
        }

        ans1.gameObject.SetActive(true);
        ans2.gameObject.SetActive(true);
        ans3.gameObject.SetActive(true);

        if (resetButton != null)
            resetButton.SetActive(false);
    }

    void SpawnRewards()
    {
        for (int i = 0; i < 15; i++)
        {
            GameObject reward = Instantiate(rewardPrefab, spawnArea);

            RectTransform rt = reward.GetComponent<RectTransform>();

            rt.anchoredPosition = new Vector2(
                Random.Range(-300f, 300f),
                400f
            );

            StartCoroutine(FallDown(rt));
        }
    }

    IEnumerator FallDown(RectTransform rt)
    {
        float speed = Random.Range(150f, 300f);

        while (rt.anchoredPosition.y > -500f)
        {
            rt.anchoredPosition += Vector2.down * speed * Time.deltaTime;
            yield return null;
        }

        Destroy(rt.gameObject);
    }


}
