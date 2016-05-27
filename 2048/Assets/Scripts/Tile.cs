using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    private Animator _anim;

    private int _number;
    private Image _tileImage;

    private Text _tileText;
    public int indCol;

    public int indRow;

    public bool MergedThisTurn = false;

    public int Number
    {
        get { return _number; }
        set
        {
            _number = value;
            if (_number == 0)
                SetEmpty();
            else
            {
                ApplyStyle(_number);
                SetVisible();
            }
        }
    }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _tileText = GetComponentInChildren<Text>();
        _tileImage = transform.Find("NumberedCell").GetComponent<Image>();
    }

    public void PlayMergedAnimation()
    {
        _anim.SetTrigger("Merge");
    }

    public void PlayAppearAnimation()
    {
        _anim.SetTrigger("Appear");
    }

    private void ApplyStyleFromHolder(int index)
    {
        _tileText.text = TileStyleHolder.Instance.TileStyles[index].Number.ToString();
        _tileText.color = TileStyleHolder.Instance.TileStyles[index].TextColor;
        _tileImage.color = TileStyleHolder.Instance.TileStyles[index].TileColor;
    }

    private int CalcIndex(int num)
    {
        var count = 0;
        var calcNum = num;
        for (; calcNum >= 2;)
        {
            calcNum = calcNum/2;
            count++;
        }
        if (count - 1 < 0 || count - 1 > 15)
        {
            //Debug.LogError("Error Num=" + num);
            return 0;
        }
        return count - 1;
    }

    private void ApplyStyle(int num)
    {
        ApplyStyleFromHolder(CalcIndex(num));
    }

    private void SetVisible()
    {
        _tileImage.enabled = true;
        _tileText.enabled = true;
    }

    private void SetEmpty()
    {
        _tileImage.enabled = false;
        _tileText.enabled = false;
    }
}