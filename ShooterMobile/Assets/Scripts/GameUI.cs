using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Game Over UI")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject gameOverUI;

    [Header("Wave UI")]
    [SerializeField] private RectTransform newWaveBanner;
    [SerializeField] private TMP_Text newWaveNumberText;
    [SerializeField] private TMP_Text newWaveEnemyCountText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private TMP_Text streakText;

    [Header("Health UI")]
    [SerializeField] Slider healthSlider;
    [SerializeField] Image fillImage;
    [SerializeField] Color fullHealthColor;
    [SerializeField] Color zeroHealthColor;

    [Header("Gun UI")]
    [SerializeField] private Image[] bulletsImage;
    [SerializeField] private Color bulletColor;

    Spawner spawner;
    Player player;
    GunController gunController;
    float screenHeight;
    float bannerTopPos;
    float bannerBotPos;
    IEnumerator currentStreakcoroutine;

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.onNewWave += onNewWave;
        player = FindObjectOfType<Player>();
        gunController = player.GetComponent<GunController>();
        FindObjectOfType<ScoreKeeper>().onStreak += onStreak;
        streakText.alpha = 0;
    }

    private void Start()
    {
        player.onDeath += onGameOver;        
        screenHeight = Screen.height;
        //print(screenHeight);
        //bannerBotPos = screenHeight - (screenHeight * 5 / 3);
        bannerBotPos = -(2 * screenHeight / 5);
        //print("bot pos" + bannerBotPos);
        bannerTopPos = screenHeight / 3;
    }

    private void Update()
    {
        scoreText.text = ScoreKeeper.score.ToString("D6");
        if (player != null)
        {
            setHealthUI(player.getHealth());
        }
        if (gunController != null)
        {
            setBulletsUI(gunController.getRemainingBullets);
        }
    }

    void onNewWave(int waveNumber) {
        string[] numbers = { "One", "Two", "Three", "Four", "Five", "Six", "Seven"};
        newWaveNumberText.text = "--Wave " + numbers[waveNumber - 1] + "--";
        string enemyCount = spawner.waves[waveNumber - 1].infinite ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "";
        newWaveEnemyCountText.text = "Enemies: " + enemyCount;

        StopCoroutine("animateBanner");
        StartCoroutine(animateBanner());
    }

    void setBulletsUI(int remainingBullets) {
        int bulletsImageLength = bulletsImage.Length;
        for (int i = 0; i < bulletsImageLength; i++)
        {
            if (i < remainingBullets)
            {
                bulletsImage[i].color = bulletColor;
            }
            else bulletsImage[i].color = Color.black;
        }
    }

    void onGameOver() {
        Cursor.visible = true;
        gameOverScoreText.text = scoreText.text;
        highScoreText.text = ScoreKeeper.highScore.ToString("D6");
        StartCoroutine(fade(Color.clear, new Color(0, 0, 0, .95f), 1));
        scoreText.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    void onStreak() {

        if (currentStreakcoroutine != null)
        {
            StopCoroutine(currentStreakcoroutine);
        }

        currentStreakcoroutine = animateStreakPoint();
        StartCoroutine(currentStreakcoroutine);        
    }

    void setHealthUI(float health) {
        healthSlider.value = health;
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, health / player.startingHealth);
    }

    IEnumerator fade(Color from, Color to, float time) {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadeImage.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    IEnumerator animateBanner() {
        //Banner will go down firstly then go back to up
        float delayTime = 2f;
        float speed = 3f;
        float animatePercent = 0;
        float endDelayTime = Time.time + 1 / speed + delayTime;
        int direction = -1;
        
        while (animatePercent >= 0){
            animatePercent -= Time.deltaTime * speed * direction;

            if (animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > endDelayTime)
                {
                    direction = 1;
                }
            }
            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(bannerTopPos, bannerBotPos, animatePercent);
            yield return null;
        }
    }

    IEnumerator animateStreakPoint()
    {
        streakText.alpha = 1;
        streakText.text = "Streak! X" + Mathf.Pow(3, ScoreKeeper.streakCount);
        yield return new WaitForSeconds(.5f);
        streakText.alpha = 0;
    }

    public void startNewGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void resetHighScore()
    {
        ScoreKeeper.resetHighScore();
        highScoreText.text = ScoreKeeper.highScore.ToString("D6");
    }

    public void returnToMainMenu() {
        SceneManager.LoadScene("MenuScene");
    }
}
