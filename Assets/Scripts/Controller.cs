using System;
using System.Collections.Generic;
using UnityEngine;

// extends MonoBehaviour so that we can wire it into the Application object in Unity
public class Controller : MonoBehaviour
{
    private const float INITIAL_Y = 4f;
    private const float Y_INCREMENT = 0.75f;

    public TargetController targetPrefab;
    public WireController wirePrefab;

    private Dictionary<BoardObject, WireController> modelToViewBoardObjects = new Dictionary<BoardObject, WireController>();
    private Dictionary<BoardObject, TargetController> modelToViewTargets = new Dictionary<BoardObject, TargetController>();

    private Model model;
    private View view;

    public void init(Model model, View view) {
        this.model = model;
        this.view = view;

        // add listeners to all model events.
        BoardObject[] inputs = this.model.getInputs();
        for (int i = 0 ; i < inputs.Length ; i++) {
            ((Wire)inputs[i]).wireActivated += onWireActivated;
        }
        Target[] targets = this.model.getTargets();
        for (int i = 0 ; i < targets.Length ; i++) {
            targets[i].targetActivated += onTargetActivated;
        }
        this.model.zapFired += onZapFired;

        // create the view - one component for each element in the model.
        TargetController[] targetViews = constructTargetViews();
        this.view.setTargets(targetViews);

        WireController[] inputViews = constructInputViews();
        this.view.setInputs(inputViews);

        // Create the map/dictionary of model -> view elements, so can instruct changes in the view.
        for (int i = 0 ; i < inputs.Length ; i++) {
            this.modelToViewBoardObjects.Add(inputs[i], inputViews[i]);
        }
        for (int i = 0 ; i < targets.Length ; i++) {
            this.modelToViewTargets.Add(targets[i], targetViews[i]);
        }

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

    void onZapFired(Model sender, EventArgs e)
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

    private void onWireActivated(Wire sender, EventArgs e) {
        this.view.onWireActivated(this.modelToViewBoardObjects[sender]);
    }

    private void onTargetActivated(Target sender, EventArgs e) {
        this.view.onTargetActivated(this.modelToViewTargets[sender]);
    }

    private TargetController[] constructTargetViews() {
        TargetController[] targetViews = new TargetController[12];

        float yPos = INITIAL_Y;

        for (int index = 0 ; index < targetViews.Length ; index++) {
            targetViews[index] = Instantiate(targetPrefab);
            targetViews[index].transform.position = new Vector3(0, yPos, 0);
            yPos = yPos - Y_INCREMENT;
        }

        return targetViews;
    }

    private WireController[] constructInputViews() {
        WireController[] inputViews = new WireController[12];

        float yPos = INITIAL_Y;

        for (int index = 0 ; index < inputViews.Length ; index++) {
            inputViews[index] = Instantiate(wirePrefab);
            inputViews[index].transform.position = new Vector3(-3.27f, yPos, 0);
            yPos = yPos - Y_INCREMENT;
        }

        return inputViews;
    }

}