using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour {
    [ SerializeField ] private TextMeshProUGUI coinCounterText;
    private static UIManager instance = null ;
    public static UIManager Instance => instance;

    private PlayerStatistics statistics;
    
    [ SerializeField ] private Character character;
    [ SerializeField ] private Image healthBar;


    private void Awake()
    {
        instance = this;

        this.statistics = new PlayerStatistics() { coinCounter = 0 };
    }

    public void CollectCoin()
    {
        this.statistics.coinCounter++;

        string coinText = $"{this.statistics.coinCounter}";
        this.coinCounterText.text = coinText;
    }
    


    private class PlayerStatistics
    {
        public int coinCounter = 0;
    }
    
    private void Update () {
        float percent = this.character.GetCurrentHealth() / this.character.GetMaxHealth();
        this.healthBar.fillAmount = percent;
    }
    
}
