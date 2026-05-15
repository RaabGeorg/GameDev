using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour 
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinCounterText;
    [SerializeField] private Image healthBar;
    
    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup hudCanvasGroup;
    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private CanvasGroup victoryCanvasGroup;

    [Header("Settings")]
    [SerializeField] private Character character;
    [SerializeField] private float fadingTime = 2.0f;

    private static UIManager instance = null;
    public static UIManager Instance => instance;

    private PlayerStatistics statistics;
    private bool isGameOver = false;

    // Public property for the Character script to check state
    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        instance = this;
        this.statistics = new PlayerStatistics() { coinCounter = 0 };
    }

    private void Update() 
    {
        if (this.character == null || isGameOver) return;

        float percent = this.character.GetCurrentHealth() / this.character.GetMaxHealth();
        this.healthBar.fillAmount = percent;

        if (percent <= 0.0f) 
        {
            TriggerGameOver();
        }
    }

    public void CollectCoin()
    {
        this.statistics.coinCounter++;
        this.coinCounterText.text = this.statistics.coinCounter.ToString();
    }

    public void TriggerVictory()
    {
        if (!isGameOver) StartCoroutine(FadeInCanvas(victoryCanvasGroup));
    }

    public void TriggerGameOver()
    {
        if (!isGameOver) StartCoroutine(FadeInCanvas(gameOverCanvasGroup));
    }

    public void OnRespawnButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }

    private IEnumerator FadeInCanvas(CanvasGroup targetGroup) 
    {
        isGameOver = true;
        float timer = 0.0f;

        while (timer < this.fadingTime) 
        {
            float percent = timer / this.fadingTime;
            this.hudCanvasGroup.alpha = 1.0f - percent;
            targetGroup.alpha = percent;
            yield return null;
            timer += Time.deltaTime;
        }

        this.hudCanvasGroup.alpha = 0.0f;
        targetGroup.alpha = 1.0f;

        targetGroup.interactable = true;
        targetGroup.blocksRaycasts = true;
    }

    private class PlayerStatistics
    {
        public int coinCounter = 0;
    }
}