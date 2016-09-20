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
        BoardObject[,] player1Board = this.model.getPlayer1Board();
        addBoardObjectActivatedEventListeners(player1Board);
        BoardObject[,] player2Board = this.model.getPlayer2Board();
        addBoardObjectActivatedEventListeners(player2Board);

        List<Target> targets = this.model.getTargets();
        for (int i = 0 ; i < targets.Count ; i++) {
            targets[i].boardObjectActivated += onBoardObjectActivated;
            targets[i].boardObjectDeactivated += onBoardObjectDeactivated;
        }

        this.model.zapFired += onZapFired;
        this.model.zapExpired += onZapExpired;
        this.model.getGameBoard().targetSummaryActivated += onTargetSummaryActivated;
        this.model.getGameBoard().targetSummaryDeactivated += onTargetSummaryDeactivated;

        // create the view - one component for each element in the model.
        this.view.init(player1Board, player2Board);

        // Create the map/dictionary of model -> view elements, so can instruct changes in the view.
        AbstractBoardObjectController[,] player1BoardViews = this.view.getPlayer1Board();
        AbstractBoardObjectController[,] player2BoardViews = this.view.getPlayer2Board();
        List<TargetController> targetViews = this.view.getTargets();
        addToDictionary(player1Board, player1BoardViews);
        addToDictionary(player2Board, player2BoardViews);
        addToDictionary(targets, targetViews);

        // setup the players (views)
        ZapController player1 = this.view.createPlayer1();
        player1.playerMoveRequested += onPlayerMoveRequested;
        player1.firePressed += onFirePressed;

        ZapController player2 = this.view.createPlayer2();
        player2.playerMoveRequested += onPlayerMoveRequested;
        player2.firePressed += onFirePressed;

        // setup the players lives (view)
        List<ZapController> player1Lives = this.view.createPlayer1Lives(this.model.getNumberOfLives());
        for (int i = 0 ; i < player1Lives.Count ; i++) {
            player1Lives[i].playerMoveRequested += onPlayerMoveRequested;
            player1Lives[i].firePressed += onFirePressed;
        }

        List<ZapController> player2Lives = this.view.createPlayer2Lives(this.model.getNumberOfLives());
        for (int i = 0 ; i < player2Lives.Count ; i++) {
            player2Lives[i].playerMoveRequested += onPlayerMoveRequested;
            player2Lives[i].firePressed += onFirePressed;
        }

        // listen for the model changing the player position
        this.model.getPlayer1().playerMoved += onPlayerMoved;
        this.model.getPlayer2().playerMoved += onPlayerMoved;
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

    void onTargetSummaryActivated(GameBoard sender, EventArgs e)
    {
        this.view.onTargetSummaryActivated();
    }

    void onTargetSummaryDeactivated(GameBoard sender, EventArgs e)
    {
        this.view.onTargetSummaryDeactivated();
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

    private void onBoardObjectActivated(BoardObject sender, EventArgs e) {
        this.view.onBoardObjectActivated(this.modelToViewBoardObjects[sender]);
    }

    private void onBoardObjectDeactivated(BoardObject sender, EventArgs e) {
        this.view.onBoardObjectDeactivated(this.modelToViewBoardObjects[sender]);
    }

}