using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu("Layout/Auto Grid Layout Group", 152)]
public class AutoGridLayout : GridLayoutGroup
{
    [SerializeField] private bool _mIsColumn;
    [SerializeField] private int m_Column = 1, m_Row = 1;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        float iColumn;
        float iRow;
        if (_mIsColumn)
        {
            iColumn = m_Column;
            if (iColumn <= 0)
            {
                iColumn = 1;
            }
            iRow = Mathf.CeilToInt(this.transform.childCount/iColumn);
        }
        else
        {
            iRow = m_Row;
            if (iRow <= 0)
            {
                iRow = 1;
            }
            iColumn = Mathf.CeilToInt(transform.childCount/iRow);
        }
        var fHeight = rectTransform.rect.height - (iRow - 1)*spacing.y - (padding.top + padding.bottom);
        var fWidth = rectTransform.rect.width - (iColumn - 1)*spacing.x - (padding.right + padding.left);
        var vSize = new Vector2(fWidth/iColumn, (fHeight)/iRow);
        cellSize = vSize;
    }
}
