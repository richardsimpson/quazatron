using System;
using System.Collections.Generic;
using UnityEngine;

// extends MonoBehaviour so that we can wire it into the Application object in Unity
public class Controller : MonoBehaviour
{
    private Dictionary<BoardObject, AbstractBoardObjectController> modelToViewBoardObjects = new Dictionary<BoardObject, AbstractBoardObjectController>();

    private Model model;
    private View view;

    private void addBoardObjectActivatedEventListener(BoardObject boardObject) {
        boardObject.boardObjectActivated += onBoardObjectActivated;
        for (int i = 0 ; i < boardObject.getOutputs().Count ; i++) {
            addBoardObjectActivatedEventListener(boardObject.getOutputs()[i]);
        }
    }

    public void init(Model model, View view) {
        this.model = model;
        this.view = view;

        // add listeners to all model events.
        BoardObject[] inputs = this.model.getInputs();
        for (int i = 0 ; i < inputs.Length ; i++) {
            addBoardObjectActivatedEventListener(inputs[i]);
        }
        this.model.zapFired += onZapFired;

        // create the view - one component for each element in the model.
        this.view.init();

        // Create the map/dictionary of model -> view elements, so can instruct changes in the view.
        List<WireController> inputViews = this.view.getInputs();
        addToDictionary(inputs, inputViews);

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

    private void addToDictionary(BoardObject[] inputs, List<WireController> inputViews) {
        for (int i = 0 ; i < inputs.Length ; i++) {
            addToDictionary(inputs[i], inputViews[i]);
        }
    }

    private void addToDictionary(BoardObject input, AbstractBoardObjectController inputView) {
        this.modelToViewBoardObjects.Add(input, inputView);

        for (int i = 0 ; i < input.getOutputs().Count ; i++) {
            addToDictionary(input.getOutputs()[i], inputView.getOutputs()[i]);
        }
    }

    private void onZapFired(Model sender, EventArgs e)
    {
        this.view.onZapFired();
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

}