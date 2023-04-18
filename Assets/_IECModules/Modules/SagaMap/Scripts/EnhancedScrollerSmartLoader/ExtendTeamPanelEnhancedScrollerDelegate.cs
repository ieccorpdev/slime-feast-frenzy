using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using EnhancedScrollerDemos.MultipleCellTypesDemo;

public class ExtendTeamPanelEnhancedScrollerDelegate<T> : IEnhancedScrollerDelegate where T : EnhancedScrollerCellView
{
    public float CellSize;
    public T CellViewPrefab;
    public int NumberOfCells;
    public Data CellViewData;

    public SetDataDelegate SetDataEvent;
    public delegate void SetDataDelegate(T cellView, int dataIndex);

    //public List<T> Cells => cells;
    //List<T> cells = new List<T>();
    public void Init(float CellSize, int NumberOfCells, T CellPrefab, SetDataDelegate SetDataEvent, Data CellData)
    {
        this.CellSize = CellSize;
        this.NumberOfCells = NumberOfCells;
        this.CellViewPrefab = CellPrefab;
        this.SetDataEvent = SetDataEvent;
        this.CellViewData = CellData;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        T cellView;
        cellView = scroller.GetCellView(CellViewPrefab) as T;
        SetData(cellView, dataIndex);
        return cellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return CellSize;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return NumberOfCells;
    }

    void SetData(T cellView, int dataIndex)
    {
        SetDataEvent?.Invoke(cellView, dataIndex);
    }
}