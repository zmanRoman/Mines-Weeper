using UnityEngine;

public class Grid
{
    private readonly Cell[,] _cellArray; 
    public int Width => _cellArray.GetLength(0);
    public int Height => _cellArray.GetLength(1); 

    public Cell this[int x, int y] => _cellArray[x, y];

    public Grid(int width, int height)
    {
        _cellArray = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _cellArray[x, y] = new Cell
                {
                    Position = new Vector3Int(x, y, 0),
                    type = Cell.Type.Empty 
                };
            }
        }
    }

    public void PlaceMines(Cell startCell, int mineCount) 
    {
        int maxWidth = Width;
        int maxHeight = Height;

        for (int i = 0; i < mineCount; i++) 
        {
            int randX = Random.Range(0, maxWidth);
            int randY = Random.Range(0, maxHeight);

            Cell currentCell = _cellArray[randX, randY];

            while (currentCell.type == Cell.Type.Mine || AreCellsAdjacent(startCell, currentCell))
            {
                randX++;

                if (randX >= maxWidth) 
                {
                    randX = 0; 
                    randY++;

                    if (randY >= maxHeight) 
                    {
                        randY = 0;
                    }
                }

                currentCell = _cellArray[randX, randY]; 
            }

            currentCell.type = Cell.Type.Mine; 
        }
    }

    public void CalculateAdjacentMineCounts() 
    {
        int maxWidth = Width;
        int maxHeight = Height; 

        for (int x = 0; x < maxWidth; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                Cell currentCell = _cellArray[x, y];

                if (currentCell.type == Cell.Type.Mine)
                {
                    continue; // continue;
                }

                currentCell.Num = GetAdjacentMineCount(currentCell);
                currentCell.type = currentCell.Num > 0 ? Cell.Type.Number : Cell.Type.Empty;
            }
        }
    }

    private int GetAdjacentMineCount(Cell cell)
    {
        int count = 0;

        for (int deltaX = -1; deltaX <= 1; deltaX++) 
        {
            for (int deltaY = -1; deltaY <= 1; deltaY++)
            {
                if (deltaX == 0 && deltaY == 0)
                {
                    continue;
                }

                int newX = cell.Position.x + deltaX; 
                int newY = cell.Position.y + deltaY;

                if (IsValidCoordinate(newX, newY) && _cellArray[newX, newY].type == Cell.Type.Mine)
                {
                    count++; 
                }
            }
        }

        return count;
    }

    public int GetAdjacentFlagCount(Cell cell) // CountAdjacentFlags(Cell cell)
    {
        int count = 0; 

        for (int deltaX = -1; deltaX <= 1; deltaX++) 
        {
            for (int deltaY = -1; deltaY <= 1; deltaY++) 
            {
                if (deltaX == 0 && deltaY == 0)
                {
                    continue; 
                }

                int newX = cell.Position.x + deltaX; 
                int newY = cell.Position.y + deltaY;

                if (IsValidCoordinate(newX, newY) && !_cellArray[newX, newY].IsRevealed && _cellArray[newX, newY].IsFlagged)
                {
                    count++;
                }
            }
        }

        return count; 
    }

    public Cell GetCellAt(int x, int y)
    {
        return IsValidCoordinate(x, y) ? _cellArray[x, y] : null;
    }

    public bool TryGetCellAt(int x, int y, out Cell cell) 
    {
        cell = GetCellAt(x, y); 
        return cell != null; 
    }

    public bool IsValidCoordinate(int x, int y) 
    {
        return x >= 0 && x < Width && y >= 0 && y < Height; 
    }

    public bool AreCellsAdjacent(Cell cell1, Cell cell2) 
    {
        return Mathf.Abs(cell1.Position.x - cell2.Position.x) <= 1 && 
               Mathf.Abs(cell1.Position.y - cell2.Position.y) <= 1;
    }
}
