using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    private string PrefHighScoreString = "HighScore";
    private int _score;

    public static ScoreTracker Instance;
    public Text ScoreText;
    public Text HighScoreText;

    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            ScoreText.text = _score.ToString();

            if (PlayerPrefs.GetInt(PrefHighScoreString) < _score)
            {
                PlayerPrefs.SetInt(PrefHighScoreString, _score);
                HighScoreText.text = _score.ToString();
            }
        }
    }

    void Awake()
    {
        Instance = this;

        if (!PlayerPrefs.HasKey(PrefHighScoreString)) PlayerPrefs.SetInt(PrefHighScoreString, 0);

        ScoreText.text = "0";
        HighScoreText.text = PlayerPrefs.GetInt(PrefHighScoreString).ToString();
    }
}
