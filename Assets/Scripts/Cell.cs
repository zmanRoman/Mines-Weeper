using UnityEngine;

public class Cell
{
    public enum Type
    {
        Empty,
        Mine,
        Number,
    }

    public Vector3Int Position;
    public Type type;
    public int Num;
    public bool IsRevealed;
    public bool IsFlagged;
    public bool IsExploded;
    public bool IsExpanded;
    
}