using System;
using System.Collections.Generic;
using UnityEngine;

// Game Board Description
//
// Each side of the game board follows this pattern:
//
// P1--2--3--[] 
//
// Where:
//     P is the column that the player moved up and down in
//     1,2,3 are the locations of board objects (splitters, terminators, etc
//     [] is the central column
//
// When firing a 'zap', the zap is moved into column '1'.
//
// Note that if there is a terminator in '1', then the player cannot fire into that row.
// Note also that a player cannot fire into a row that alrady has an active zap.

// NOTE: This will ultimately indicate which player is winning, via an enum (just like wires), but for now the event is empty.
public delegate void TargetSummaryUpdatedEventHandler(GameBoard sender, TargetSummaryUpdatedEventArgs e);

public delegate void SideChangedEventHandler(GameBoard sender, SideChangedEventArgs e);

public class TargetSummaryUpdatedEventArgs : EventArgs
{
    public PlayerNumber playerNumber;

    public TargetSummaryUpdatedEventArgs(PlayerNumber playerNumber)
    {
        this.playerNumber = playerNumber;
    }    
}

public class SideChangedEventArgs : EventArgs
{
    public Side side;

    public SideChangedEventArgs(Side side)
    {
        this.side = side;
    }

}

public class GameBoard
{
    public event TargetSummaryUpdatedEventHandler targetSummaryUpdated;
    public event SideChangedEventHandler sideChanged;

    private const int ROW_COUNT = 12;
    private const int MAX_TERMINATOR_COUNT = 6;

    private List<Target> targets = new List<Target>();
    private AbstractBoardObject[,] leftBoard = new AbstractBoardObject[3,ROW_COUNT];
    private AbstractBoardObject[,] rightBoard = new AbstractBoardObject[3,ROW_COUNT];

    private Side player1Side = Side.LEFT;

	public GameBoard()
	{
        for (int i = 0 ; i < ROW_COUNT ; i++) {
            Target target = new Target();
            target.boardObjectActivated += onTargetActivatedStateChanged;
            this.targets.Add(target);
        }

        createBoard(this.leftBoard);
        createBoard(this.rightBoard);
	}

    private void createExampleBoard(AbstractBoardObject[,] board) {
        // setup this board:
        //        1      2      3
        //
        //     1  ---------------------
        //     2  <      --------------
        //     3  -------|
        //     4  <      --------------
        //     5  ------->-------------
        //     6  --------------o------
        //     7  -------<
        //     8  -------
        //     9  <      |------
        //    10  -------       |------ 
        //    11  --------------
        //    12  ---------------------

        // column 3 first:
        board[2, 0] = new Wire(this.targets[0]);
        board[2, 1] = new Wire(this.targets[1]);
        board[2, 2] = null;
        board[2, 3] = new Wire(this.targets[3]);
        board[2, 4] = new Wire(this.targets[4]);
        board[2, 5] = new Swapper(this.targets[5]);
        board[2, 6] = null;
        board[2, 7] = null;
        board[2, 8] = null;
        board[2, 9] = new Connector(this.targets[9]);
        board[2, 10] = null;
        board[2, 11] = new Wire(this.targets[11]);

        board[1, 0] = new Wire(board[2, 0]);
        board[1, 1] = null;
        List<AbstractBoardObject> outputs = new List<AbstractBoardObject>();
        outputs.Add(board[2, 1]);
        outputs.Add(board[2, 3]);
        board[1, 2] = new Connector(outputs);
        board[1, 3] = null;
        board[1, 4] = new Initiator(board[2, 4]);
        board[1, 5] = new Wire(board[2, 5]);
        board[1, 6] = new Terminator();
        board[1, 7] = null;
        board[1, 8] = new Connector(board[2, 9]);
        board[1, 9] = null;
        board[1, 10] = new Wire(board[2, 9]);
        board[1, 11] = new Wire(board[2, 11]);

        board[0, 0] = new Wire(board[1, 0]);
        board[0, 1] = new Terminator();
        board[0, 2] = new Wire(board[1, 2]);
        board[0, 3] = new Terminator();
        board[0, 4] = new Wire(board[1, 4]);
        board[0, 5] = new Wire(board[1, 5]);
        board[0, 6] = new Wire(board[1, 6]);
        board[0, 7] = new Wire(board[1, 8]);
        board[0, 8] = new Terminator();
        board[0, 9] = new Wire(board[1, 8]);
        board[0, 10] = new Wire(board[1, 10]);
        board[0, 11] = new Wire(board[1, 11]);
    }

    private enum ObjectType {
        None, Terminator, Wire, Swapper, Initiator, Connector2To1, Connector1To2
    }

    private System.Random random = new System.Random();

    private AbstractBoardObject getOutput(int column, int row, AbstractBoardObject[,] board, ObjectType[,] tempBoard) {
        if (column==2) {
            return this.targets[row];
        }
        else if (board[column+1, row] != null) {
            return board[column+1, row];
        }
        else if ((row > 0) && (board[column+1, row-1] is Connector)) {
            return board[column+1, row-1];
        }
        else if ((row < ROW_COUNT-1) && (board[column+1, row+1] is Connector)) {
            return board[column+1, row+1];
        }
        else {
            throw new Exception("Cannot locate output for object " + tempBoard[column, row]);
        }
    }

    private List<AbstractBoardObject> getOutputsForConnector1To2(int column, int row, AbstractBoardObject[,] board, ObjectType[,] tempBoard) {
        List<AbstractBoardObject> outputs = new List<AbstractBoardObject>();
        if (column==2) {
            outputs.Add(this.targets[row-1]);
            outputs.Add(this.targets[row+1]);
        }
        else {
            outputs.Add(getOutput(column, row-1, board, tempBoard));
            outputs.Add(getOutput(column, row+1, board, tempBoard));
        }

        return outputs;
    }

    private void createBoard(AbstractBoardObject[,] board) {
        ObjectType[,] tempBoard = new ObjectType[board.GetLength(0), board.GetLength(1)];
        int numberOfTerminators = 0;

        // populate the grid
        for (int i = 0 ; i <= 2 ; i++) {
            for (int j = 0 ; j < ROW_COUNT ; j++) {
                if (i == 0) {
                    ObjectType newObject = selectFirstColumnObject(tempBoard, i, j, numberOfTerminators);
                    if (ObjectType.Terminator == newObject) {
                        numberOfTerminators++;
                    }
                    tempBoard[i,j] = newObject;

                }
                else {
                    ObjectType newObject = selectObject(tempBoard, i, j, numberOfTerminators);
                    if (ObjectType.Terminator == newObject) {
                        numberOfTerminators++;
                    }
                    tempBoard[i,j] = newObject;

                    if ((tempBoard[i,j] == ObjectType.Connector1To2) || (tempBoard[i,j] == ObjectType.Connector2To1)) {
                        tempBoard[i,j+1] = ObjectType.None;
                    }
                }
            }
        }

        // now convert the tempBoard to a real board
        for (int i = 2 ; i >= 0 ; i--) {
            for (int j = 0 ; j < ROW_COUNT ; j++) {
                switch(tempBoard[i,j]) {
                case ObjectType.None:
                    board[i,j] = null;
                    break;
                case ObjectType.Terminator:
                    board[i,j] = new Terminator();
                    break;
                case ObjectType.Wire:
                    board[i,j] = new Wire(getOutput(i, j, board, tempBoard));
                    break;
                case ObjectType.Swapper:
                    board[i,j] = new Swapper(getOutput(i, j, board, tempBoard));
                    break;
                case ObjectType.Initiator:
                    board[i,j] = new Initiator(getOutput(i, j, board, tempBoard));
                    break;
                case ObjectType.Connector2To1:
                    board[i,j] = new Connector(getOutput(i, j, board, tempBoard));
                    break;
                case ObjectType.Connector1To2:
                    board[i,j] = new Connector(getOutputsForConnector1To2(i, j, board, tempBoard));
                    break;
                }
            }
        }
    }

    private ObjectType selectFirstColumnObject(ObjectType[,] board, int column, int row, int currentNumberOfTerminators) {
        if (currentNumberOfTerminators >= MAX_TERMINATOR_COUNT) {
            return ObjectType.Wire;
        }

        // Don't allow two terminators together
        if ((row > 0) && (board[column, row-1] == ObjectType.Terminator)) {
            return ObjectType.Wire;
        }

        // twice as much chance of getting a Wire than a Terminator
        int index = this.random.Next(0, 3);

        if (index == 0) {
            return ObjectType.Terminator;
        }

        return ObjectType.Wire;
    }

    private bool objectHasWireOutputOnSameRow(ObjectType objectType) {
        return (ObjectType.Wire == objectType) || (ObjectType.Swapper == objectType) 
            || (ObjectType.Initiator == objectType) || (ObjectType.Connector2To1 == objectType);
    }

    private ObjectType selectObject(ObjectType[,] board, int column, int row, int currentNumberOfTerminators) {
        List<ObjectType> possibleObjects = new List<ObjectType>();

        if ((row > 0) && ((board[column, row-1] == ObjectType.Connector1To2) || (board[column, row-1] == ObjectType.Connector2To1))) {
            return ObjectType.None;
        }

        // see if this HAS to be a 2 to 1 connector
        if ((row > 0) && (row < ROW_COUNT-1) && (objectHasWireOutputOnSameRow(board[column-1, row-1])) 
            && (board[column, row-1] == ObjectType.None)) {
            if (row == 1) {
                return ObjectType.Connector2To1;
            }
            else if ((board[column, row-2] != ObjectType.Connector1To2) && (board[column, row-2] != ObjectType.Connector2To1)) {
                return ObjectType.Connector2To1;
            }
        }

        if ((objectHasWireOutputOnSameRow(board[column-1, row])) 
            || (row > 0 && board[column-1, row-1] == ObjectType.Connector1To2) 
            || (row < ROW_COUNT-1 && board[column-1, row+1] == ObjectType.Connector1To2)) {

            // Add Wire twice, so it has a higher chance of being selected
            possibleObjects.Add(ObjectType.Wire);
            possibleObjects.Add(ObjectType.Wire);

            // Don't allow two terminators together
            if ((row > 0) && (board[column, row-1] != ObjectType.Terminator)) {
                if (currentNumberOfTerminators < MAX_TERMINATOR_COUNT) {
                    possibleObjects.Add(ObjectType.Terminator);
                }
            }
            // only allow swappers in the last column (to avoid having two on one row, and to avoid having connectors with different coloured inputs.
            if (column == 2) {
                possibleObjects.Add(ObjectType.Swapper);
            }
            possibleObjects.Add(ObjectType.Initiator);

            // see if this can be null, with a 2 to 1 connector in the row below
            if (objectHasWireOutputOnSameRow(board[column-1, row])) {
                if ((row < ROW_COUNT-2) && (!objectHasWireOutputOnSameRow(board[column-1, row+1])) && (objectHasWireOutputOnSameRow(board[column-1, row+2]))) {
                    possibleObjects.Add(ObjectType.None);

                }
            }

            // see if this can be a 1 to 2 connector
            if ((row > 0) && (row < ROW_COUNT-1) && (!objectHasWireOutputOnSameRow(board[column-1, row-1])) 
                && (!objectHasWireOutputOnSameRow(board[column-1, row+1])) && (board[column, row-1] == ObjectType.None)) {

                if ((row == 1) || (row == ROW_COUNT-2)) {
                    possibleObjects.Add(ObjectType.Connector1To2);
                }
                else if ((board[column, row-2] != ObjectType.Connector1To2) && (board[column, row-2] != ObjectType.Connector2To1)
                    && (board[column-1, row-2] != ObjectType.Connector1To2) && (board[column-1, row+2] != ObjectType.Connector1To2)) {
                    
                    possibleObjects.Add(ObjectType.Connector1To2);
                }
            }
        }
        else {
            possibleObjects.Add(ObjectType.None);

            if ((row > 0) && (row < ROW_COUNT-1) 
                && (objectHasWireOutputOnSameRow(board[column-1, row-1])) && (objectHasWireOutputOnSameRow(board[column-1, row+1]))
                && (board[column, row-1] == ObjectType.None)) {

                // make sure we don't have a connector too near another
                if (row < 2) {
                    possibleObjects.Add(ObjectType.Connector2To1);
                }
                else if ((board[column, row-2] != ObjectType.Connector1To2) && (board[column, row-2] != ObjectType.Connector2To1)) {
                    possibleObjects.Add(ObjectType.Connector2To1);
                }
            }
        }

        return possibleObjects[this.random.Next(0, possibleObjects.Count)];
    }
            
    public List<Target> getTargets() {
        return this.targets;
    }

    public BoardObject[,] getLeftBoard() {
        return this.leftBoard;
    }

    public BoardObject[,] getRightBoard() {
        return this.rightBoard;
    }

    private BoardObject[,] getPlayer1Board() {
        if (this.player1Side == Side.LEFT) {
            return this.leftBoard;
        }
        return this.rightBoard;
    }

    private BoardObject[,] getPlayer2Board() {
        if (this.player1Side == Side.LEFT) {
            return this.rightBoard;
        }
        return this.leftBoard;
    }

    public BoardObject[,] getBoardForPlayerNumber(PlayerNumber playerNumber) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            return getPlayer1Board();
        }

        return getPlayer2Board();
    }

    public void onFirePressed(PlayerNumber playerNumber, int playerPosition)
    {
        BoardObject[,] board = getBoardForPlayerNumber(playerNumber);
        board[0, playerPosition].inputActivated(null, playerNumber);
    }

    // model is only passed in so that the Initiator can use it to call StartCoroutine
    public void onPlayerRemoved(Model model, PlayerNumber playerNumber, int playerPosition)
    {
        BoardObject[,] board = getBoardForPlayerNumber(playerNumber);
        board[0, playerPosition].inputDeactivated(model, null);
    }

    protected void OnTargetSummaryUpdated(TargetSummaryUpdatedEventArgs eventArgs) {
        Debug.Log("OnTargetSummaryUpdated.");
        if (targetSummaryUpdated != null)
            targetSummaryUpdated(this, eventArgs);
    }

    private void recalculateTargetSummaryState() {
        int player1Targets = 0;
        int player2Targets = 0;

        for (int i = 0 ; i < this.targets.Count ; i++) {
            PlayerNumber controllingPlayer = this.targets[i].getControllingPlayer();
            if (PlayerNumber.PLAYER1 == controllingPlayer) {
                player1Targets = player1Targets + 1;
            }
            else if (PlayerNumber.PLAYER2 == controllingPlayer) {
                player2Targets = player2Targets + 1;
            }
        }

        if (player1Targets > player2Targets) {
            OnTargetSummaryUpdated(new TargetSummaryUpdatedEventArgs(PlayerNumber.PLAYER1));
        }
        else if (player1Targets < player2Targets) {
            OnTargetSummaryUpdated(new TargetSummaryUpdatedEventArgs(PlayerNumber.PLAYER2));
        }
        else {
            OnTargetSummaryUpdated(new TargetSummaryUpdatedEventArgs(PlayerNumber.NEITHER));
        }
    }

    private void onTargetActivatedStateChanged(BoardObject sender, BoardObjectActivatedEventArgs e) {
        recalculateTargetSummaryState();
    }

    public void onSideChangedRequested(Side side)
    {
        if (this.player1Side != side) {
            this.player1Side = side;

            onSideChanged(new SideChangedEventArgs(side));
        }
    }

    private void onSideChanged(SideChangedEventArgs eventArgs) {
        Debug.Log("onSideChanged: " + eventArgs.side);
        if (sideChanged != null)
            sideChanged(this, eventArgs);
    }

}

