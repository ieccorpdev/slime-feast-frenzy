using System;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;

public class EnhancedScrollerTeamPanelController<TOne, TTwo> where TOne : EnhancedScrollerCellView
{
    public float cellSize = 220;
    public int ItemPerLine = 4;
    public int preloadValue = 2;


    //Private
    int _index_currentPage = 0;
    int _index_totalPage = 0;
    public bool IsLoading = false;

    EnhancedScroller enhancedScroller;
    TOne prefabScroller;

    List<TTwo> Datas = new List<TTwo>();


    #region Actions
    public delegate void SetDataDelegate(TOne cellView, int dataIndex, List<TTwo> packs);
    //Action To Show Data:
    SetDataDelegate SetDataAction;
    //Action To load Data:
    Action<int> onGetDataByPageAction;
    //Action when reach limit of Data but we still have to wait
    Action onGetDataHaveToWaitAction;
    #endregion

    public void Init(EnhancedScroller enhancedScroller, TOne prefabScroller)
    {
        this.enhancedScroller = enhancedScroller;
        this.prefabScroller = prefabScroller;

        this.enhancedScroller.ScrollPosition = 0;
        Datas.Clear();
        _index_currentPage = 0;
    }

    public void InitAction(Action<int> onGetDataByPageAction, SetDataDelegate SetDataAction, Action onGetDataHaveToWaitAction)
    {
        this.onGetDataByPageAction = onGetDataByPageAction;
        this.SetDataAction = SetDataAction;
        this.onGetDataHaveToWaitAction = onGetDataHaveToWaitAction;
    }

    public void ResetData()
    {
        this.enhancedScroller.ScrollPosition = 0;
    
        _index_currentPage = 0;
        _index_totalPage = 0;
        IsLoading = false;
        Datas.Clear();
    }
    public void StartLoadData()
    {
        Datas.Clear();
        GetDataByPage(0);
    }

    public void OnLoadDone(int _pageCurrent, int _pageTotal, int _totalItem, TTwo[] newDatas)
    {

        _index_currentPage = _pageCurrent;
        _index_totalPage = _pageTotal;

        IsLoading = false;
        foreach (var pack in newDatas)
        {
            if (!Datas.Contains(pack))
                Datas.Add(pack);
        }

        int _totalLine = (int)Math.Ceiling((double)_totalItem / (double)ItemPerLine);
        int _totalLoadedLine = (int)Math.Ceiling((double)Datas.Count / (double)ItemPerLine);

        ExtendEnhancedScrollerDelegate<TOne> scrollerDelegate = new ExtendEnhancedScrollerDelegate<TOne>();
        scrollerDelegate.Init(cellSize, _totalLine, prefabScroller, (cellView, index) =>
        {
            int _startDataOfLine = ItemPerLine * index;
            List<TTwo> packs = new List<TTwo>();
            for (int i = _startDataOfLine; i < _startDataOfLine + ItemPerLine; i++)
            {
                if (i < Datas.Count)
                    packs.Add(Datas[i]);
            }
            SetDataAction?.Invoke(cellView, index, packs);

            if (!IsLoading && _totalLoadedLine - index <= preloadValue)
            {
                //Time to load
                if (_totalLoadedLine - index <= 0)
                {
                    onGetDataHaveToWaitAction?.Invoke();
                    // waitingPanel.gameObject.SetActive(true);
                }
                if (_index_currentPage < _index_totalPage)
                    GetDataByPage(_index_currentPage + 1);
            }
        });
        
        enhancedScroller.Delegate = scrollerDelegate;
        enhancedScroller.ReloadData();

    }

    void GetDataByPage(int page)
    {
        IsLoading = true;
        onGetDataByPageAction?.Invoke(page);
    }
}