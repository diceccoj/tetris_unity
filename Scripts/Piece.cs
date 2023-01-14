using System.Globalization;
using System.Threading;

using UnityEngine;

public class Piece : MonoBehaviour
{

    public Board board {get; private set;}
    public TetrominoData data {get; private set;}
    public Vector3Int position {get; private set;}
    public Vector3Int[] cells {get; private set;}
    public int rotationIndex {get; private set;}
    public float stepDelay = 1f;
    public float lockDelay = 0.5f;
    public int totalBlocksPlaced = 0;

    private float stepTime;
    private float lockTime;



    public void Initialize(Board board, Vector3Int position, TetrominoData data) {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;
        
        
        //Initializes vector for the first time
        if (this.cells == null) {
            this.cells = new Vector3Int[data.cells.Length];
        }

        //populates cells based off of array
        for(int i=0; i<data.cells.Length; i++) {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update() {

        this.board.Clear(this);
        this.lockTime += Time.deltaTime;

        //Movement
        //shift left
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            Move(Vector2Int.left); 
        }
        //shift right
        else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            Move(Vector2Int.right);
        }
        //drop down 1
        else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            Move(Vector2Int.down);
        }
        //drop to furthest available row
        else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            HardDrop();
        }
        else if(Input.GetKeyDown(KeyCode.Space)) {
            Rotate(1);
        }

        if(Time.time >= this.stepTime) {
            Step();
        }

        this.board.Set(this);
        
    }

    //implementing descent
    private void Step() {
        this.stepTime = Time.time + this.stepDelay;
        Move(Vector2Int.down);

        if(this.lockTime >= this.lockDelay) {
            Lock();
        }
    }

    //locks pieces into place when landing
    private void Lock() {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
        totalBlocksPlaced++;

        if(totalBlocksPlaced%10 == 0) {
            if(this.stepDelay > 0.1) {
                this.stepDelay -= 0.1f;
            }
        }
    }

    //Movement function
    private bool Move(Vector2Int translation) {

        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition);

        if (valid) {
            this.position = newPosition;
            this.lockTime = 0f;
        }
        return valid;
    }

    //hard drop
    private void HardDrop() {
        while(Move(Vector2Int.down)) {
            continue;
        }

        Lock();
    }



    //rotation implementation
    private void Rotate(int direction) {

        int ogRotation = this.rotationIndex;
        this.rotationIndex = wrap(this.rotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(this.rotationIndex, direction)) {
            this.rotationIndex = ogRotation;
            ApplyRotationMatrix(-direction);
        }
    }
    private void ApplyRotationMatrix(int direction) {
        for (int i=0; i<this.cells.Length; i++) {
            Vector3 cell = this.cells[i];
            int x,y;

            //repositions cells with rotation algorithm
            switch(this.data.tetromino) {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt(((cell.x)*Data.RotationMatrix[0]*direction) + (cell.y)*Data.RotationMatrix[1]*direction);
                    y = Mathf.CeilToInt(((cell.x)*Data.RotationMatrix[2]*direction) + (cell.y)*Data.RotationMatrix[3]*direction);
                    break;
                default:
                    x = Mathf.RoundToInt(((cell.x)*Data.RotationMatrix[0]*direction) + (cell.y)*Data.RotationMatrix[1]*direction);
                    y = Mathf.RoundToInt(((cell.x)*Data.RotationMatrix[2]*direction) + (cell.y)*Data.RotationMatrix[3]*direction);
                    break;
            }

            this.cells[i] = new Vector3Int(x,y,0);
        }
    }


    private int wrap(int input, int min, int max) {
        if (input < min) {
            return max - (min-input)%(max - min);
        }
        else {
            return min + (input-min)%(max-min);
        }
    }


    //wallkick testing (prevents wall clipping when rotating) 
    private bool TestWallKicks(int rotationIndex, int direction) {
        int wallKickIndex = GetWallKickIndex(rotationIndex, direction);

        //translation
        for(int i=0; i<this.data.wallKicks.GetLength(1); i++) {
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

            if (Move(translation)) {
            return true;
            }
        }
        return false;
    }
        
    private int GetWallKickIndex(int rotationIndex, int direction) {
        int wallKickIndex = rotationIndex*2;

        if (rotationIndex < 0) {
            wallKickIndex--;
        }

        return wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }

    
}
