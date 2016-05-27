using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum GameStates
{
    Playing,
    GameOver,
    WaitForMoveToEnd
}

public class GameManager : MonoBehaviour
{
    private Tile[,] _allTiles = new Tile[4, 4];
    private List<Tile[]> _columns = new List<Tile[]>();
    private List<Tile> _emptyTiles = new List<Tile>();
    private bool[] _lineMoveComplite;
    private bool _moveMade;
    private List<Tile[]> _rows = new List<Tile[]>();

    private List<StoredMove> moves;

    private int _scoreToWin;

    public GameObject GameOverPanel;
    public Text GameOverScoreText;
    public GameObject GameOverText;
    public GameObject GameStartPanel;
    public GameObject HelpPanel;
    public GameObject SettingsPanel;
    public GameObject MainPanel;
    public GameStates State;
    public Text TitleText;
    public GameObject YouWonText;
    public Toggle SettingsToggleAnimation;
    public Slider SettingSliderAnumationSpeed;
    public Text TextBack;
    public GameObject StatisticsPanel;

    private int BackCount = 5;


    private Lang LMan;
    public Text GameStartPanelStatisticsText;
    public Text GameStartPanelUnlimText;
    public Text GameStartPanelSettingsText;

    public Text MainPanelScoreText;
    public Text MainPanelBackText;

    public Text GameOverPanelGameOverText;
    public Text GameOverPanelYouWonText;
    public Text GameOverPanelToMainMenuText;
    public Text GameOverPanelContinueText;

    public Text HelpPanelHelpText;
    public Text HelpPanelBugText;
    public Text HelpPanelCloseText;

    public Text SettingsPanelCloseText;
    public Text SettingsPanelEnableAnimationText;
    public Text SettingsPanelMoveSpeedText;
    public Text SettingsPanelResetText;

    public Text StatisticsPanelAllStatsText;
    public Text StatisticsPanelAllStatsGamesText;
    public Text StatisticsPanelAllStatsWinsText;

    public Text StatisticsPanel2048StatsHighScoreText;
    public Text StatisticsPanel2048StatsGamesText;
    public Text StatisticsPanel2048StatsWinsText;

    public Text StatisticsPanel4096StatsHighScoreText;
    public Text StatisticsPanel4096StatsGamesText;
    public Text StatisticsPanel4096StatsWinsText;

    public Text StatisticsPanel8192StatsHighScoreText;
    public Text StatisticsPanel8192StatsGamesText;
    public Text StatisticsPanel8192StatsWinsText;

    public Text StatisticsPanelUnlimStatsText;
    public Text StatisticsPanelUnlimStatsHighScoreText;
    public Text StatisticsPanelUnlimStatsGamesText;
    public Text StatisticsPanelUnlimStatsWinsText;

    public Text StatisticsPanelCloseText;

    private void Awake()
    {
        LMan = new Lang(Path.Combine(Application.dataPath, "lang.xml"), Application.systemLanguage.ToString(), false);
        //LMan = new Lang(Path.Combine(Application.dataPath, "lang.xml"), "English", false);

        GameStartPanelStatisticsText.text = LMan.getString("statistics");
        GameStartPanelUnlimText.text = LMan.getString("unlim");
        GameStartPanelSettingsText.text = LMan.getString("settings");

        MainPanelScoreText.text = LMan.getString("score");
        MainPanelBackText.text = LMan.getString("back");

        GameOverPanelGameOverText.text = LMan.getString("gameOver");
        GameOverPanelYouWonText.text = LMan.getString("youWon");
        GameOverPanelToMainMenuText.text = LMan.getString("toMainMenu");
        GameOverPanelContinueText.text = LMan.getString("continue");

        HelpPanelHelpText.text = LMan.getString("helpText");
        HelpPanelBugText.text = LMan.getString("bugText");
        HelpPanelCloseText.text = LMan.getString("close");

        SettingsPanelCloseText.text = LMan.getString("close");
        SettingsPanelEnableAnimationText.text = LMan.getString("enableAnimation");
        SettingsPanelMoveSpeedText.text = LMan.getString("moveSepeed");
        SettingsPanelResetText.text = LMan.getString("reset");

        StatisticsPanelAllStatsText.text = LMan.getString("allStats");
        StatisticsPanelAllStatsGamesText.text = LMan.getString("games");
        StatisticsPanelAllStatsWinsText.text = LMan.getString("wins");

        StatisticsPanel2048StatsHighScoreText.text = LMan.getString("highScore");
        StatisticsPanel2048StatsGamesText.text = LMan.getString("games");
        StatisticsPanel2048StatsWinsText.text = LMan.getString("wins");

        StatisticsPanel4096StatsHighScoreText.text = LMan.getString("highScore");
        StatisticsPanel4096StatsGamesText.text = LMan.getString("games");
        StatisticsPanel4096StatsWinsText.text = LMan.getString("wins");

        StatisticsPanel8192StatsHighScoreText.text = LMan.getString("highScore");
        StatisticsPanel8192StatsGamesText.text = LMan.getString("games");
        StatisticsPanel8192StatsWinsText.text = LMan.getString("wins");

        StatisticsPanelUnlimStatsText.text = LMan.getString("unlim");
        StatisticsPanelUnlimStatsHighScoreText.text = LMan.getString("highScore");
        StatisticsPanelUnlimStatsGamesText.text = LMan.getString("games");
        StatisticsPanelUnlimStatsWinsText.text = LMan.getString("wins");

        StatisticsPanelCloseText.text = LMan.getString("close");
    }

    public bool IsMainMenu
    {
        get { return GameStartPanel.activeSelf; }
    }
    private int ScoreToWin
    {
        get { return _scoreToWin; }
        set
        {
            if (value == 16384)
            {
                _scoreToWin = 10005000;
                TitleText.text = "Unlim";
            }
            else
            {
                _scoreToWin = value;
                TitleText.text = _scoreToWin.ToString();
            }
        }
    }

    public void WorkGameOverPanel(bool open, bool isWin)
    {
        GameStartPanel.SetActive(!open);
        MainPanel.SetActive(open);
        GameOverPanel.SetActive(open);
        if (isWin) ScoreTracker.Instance.AddWin(ScoreToWin);
    }

    public void WorkHelpPanel(bool show)
    {
        GameStartPanel.SetActive(!show);
        HelpPanel.SetActive(show);
    }

    public void WorkSettingsPanel(bool show)
    {
        SettingsToggleAnimation.isOn = ScoreTracker.Instance.AnimationEnabled;
        SettingSliderAnumationSpeed.value = ScoreTracker.Instance.AnimationSpeed;

        if(show) ScoreTracker.Instance.SetTexts();

        GameStartPanel.SetActive(!show);
        SettingsPanel.SetActive(show);
    }

    public void WorkStatisticsPanel(bool show)
    {
        GameStartPanel.SetActive(!show);
        StatisticsPanel.SetActive(show);
    }

    // Use this for initialization
    private void StartGame()
    {
        ScoreTracker.Instance.AddGame(ScoreToWin);

        ScoreTracker.Instance.SetScore(ScoreToWin,0);

        BackCount = 5;
        TextBack.text = BackCount.ToString();

        _lineMoveComplite = new bool[] { false, false, false, false };
        moves = new List<StoredMove>();

        _emptyTiles.Clear();
        var allTilesOneDim = FindObjectsOfType<Tile>();
        foreach (var t in allTilesOneDim)
        {
            t.GetComponent<Animator>().enabled = ScoreTracker.Instance.AnimationEnabled;

            if (ScoreToWin > 2048 && t.indRow==0 && t.indCol ==0)
            {
                t.Number = ScoreToWin == 10005000 ? 8192 : ScoreToWin / 2;
                _allTiles[t.indRow, t.indCol] = t;
            }
            else
            {
                t.Number = 0;
                _allTiles[t.indRow, t.indCol] = t;
                _emptyTiles.Add(t);
            }
        }
        _columns.Clear();
        _columns.Add(new[] {_allTiles[0, 0], _allTiles[1, 0], _allTiles[2, 0], _allTiles[3, 0]});
        _columns.Add(new[] {_allTiles[0, 1], _allTiles[1, 1], _allTiles[2, 1], _allTiles[3, 1]});
        _columns.Add(new[] {_allTiles[0, 2], _allTiles[1, 2], _allTiles[2, 2], _allTiles[3, 2]});
        _columns.Add(new[] {_allTiles[0, 3], _allTiles[1, 3], _allTiles[2, 3], _allTiles[3, 3]});

        _rows.Clear();
        _rows.Add(new[] {_allTiles[0, 0], _allTiles[0, 1], _allTiles[0, 2], _allTiles[0, 3]});
        _rows.Add(new[] {_allTiles[1, 0], _allTiles[1, 1], _allTiles[1, 2], _allTiles[1, 3]});
        _rows.Add(new[] {_allTiles[2, 0], _allTiles[2, 1], _allTiles[2, 2], _allTiles[2, 3]});
        _rows.Add(new[] {_allTiles[3, 0], _allTiles[3, 1], _allTiles[3, 2], _allTiles[3, 3]});

        Generate();
        Generate();
    }

    private void YouWon()
    {
        GameOverText.SetActive(false);
        YouWonText.SetActive(true);

        GameOver(true);
    }

    private void GameOver(bool isWin = false)
    {
        State = GameStates.GameOver;
        GameOverScoreText.text = ScoreTracker.Instance.Score.ToString();
        WorkGameOverPanel(true, isWin);
    }

    private bool CanMove()
    {
        if (_emptyTiles.Count > 0) return true;
        //columns
        for (var i = 0; i < _columns.Count; i++)
            for (var j = 0; j < _rows.Count - 1; j++)
                if (_allTiles[j, i].Number == _allTiles[j + 1, i].Number)
                    return true;

        //rows
        for (var i = 0; i < _rows.Count; i++)
            for (var j = 0; j < _columns.Count - 1; j++)
                if (_allTiles[i, j].Number == _allTiles[i, j + 1].Number)
                    return true;

        return false;
    }

    public void NewGameButtonHandler(int scoreToWin)
    {
        State = GameStates.Playing;
        ScoreToWin = scoreToWin;
        GameStartPanel.SetActive(false);
        MainPanel.SetActive(true);
        StartGame();
        //Application.LoadLevel(Application.loadedLevel);
    }

    public void ContinueGameButtonHandler()
    {
        State = GameStates.Playing;
        ScoreToWin = ScoreToWin*2;
        GameOverPanel.SetActive(false);
    }

    public void ToMainMenuButtonHandler()
    {
        WorkGameOverPanel(false, false);
        WorkSettingsPanel(false);
        WorkHelpPanel(false);
        WorkStatisticsPanel(false);
    }

    private bool MakeOneMoveDownIndex(IList<Tile> lineOfTiles)
    {
        for (var i = 0; i < lineOfTiles.Count - 1; i++)
        {
            //MOVE BLOCK
            if (lineOfTiles[i].Number == 0 && lineOfTiles[i + 1].Number != 0)
            {
                lineOfTiles[i].Number = lineOfTiles[i + 1].Number;
                lineOfTiles[i + 1].Number = 0;
                return true;
            }
            //MERGE BLOCK
            if (lineOfTiles[i].Number != 0
                && lineOfTiles[i].Number == lineOfTiles[i + 1].Number
                && lineOfTiles[i].MergedThisTurn == false && lineOfTiles[i + 1].MergedThisTurn == false)
            {
                lineOfTiles[i].Number *= 2;
                lineOfTiles[i + 1].Number = 0;
                lineOfTiles[i].MergedThisTurn = true;
                lineOfTiles[i].PlayMergedAnimation();
                ScoreTracker.Instance.SetScore(ScoreToWin, ScoreTracker.Instance.Score + lineOfTiles[i].Number);
                if (lineOfTiles[i].Number == ScoreToWin) YouWon();
                return true;
            }
        }
        return false;
    }

    private bool MakeOneMoveUpIndex(IList<Tile> lineOfTiles)
    {
        for (var i = lineOfTiles.Count - 1; i > 0; i--)
        {
            //MOVE BLOCK
            if (lineOfTiles[i].Number == 0 && lineOfTiles[i - 1].Number != 0)
            {
                lineOfTiles[i].Number = lineOfTiles[i - 1].Number;
                lineOfTiles[i - 1].Number = 0;
                return true;
            }
            //MERGE BLOCK
            if (lineOfTiles[i].Number != 0
                && lineOfTiles[i].Number == lineOfTiles[i - 1].Number
                && lineOfTiles[i].MergedThisTurn == false && lineOfTiles[i - 1].MergedThisTurn == false)
            {
                lineOfTiles[i].Number *= 2;
                lineOfTiles[i - 1].Number = 0;
                lineOfTiles[i].MergedThisTurn = true;
                lineOfTiles[i].PlayMergedAnimation();
                ScoreTracker.Instance.SetScore(ScoreToWin,ScoreTracker.Instance.Score + lineOfTiles[i].Number);
                if (lineOfTiles[i].Number == ScoreToWin) YouWon();
                return true;
            }
        }
        return false;
    }

    private void Generate()
    {
        if (_emptyTiles.Count > 0)
        {
            var indexForNewNumber = Random.Range(0, _emptyTiles.Count);

            _emptyTiles[indexForNewNumber].Number = Random.Range(0, 10) == 0 ? 4 : 2; //10%
            _emptyTiles[indexForNewNumber].PlayAppearAnimation();
            _emptyTiles.RemoveAt(indexForNewNumber);
        }
    }

    private void ResetMergedFlags()
    {
        foreach (var t in _allTiles)
            t.MergedThisTurn = false;
    }

    private void UpdateEmptyTiles()
    {
        _emptyTiles.Clear();
        foreach (var t in _allTiles)
        {
            if (t.Number == 0)
                _emptyTiles.Add(t);
        }
    }

    private IEnumerator MoveOneLineUpIndexCoroutine(IList<Tile> line, int index)
    {
        _lineMoveComplite[index] = false;

        while (MakeOneMoveUpIndex(line))
        {
            _moveMade = true;
            yield return new WaitForSeconds(ScoreTracker.Instance.AnimationSpeed);
        }
        _lineMoveComplite[index] = true;
    }

    private IEnumerator MoveOneLineDownIndexCoroutine(IList<Tile> line, int index)
    {
        _lineMoveComplite[index] = false;

        while (MakeOneMoveDownIndex(line))
        {
            _moveMade = true;
            yield return new WaitForSeconds(ScoreTracker.Instance.AnimationSpeed);
        }
        _lineMoveComplite[index] = true;
    }

    private IEnumerator MoveCoroutine(MoveDirection md)
    {
        State = GameStates.WaitForMoveToEnd;

        _lineMoveComplite = new bool[] { false, false, false, false };

        switch (md)
        {
            case MoveDirection.Down:
                for (var i = 0; i < _columns.Count; i++)
                    StartCoroutine(MoveOneLineUpIndexCoroutine(_columns[i], i));
                break;
            case MoveDirection.Left:
                for (var i = 0; i < _rows.Count; i++)
                    StartCoroutine(MoveOneLineDownIndexCoroutine(_rows[i], i));
                break;
            case MoveDirection.Right:
                for (var i = 0; i < _rows.Count; i++)
                    StartCoroutine(MoveOneLineUpIndexCoroutine(_rows[i], i));
                break;
            case MoveDirection.Up:
                for (var i = 0; i < _columns.Count; i++)
                    StartCoroutine(MoveOneLineDownIndexCoroutine(_columns[i], i));
                break;
        }

        while (!(_lineMoveComplite[0] && _lineMoveComplite[1] && _lineMoveComplite[2] && _lineMoveComplite[3]))
            yield return null;

        if (_moveMade)
        {
            UpdateEmptyTiles();
            Generate();

            if (!CanMove()) GameOver();
            else if(State != GameStates.GameOver) State = GameStates.Playing;
        }
        else State = GameStates.Playing;

        StopAllCoroutines();
    }

    private class StoredTile
    {
        public int x;
        public int y;
        public int number;

        public StoredTile(Tile t)
        {
            x = t.indRow; 
            y = t.indCol;
            number = t.Number;
        }

    }
    private class StoredMove
    {
        public List<StoredTile> Tiles;
        public int Score;

        public StoredMove(Tile[,] allTiles, int score)
        {
            //Debug.Log("new move score=" + score);
            Tiles = new List<StoredTile>();
            foreach (var t in allTiles) Tiles.Add(new StoredTile(t));
            Score = score;
        }
    }

    public void Move(MoveDirection md)
    {
        _moveMade = false;

        if(moves.Count>=5)moves.RemoveAt(0);
        moves.Add(new StoredMove(_allTiles, ScoreTracker.Instance.Score));

        ResetMergedFlags();

        if (ScoreTracker.Instance.AnimationSpeed > 0) StartCoroutine(MoveCoroutine(md));
        else
        {
            for (var i = 0; i < _rows.Count; i++)
            {
                switch (md)
                {
                    case MoveDirection.Down:
                        while (MakeOneMoveUpIndex(_columns[i])) _moveMade = true;
                        break;
                    case MoveDirection.Left:
                        while (MakeOneMoveDownIndex(_rows[i])) _moveMade = true;
                        break;
                    case MoveDirection.Right:
                        while (MakeOneMoveUpIndex(_rows[i])) _moveMade = true;
                        break;
                    case MoveDirection.Up:
                        while (MakeOneMoveDownIndex(_columns[i])) _moveMade = true;
                        break;
                }
            }

            if (_moveMade)
            {
                UpdateEmptyTiles();
                Generate();

                if (!CanMove()) GameOver();
            }
        }
    }

    public void SettingChangeAnimation()
    {
        ScoreTracker.Instance.AnimationEnabled = SettingsToggleAnimation.isOn;
    }
    public void SettingChangeAnimationSpeed()
    {
        ScoreTracker.Instance.AnimationSpeed = SettingSliderAnumationSpeed.value;
    }
    public void SettingReset()
    {
        ScoreTracker.Instance.Reset();

        SettingsToggleAnimation.isOn = ScoreTracker.Instance.AnimationEnabled;
        SettingSliderAnumationSpeed.value = ScoreTracker.Instance.AnimationSpeed;

    }

    public void Back()
    {
        if(BackCount == 0)return;
       
        if (moves.Count>0)
        {
            //Debug.Log("move back score=" + moves[moves.Count - 1].Score);

            foreach (var n in moves[moves.Count - 1].Tiles) _allTiles[n.x, n.y].Number = n.number;

            ScoreTracker.Instance.SetScore(ScoreToWin, moves[moves.Count - 1].Score);

            moves.RemoveAt(moves.Count-1);
            UpdateEmptyTiles();

            BackCount--;
            TextBack.text = BackCount.ToString();
        }
    }
}