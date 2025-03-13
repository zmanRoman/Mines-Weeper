using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10; 
    [SerializeField] private int totalMines = 10; 

    public Action OnWin;
    public Action OnLose; 
    
    [SerializeField] private BoardRenderer boardRenderer;
    private Grid _grid;
    private InputHandler _inputHandler;
    
    private bool _isGameOver;
    private bool _isGridGenerated;

    private void OnValidate()
    {
        totalMines = Mathf.Clamp(totalMines, 0, gridWidth * gridHeight);
    }

    private void Awake()
    {
        _inputHandler = new InputHandler(this);
    }

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        boardRenderer.ClearBoard();
        StopAllCoroutines();

        if (Camera.main != null) 
            Camera.main.transform.position = new Vector3(gridWidth / 2f, gridHeight / 2f, -10f);

        _isGameOver = false;
        _isGridGenerated = false;

        _grid = new Grid(gridWidth, gridHeight);
        boardRenderer.Render(_grid);
    }

    private void Update()
    {
        if (!_isGameOver)
        {
            _inputHandler.HandleInput();
        }
    }

    public void RevealCell ()
    {
        if (GetCellUnderCursor(out Cell targetCell))
        {
            if (!_isGridGenerated)
            {
                _grid.PlaceMines(targetCell, totalMines);
                _grid.CalculateAdjacentMineCounts();
                _isGridGenerated = true;
            }
            
            ProcessCellReveal(targetCell);
        }
    }

    private void ProcessCellReveal(Cell targetCell)
    {
        if (targetCell.IsRevealed || targetCell.IsFlagged) return;

        switch (targetCell.type)
        {
            case Cell.Type.Mine:
                TriggerExplosion(targetCell);
                break;
            case Cell.Type.Empty:
                StartCoroutine(ExpandEmptyArea(targetCell));
                break;
            default:
                targetCell.IsRevealed = true;
                ValidateWin();
                break;
        }

        boardRenderer.Render(_grid);
    }

    private IEnumerator ExpandEmptyArea(Cell cell)
    {
        if (_isGameOver || cell.IsRevealed || cell.type == Cell.Type.Mine) yield break;

        cell.IsRevealed = true;
        boardRenderer.Render(_grid);
        yield return null;

        if (cell.type == Cell.Type.Empty)
        {
            TryRevealNeighbor(cell.Position.x - 1, cell.Position.y);
            TryRevealNeighbor(cell.Position.x + 1, cell.Position.y);
            TryRevealNeighbor(cell.Position.x, cell.Position.y - 1);
            TryRevealNeighbor(cell.Position.x, cell.Position.y + 1);
        }

        if (!_isGameOver) 
            ValidateWin();
    }

    private void TryRevealNeighbor(int x, int y)
    {
        if (_grid.TryGetCellAt(x, y, out Cell neighbor))
        {
            StartCoroutine(ExpandEmptyArea(neighbor));
        }
    }

    public void  FlagToggle()
    {
        if (!GetCellUnderCursor(out Cell cell) || cell.IsRevealed) return;
        
        cell.IsFlagged = !cell.IsFlagged;
        boardRenderer.Render(_grid);
    }

    private void TriggerExplosion(Cell cell)
    {
        cell.IsRevealed = true;
        cell.IsExploded = true;
       _isGameOver = true;
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                cell = _grid[x, y];

                if (cell.type == Cell.Type.Mine) {
                    cell.IsRevealed = true;
                }
            }
        }
        
        OnLose?.Invoke();
    }
    

    private void ValidateWin()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Cell cell = _grid[x, y];
                
                if (cell.type != Cell.Type.Mine && !cell.IsRevealed)
                {
                    return;
                }
            }
        }

        _isGameOver = true;
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Cell cell = _grid[x, y];

                if (cell.type == Cell.Type.Mine) {
                    cell.IsFlagged = true;
                }
            }
        }
        
        OnWin?.Invoke();
    }

    private bool GetCellUnderCursor(out Cell cell)
    {
        if (Camera.main != null)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = boardRenderer.tilemap.WorldToCell(worldPosition);
            return _grid.TryGetCellAt(gridPosition.x, gridPosition.y, out cell);
        }

        cell = null;
        return false;
    }
}
