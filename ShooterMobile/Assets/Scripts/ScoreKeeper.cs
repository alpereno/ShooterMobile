using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    // we have to notifying when the player kills an enemy, we can use onDeath event
    // this event should be static

    public static int score { get; private set; }
    public static int highScore { get; private set; }
    float lastEnemyKilledTime;
    int streakCount;
    float streakTime = .5f;

    private void Start()
    {
        Enemy.onDeathStatic += onEnemyDeath;
        FindObjectOfType<Player>().onDeath += onPlayerDeath;
        highScore = PlayerPrefs.GetInt("HighScore");
    }

    void onEnemyDeath() {
        if (Time.time < lastEnemyKilledTime + streakTime){
            streakCount++;
        }
        else {
            streakCount = 0;
        }
        lastEnemyKilledTime = Time.time;

        score += 4 + (int)Mathf.Pow(3, streakCount);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }

    // when the player dies, onEnemyDeath subscribeing twice because it was static event
    void onPlayerDeath() {
        Enemy.onDeathStatic -= onEnemyDeath;
        score = 0;
    }

    public static void resetHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        highScore = 0;
    }
}