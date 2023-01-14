using System.Runtime.CompilerServices;
using System.Globalization;
using UnityEngine.Tilemaps;
using UnityEngine;


//list of tetromino shapes
public enum Tetromino {
    I,O,T,J,S,Z,L,
}


[System.Serializable]

//declarations
public struct TetrominoData {
    public Tetromino tetromino;
    public Tile tile;

    public Vector2Int[] cells { get; set;}
    public Vector2Int[,] wallKicks { get; private set; }

    public void Initialize() {
        this.cells = Data.Cells[this.tetromino];
        this.wallKicks = Data.WallKicks[this.tetromino];
    }

}