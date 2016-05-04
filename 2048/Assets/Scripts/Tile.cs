using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

	public bool mergedThisTurn = false;

	public int indRow;
	public int indCol;

	public int Number 
	{
		get
		{
			return number;
		}
		set
		{
			number = value;
			if (number == 0)
				SetEmpty ();
			else {
				ApplyStyle (number);
				SetVisible ();
			}
		}
	}
	private int number;

	private Text TileText;
	private Image TileImage;

	void Awake()
	{
		TileText = GetComponentInChildren<Text> ();
		TileImage = transform.Find ("NumberedCell").GetComponent<Image>();
	}

	void ApplyStyleFromHolder(int index)
	{
		TileText.text = TileStyleHolder.Instance.TileStyles [index].Number.ToString();
		TileText.color = TileStyleHolder.Instance.TileStyles [index].TextColor;
		TileImage.color = TileStyleHolder.Instance.TileStyles [index].TileColor;
	}

	int CalcIndex(int num)
	{
		int count=0;
		int calcNum = num;
		for(;calcNum>=2;)
		{
			calcNum=calcNum/2;
			count++;
		}
		if ((count - 1) < 0 || (count - 1) > 11) 
		{
			Debug.LogError ("Error Num=" + num);
			return 0;
		}
		return count-1;
	}

	void ApplyStyle(int num)
	{
		ApplyStyleFromHolder (CalcIndex(num));
	}

	private void SetVisible()
	{
		TileImage.enabled = true;
		TileText.enabled = true;
	}

	private void SetEmpty()
	{
		TileImage.enabled = false;
		TileText.enabled = false;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
