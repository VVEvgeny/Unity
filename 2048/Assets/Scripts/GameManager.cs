using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    public GameStates State;
    [Range(0, 2f)]
    public float delay;
    private bool _moveMade;
    private bool[] _lineMoveComplite = new bool[4] {true, true, true, true};
    public GameObject YouWonText;
    public GameObject GameOverText;
    public Text GameOverScoreText;
    public GameObject GameOverPanel;
    public GameObject GameStartPanel;
    public GameObject MainPanel;
    public GameObject HelpPanel;
    public Text TitleText;

    public void WorkHelpPanel(bool open)
    {
        GameStartPanel.SetActive(!open);
        HelpPanel.SetActive(open);
    }
    public void WorkGameOverPanel(bool open)
    {
        GameStartPanel.SetActive(!open);
        MainPanel.SetActive(open);
        GameOverPanel.SetActive(open);
    }


    private Tile[,] _allTiles = new Tile[4, 4];
    private List<Tile[]> _columns = new List<Tile[]>();
    private List<Tile[]> _rows = new List<Tile[]>();
    private List<Tile> _emptyTiles = new List<Tile>();

    private int _scoreToWin;
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

    // Use this for initialization
    void StartGame()
    {
        var allTilesOneDim = GameObject.FindObjectsOfType<Tile>();

        foreach (var t in allTilesOneDim)
        {
            t.Number = 0;
            _allTiles[t.indRow, t.indCol] = t;
            _emptyTiles.Add(t);
        }
        _columns.Add(new[] {_allTiles[0, 0], _allTiles[1, 0], _allTiles[2, 0], _allTiles[3, 0]});
        _columns.Add(new[] {_allTiles[0, 1], _allTiles[1, 1], _allTiles[2, 1], _allTiles[3, 1]});
        _columns.Add(new[] {_allTiles[0, 2], _allTiles[1, 2], _allTiles[2, 2], _allTiles[3, 2]});
        _columns.Add(new[] {_allTiles[0, 3], _allTiles[1, 3], _allTiles[2, 3], _allTiles[3, 3]});

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

        GameOver();
    }

    private void GameOver()
    {
        State = GameStates.GameOver;
        GameOverScoreText.text = ScoreTracker.Instance.Score.ToString();
        WorkGameOverPanel(true);
    }

    bool CanMove()
    {
        if (_emptyTiles.Count > 0) return true;
        //columns
        for (int i = 0; i < _columns.Count; i++)
            for (int j = 0; j < _rows.Count - 1; j++)
                if (_allTiles[j, i].Number == _allTiles[j + 1, i].Number)
                    return true;

        //rows
        for (int i = 0; i < _rows.Count; i++)
            for (int j = 0; j < _columns.Count - 1; j++)
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
        ScoreToWin = ScoreToWin * 2;
        GameOverPanel.SetActive(false);
    }

    bool MakeOneMoveDownIndex(IList<Tile> lineOfTiles)
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
                ScoreTracker.Instance.Score += lineOfTiles[i].Number;
                if (lineOfTiles[i].Number == ScoreToWin) YouWon();
                return true;
            }
        }
        return false;
    }

    bool MakeOneMoveUpIndex(IList<Tile> lineOfTiles)
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
                ScoreTracker.Instance.Score += lineOfTiles[i].Number;
                if (lineOfTiles[i].Number == ScoreToWin) YouWon();
                return true;
            }
        }
        return false;
    }

    void Generate()
    {
        if (_emptyTiles.Count > 0)
        {
            var indexForNewNumber = Random.Range(0, _emptyTiles.Count);

            var randomNum = Random.Range(0, 10); //10%

            //_emptyTiles[indexForNewNumber].Number = randomNum == 0 ? 4 : 2;
            _emptyTiles[indexForNewNumber].Number = 512;

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

    IEnumerator MoveOneLineUpIndexCoroutine(IList<Tile> line, int index)
    {
        _lineMoveComplite[index] = false;
        while (MakeOneMoveUpIndex(line))
        {
            _moveMade = true;
            yield return new WaitForSeconds(delay);
        }
        _lineMoveComplite[index] = true;
    }

    IEnumerator MoveOneLineDownIndexCoroutine(IList<Tile> line, int index)
    {
        _lineMoveComplite[index] = false;
        while (MakeOneMoveDownIndex(line))
        {
            _moveMade = true;
            yield return new WaitForSeconds(delay);
        }
        _lineMoveComplite[index] = true;
    }

    IEnumerator MoveCoroutine(MoveDirection md)
    {
        State = GameStates.WaitForMoveToEnd;

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
        }
        State = GameStates.Playing;

        StopAllCoroutines();
    }

    public void Move(MoveDirection md)
    {
        //Debug.Log(md.ToString() + " move.");
        _moveMade = false;

        ResetMergedFlags();

        if (delay > 0) StartCoroutine(MoveCoroutine(md));
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
}
