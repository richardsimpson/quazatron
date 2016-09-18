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
        BoardObject[,] board = this.model.getBoard();
        addBoardObjectActivatedEventListeners(board);

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
        this.view.init(board);

        // Create the map/dictionary of model -> view elements, so can instruct changes in the view.
        AbstractBoardObjectController[,] boardViews = this.view.getBoard();
        List<TargetController> targetViews = this.view.getTargets();
        addToDictionary(board, boardViews);
        addToDictionary(targets, targetViews);

        // setup the player (view)
        PlayerController player = this.view.createPlayer();
        player.playerMoveRequested += onPlayerMoveRequested;
        player.firePressed += onFirePressed;

        // setup the players lives (view)
        List<PlayerController> playerLives = this.view.createLives(this.model.getNumberOfLives());
        for (int i = 0 ; i < playerLives.Count ; i++) {
            playerLives[i].playerMoveRequested += onPlayerMoveRequested;
            playerLives[i].firePressed += onFirePressed;
        }

        // listen for the model changing the player position
        this.model.getCurrentPlayer().playerMoved += onPlayerMoved;
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

    private void onZapFired(Model sender, EventArgs e)
    {
        this.view.onZapFired();
    }

    private void onZapExpired(Model sender, ZapExpiredEventArgs e)
    {
        this.view.onZapExpired(e.playerPosition);
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
        this.model.onPlayerMoveRequested(e.direction);
    }

    void onFirePressed(object sender, EventArgs e)
    {
        this.model.onFirePressed();
    }

    private void onPlayerMoved(object sender, PlayerMovedEventArgs e) 
    {
        this.view.onPlayerMoved(e.position);
    }

    private void onBoardObjectActivated(BoardObject sender, EventArgs e) {
        this.view.onBoardObjectActivated(this.modelToViewBoardObjects[sender]);
    }

    private void onBoardObjectDeactivated(BoardObject sender, EventArgs e) {
        this.view.onBoardObjectDeactivated(this.modelToViewBoardObjects[sender]);
    }

}