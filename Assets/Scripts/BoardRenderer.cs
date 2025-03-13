using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class BoardRenderer : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    [SerializeField] private Tile tileUnknown;
    [SerializeField] private Tile tileEmpty;
    [SerializeField] private Tile tileMine;
    [SerializeField] private Tile tileExploded;
    [SerializeField] private Tile tileFlag;
    [SerializeField] private Tile tileNum1;
    [SerializeField] private Tile tileNum2;
    [SerializeField] private Tile tileNum3;
    [SerializeField] private Tile tileNum4;
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Render(Grid grid)
    {
        var width = grid.Width;
        var height = grid.Height;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                tilemap.SetTile(cell.Position, GetTile(cell));
            }
        }
    }

    public void ClearBoard()
    {
        tilemap.ClearAllTiles();
    }
    
    private Tile GetTile(Cell cell)
    {
        if (cell.IsRevealed) {
            return GetRevealedTile(cell);
        }

        if (cell.IsFlagged) {
            return tileFlag;
        }
        if (cell.IsExpanded) {
            return tileEmpty;
        }
        return tileUnknown;
    }

    private Tile GetRevealedTile(Cell cell)
    {
        return cell.type switch
        {
            Cell.Type.Empty => tileEmpty,
            Cell.Type.Mine => cell.IsExploded ? tileExploded : tileMine,
            Cell.Type.Number => GetNumberTile(cell),
            _ => null
        };
    }

    private Tile GetNumberTile(Cell cell)
    {
        return cell.Num switch
        {
            1 => tileNum1,
            2 => tileNum2,
            3 => tileNum3,
            4 => tileNum4,
            _ => null
        };
    }
}
