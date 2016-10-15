using System;
using System.Collections.Generic;
using UnityEngine;

// extends MonoBehaviour so that we can wire it into the Application object in Unity
public class Controller : MonoBehaviour
{
    private Model model;
    private View view;

    private Dictionary<BoardObject, AbstractBoardObjectController> modelToViewBoardObjects = new Dictionary<BoardObject, AbstractBoardObjectController>();

    private void addBoardObjectActivatedEventListeners(BoardObject[,] board) {
        for (int i = 0 ; i < board.GetLength(0) ; i++) {
            for (int j = 0 ; j < board.GetLength(1) ; j++) {
                if (board[i,j] != null) {
                    board[i,j].boardObjectActivated += onBoardObjectActivated;
                    board[i,j].boardObjectDeactivated += onBoardObjectDeactivated;
                }
            }
        }
    }

    public void init(Model model, View view) {
        this.model = model;
        this.view = view;

        // add listeners to all model events.
        BoardObject[,] leftBoard = this.model.getLeftBoard();
        addBoardObjectActivatedEventListeners(leftBoard);
        BoardObject[,] rightBoard = this.model.getRightBoard();
        addBoardObjectActivatedEventListeners(rightBoard);

        List<Target> targets = this.model.getTargets();
        for (int i = 0 ; i < targets.Count ; i++) {
            targets[i].boardObjectActivated += onBoardObjectActivated;
            targets[i].boardObjectDeactivated += onBoardObjectDeactivated;
        }

        this.model.zapFired += onZapFired;
        this.model.zapExpired += onZapExpired;
        this.model.getGameBoard().targetSummaryUpdated += onTargetSummaryUpdated;
        this.model.getGameBoard().sideChanged += onSideChanged;

        // create the view - one component for each element in the model.
        this.view.init(leftBoard, rightBoard);

        // Create the map/dictionary of model -> view elements, so can instruct changes in the view.
        AbstractBoardObjectController[,] leftBoardViews = this.view.getLeftBoard();
        AbstractBoardObjectController[,] rightBoardViews = this.view.getRightBoard();
        List<TargetController> targetViews = this.view.getTargets();
        addToDictionary(leftBoard, leftBoardViews);
        addToDictionary(rightBoard, rightBoardViews);
        addToDictionary(targets, targetViews);

        // setup the players (views)
        ZapController player1 = this.view.createPlayer1();
        player1.playerMoveRequested += onPlayerMoveRequested;
        player1.firePressed += onFirePressed;
        player1.sideChangeRequested += onSideChangedRequested;

        ZapController player2 = this.view.createPlayer2();
        player2.playerMoveRequested += onPlayerMoveRequested;
        player2.firePressed += onFirePressed;

        // setup the players lives (view)
        List<ZapController> player1Lives = this.view.createPlayer1Lives(this.model.getNumberOfLives());
        for (int i = 0 ; i < player1Lives.Count ; i++) {
            player1Lives[i].playerMoveRequested += onPlayerMoveRequested;
            player1Lives[i].firePressed += onFirePressed;
            player1.sideChangeRequested += onSideChangedRequested;
        }

        List<ZapController> player2Lives = this.view.createPlayer2Lives(this.model.getNumberOfLives());
        for (int i = 0 ; i < player2Lives.Count ; i++) {
            player2Lives[i].playerMoveRequested += onPlayerMoveRequested;
            player2Lives[i].firePressed += onFirePressed;
        }

        // listen for the model changing the player position
        this.model.getPlayer1().playerMoved += onPlayerMoved;
        this.model.getPlayer2().playerMoved += onPlayerMoved;

        // setup the targets to an initial state where they alternate between player1 and player2
        setTargetsInitialState(Side.LEFT);

        this.view.gameStart += onGameStart;
    }

    private void onGameStart(View sender, GameStartEventArgs e)
    {
        setTargetsInitialState(e.player1Side);
    }

    private void setTargetsInitialState(Side player1Side) {
        // setup the targets to an initial state where they alternate between yellow and blue
        PlayerNumber playerA;
        PlayerNumber playerB;

        // use player1Side to set the corrent owners (colours)
        if (Side.LEFT == player1Side) {
            playerA = PlayerNumber.PLAYER1;
            playerB = PlayerNumber.PLAYER2;
        }
        else {
            playerA = PlayerNumber.PLAYER2;
            playerB = PlayerNumber.PLAYER1;
        }

        List<Target> targets = this.model.getTargets();

        for (int i = 0; i < targets.Count ; i++) {
            if (i % 2 == 0) {
                targets[i].setControllingPlayer(playerA);
            }
            else {
                targets[i].setControllingPlayer(playerB);
            }
        }
    }

    private void addToDictionary(BoardObject[,] board, AbstractBoardObjectController[,] boardViews) {
        for (int i = 0 ; i < board.GetLength(0) ; i++) {
            for (int j = 0 ; j < board.GetLength(1) ; j++) {
                if (board[i,j] != null) {
                    this.modelToViewBoardObjects.Add(board[i,j], boardViews[i,j]);
                }
            }
        }
    }

    private void addToDictionary(List<Target> targets, List<TargetController> targetViews) {
        for (int i = 0 ; i < targets.Count ; i++) {
            this.modelToViewBoardObjects.Add(targets[i], targetViews[i]);
        }
    }

    private void onZapFired(Model sender, ZapFiredEventArgs e)
    {
        this.view.onZapFired(e.playerNumber);
    }

    private void onZapExpired(Model sender, ZapExpiredEventArgs e)
    {
        this.view.onZapExpired(e.playerNumber, e.playerPosition);
    }

    private void onTargetSummaryUpdated(GameBoard sender, TargetSummaryUpdatedEventArgs e)
    {
        this.view.onTargetSummaryUpdated(e.playerNumber);
    }

    private void onPlayerMoveRequested(object sender, PlayerMoveRequestedEventArgs e) 
    {
        this.model.onPlayerMoveRequested(e.playerNumber, e.direction);
    }

    private void onFirePressed(object sender, FirePressedEventArgs e)
    {
        this.model.onFirePressed(e.playerNumber);
    }

    private void onPlayerMoved(object sender, PlayerMovedEventArgs e) 
    {
        this.view.onPlayerMoved(e.playerNumber, e.position);
    }

    private void onBoardObjectActivated(BoardObject sender, BoardObjectActivatedEventArgs e) {
        this.view.onBoardObjectActivated(this.modelToViewBoardObjects[sender], e.playerNumber);
    }

    private void onBoardObjectDeactivated(BoardObject sender, EventArgs e) {
        this.view.onBoardObjectDeactivated(this.modelToViewBoardObjects[sender]);
    }

    private void onSideChangedRequested(object sender, SideChangeRequestedEventArgs e) {
        this.model.onSideChangedRequested(e.side);
    }

    private void onSideChanged(GameBoard sender, SideChangedEventArgs e) {
        this.view.onSideChanged(e.side);
    }

}