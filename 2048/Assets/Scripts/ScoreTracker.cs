using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    public static ScoreTracker Instance;

    public Text ScoreText;

    public Text HigtScore2048Text;
    public Text HigtScore4096Text;
    public Text HigtScore8192Text;
    public Text HigtScoreUnlimText;

    public Text GamesAllText;
    public Text Games2048Text;
    public Text Games4096Text;
    public Text Games8192Text;
    public Text GamesUnlimText;

    public Text WinsAllText;
    public Text Wins2048Text;
    public Text Wins4096Text;
    public Text Wins8192Text;
    public Text WinsUnlimText;

    private readonly string PrefAnimationEnabledString = "AnimationEnabledString";
    private readonly string PrefAnimationSpeedString = "AnimationSpeedString";

    private readonly string PrefHighScoreString2048 = "HighScore2048";
    private readonly string PrefHighScoreString4096 = "HighScore4096";
    private readonly string PrefHighScoreString8192 = "HighScore8192";
    private readonly string PrefHighScoreStringUnlim = "HighScoreUnlim";

    private readonly string PrefWinsCountString2048 = "WinsCountString2048";
    private readonly string PrefWinsCountString4096 = "WinsCountString4096";
    private readonly string PrefWinsCountString8192 = "WinsCountString8192";
    private readonly string PrefWinsCountStringUnlim = "WinsCountStringUnlim";

    private readonly string PrefGamesCountString2048 = "GamesCountString2048";
    private readonly string PrefGamesCountString4096 = "GamesCountString4096";
    private readonly string PrefGamesCountString8192 = "GamesCountString8192";
    private readonly string PrefGamesCountStringUnlim = "GamesCountStringUnlim";

    public bool AnimationEnabled
    {
        get { return PlayerPrefs.GetInt(PrefAnimationEnabledString) == 1; }
        set { PlayerPrefs.SetInt(PrefAnimationEnabledString, value ? 1 : 0); }
    }

    public float AnimationSpeed
    {
        get { return PlayerPrefs.GetFloat(PrefAnimationSpeedString); }
        set { PlayerPrefs.SetFloat(PrefAnimationSpeedString, value); }
    }

    private void Awake()
    {
        Instance = this;

        if (!PlayerPrefs.HasKey(PrefHighScoreString2048)) PlayerPrefs.SetInt(PrefHighScoreString2048, 0);
        if (!PlayerPrefs.HasKey(PrefHighScoreString4096)) PlayerPrefs.SetInt(PrefHighScoreString4096, 0);
        if (!PlayerPrefs.HasKey(PrefHighScoreString8192)) PlayerPrefs.SetInt(PrefHighScoreString8192, 0);
        if (!PlayerPrefs.HasKey(PrefHighScoreStringUnlim)) PlayerPrefs.SetInt(PrefHighScoreStringUnlim, 0);

        if (!PlayerPrefs.HasKey(PrefWinsCountString2048)) PlayerPrefs.SetInt(PrefWinsCountString2048, 0);
        if (!PlayerPrefs.HasKey(PrefWinsCountString4096)) PlayerPrefs.SetInt(PrefWinsCountString4096, 0);
        if (!PlayerPrefs.HasKey(PrefWinsCountString8192)) PlayerPrefs.SetInt(PrefWinsCountString8192, 0);
        if (!PlayerPrefs.HasKey(PrefWinsCountStringUnlim)) PlayerPrefs.SetInt(PrefWinsCountStringUnlim, 0);

        if (!PlayerPrefs.HasKey(PrefGamesCountString2048)) PlayerPrefs.SetInt(PrefGamesCountString2048, 0);
        if (!PlayerPrefs.HasKey(PrefGamesCountString4096)) PlayerPrefs.SetInt(PrefGamesCountString4096, 0);
        if (!PlayerPrefs.HasKey(PrefGamesCountString8192)) PlayerPrefs.SetInt(PrefGamesCountString8192, 0);
        if (!PlayerPrefs.HasKey(PrefGamesCountStringUnlim)) PlayerPrefs.SetInt(PrefGamesCountStringUnlim, 0);


        if (!PlayerPrefs.HasKey(PrefAnimationEnabledString)) PlayerPrefs.SetInt(PrefAnimationEnabledString, 1);
        if (!PlayerPrefs.HasKey(PrefAnimationSpeedString)) PlayerPrefs.SetInt(PrefAnimationSpeedString, 0);

        ScoreText.text = "0";
        SetTexts();
    }

    public void SetTexts()
    {
        HigtScore2048Text.text = HighScore2048.ToString();
        HigtScore4096Text.text = HighScore4096.ToString();
        HigtScore8192Text.text = HighScore8192.ToString();
        HigtScoreUnlimText.text = HighScoreUnlim.ToString();

        GamesAllText.text = (Games2048 + Games4096 + Games8192 + GamesUnlim).ToString();
        Games2048Text.text = Games2048.ToString();
        Games4096Text.text = Games4096.ToString();
        Games8192Text.text = Games8192.ToString();
        GamesUnlimText.text = GamesUnlim.ToString();

        WinsAllText.text = (Wins2048 + Wins4096 + Wins8192 + WinsUnlim).ToString();
        Wins2048Text.text = Wins2048.ToString();
        Wins4096Text.text = Wins4096.ToString();
        Wins8192Text.text = Wins8192.ToString();
        WinsUnlimText.text = WinsUnlim.ToString();
    }

    private int Wins2048
    {
        get { return PlayerPrefs.GetInt(PrefWinsCountString2048); }
        set
        {
            PlayerPrefs.SetInt(PrefWinsCountString2048, value);
            Wins2048Text.text = value.ToString();
        }
    }

    private int Wins4096
    {
        get { return PlayerPrefs.GetInt(PrefWinsCountString4096); }
        set
        {
            PlayerPrefs.SetInt(PrefWinsCountString4096, value);
            Wins4096Text.text = value.ToString();
        }
    }

    private int Wins8192
    {
        get { return PlayerPrefs.GetInt(PrefWinsCountString8192); }
        set
        {
            PlayerPrefs.SetInt(PrefWinsCountString8192, value);
            Wins8192Text.text = value.ToString();
        }
    }

    private int WinsUnlim
    {
        get { return PlayerPrefs.GetInt(PrefWinsCountStringUnlim); }
        set
        {
            PlayerPrefs.SetInt(PrefWinsCountStringUnlim, value);
            WinsUnlimText.text = value.ToString();
        }
    }

    public void AddWin(int scoreToWin)
    {
        switch (scoreToWin)
        {
            case 2048:
                Wins2048++;
                break;
            case 4096:
                Wins4096++;
                break;
            case 8192:
                Wins8192++;
                break;
            case 10005000:
                WinsUnlim++;
                break;
        }
        WinsAllText.text = (Wins2048 + Wins4096 + Wins8192 + WinsUnlim).ToString();
    }


    private int Games2048
    {
        get { return PlayerPrefs.GetInt(PrefGamesCountString2048); }
        set
        {
            PlayerPrefs.SetInt(PrefGamesCountString2048, value);
            Games2048Text.text = value.ToString();
        }
    }

    private int Games4096
    {
        get { return PlayerPrefs.GetInt(PrefGamesCountString4096); }
        set
        {
            PlayerPrefs.SetInt(PrefGamesCountString4096, value);
            Games4096Text.text = value.ToString();
        }
    }

    private int Games8192
    {
        get { return PlayerPrefs.GetInt(PrefGamesCountString8192); }
        set
        {
            PlayerPrefs.SetInt(PrefGamesCountString8192, value);
            Games8192Text.text = value.ToString();
        }
    }

    private int GamesUnlim
    {
        get { return PlayerPrefs.GetInt(PrefGamesCountStringUnlim); }
        set
        {
            PlayerPrefs.SetInt(PrefGamesCountStringUnlim, value);
            GamesUnlimText.text = value.ToString();
        }
    }
    public void AddGame(int scoreToWin)
    {
        switch (scoreToWin)
        {
            case 2048:
                Games2048++;
                break;
            case 4096:
                Games4096++;
                break;
            case 8192:
                Games8192++;
                break;
            case 10005000:
                GamesUnlim++;
                break;
        }
        GamesAllText.text = (Games2048 + Games4096 + Games8192 + GamesUnlim).ToString();
    }

    private int _score;

    public int Score
    {
        get { return _score; }
        private set
        {
            _score = value;
            ScoreText.text = _score.ToString();
        }
    }

    public void SetScore(int scoreToWin,int score)
    {
        switch (scoreToWin)
        {
            case 2048:
                if (score > HighScore2048) HighScore2048 = score;
                break;
            case 4096:
                if (score > HighScore4096) HighScore4096 = score;
                break;
            case 8192:
                if (score > HighScore8192) HighScore8192 = score;
                break;
            case 10005000:
                if (score > HighScoreUnlim) HighScoreUnlim = score;
                break;
        }
        Score = score;
    }

    private int HighScore2048
    {
        get { return PlayerPrefs.GetInt(PrefHighScoreString2048); }
        set
        {
            PlayerPrefs.SetInt(PrefHighScoreString2048, value);
            HigtScore2048Text.text = value.ToString();
        }
    }

    private int HighScore4096
    {
        get { return PlayerPrefs.GetInt(PrefHighScoreString4096); }
        set
        {
            PlayerPrefs.SetInt(PrefHighScoreString4096, value);
            HigtScore4096Text.text = value.ToString();
        }
    }

    private int HighScore8192
    {
        get { return PlayerPrefs.GetInt(PrefHighScoreString8192); }
        set
        {
            PlayerPrefs.SetInt(PrefHighScoreString8192, value);
            HigtScore8192Text.text = value.ToString();
        }
    }

    private int HighScoreUnlim
    {
        get { return PlayerPrefs.GetInt(PrefHighScoreStringUnlim); }
        set
        {
            PlayerPrefs.SetInt(PrefHighScoreStringUnlim, value);
            HigtScoreUnlimText.text = value.ToString();
        }
    }

    public void Reset()
    {
        AnimationEnabled = true;
        AnimationSpeed = 0;

        Wins2048 = 0;
        Wins4096 = 0;
        Wins8192 = 0;
        WinsUnlim = 0;

        Games2048 = 0;
        Games4096 = 0;
        Games8192 = 0;
        GamesUnlim = 0;

        HighScore2048 = 0;
        HighScore4096 = 0;
        HighScore8192 = 0;
        HighScoreUnlim = 0;

        SetTexts();

    }

}