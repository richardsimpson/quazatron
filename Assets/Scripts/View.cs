using System;
using System.Collections.Generic;
using UnityEngine;

// extends MonoBehaviour so that we can wire it into the Application object in Unity
public class View : MonoBehaviour
{
    private const float INITIAL_Y = 3f;
    private const float Y_INCREMENT = 0.60f;

    private const float LIVES_X = -8f;
    private const float LIVES_INITIAL_Y = 4f;

    private const int ROW_COUNT = 12;

    public TargetSummaryController targetSummaryPrefab;
    public TargetController targetPrefab;
    public WireController wirePrefab;
    public InitiatorController initiatorPrefab;
    public SwapperController swapperPrefab;
    public TerminatorController terminatorPrefab;
    public ConnectorController connectorOneOutputPrefab;
    public ConnectorController connectorTwoOutputPrefab;
    public PlayerController playerPrefab;

    private List<TargetController> targets = new List<TargetController>();
    private AbstractBoardObjectController[,] boardViews = new AbstractBoardObjectController[3, ROW_COUNT];

    private PlayerController player;
    private List<PlayerController> playerLives = new List<PlayerController>();
    private List<PlayerController> oldPlayers = new List<PlayerController>();

    private float colZeroXPos;
    private float colWidth;

    public void init(BoardObject[,] board) {
        this.colZeroXPos = targetPrefab.transform.position.x - (targetPrefab.transform.localScale.x/2)
            - (wirePrefab.transform.localScale.x/2) - (wirePrefab.transform.localScale.x * 2);

        this.colWidth = wirePrefab.transform.localScale.x;

        constructTargetViews();
        constructTargetSummary();
        constructInputViews(board);
    }

    private void constructTargetSummary() {
        TargetSummaryController targetSummary = Instantiate(targetSummaryPrefab);
        float posY = INITIAL_Y + this.targets[0].transform.localScale.y/2 + targetSummary.transform.localScale.y/2 + 0.10f; //0.10f = Y_INCREMENT - height of targetPrefab
        targetSummary.transform.position = new Vector3(0, posY, 0);
    }

    private void constructTargetViews() {
        float yPos = INITIAL_Y;

        for (int index = 0 ; index < ROW_COUNT ; index++) {
            TargetController newTarget = Instantiate(targetPrefab);
            newTarget.transform.position = new Vector3(0, yPos, 0);

            this.targets.Add(newTarget);

            yPos = yPos - Y_INCREMENT;
        }
    }

    // TODO: See if these methods can be collapsed into a single one
    private WireController constructWire(int column, int row) {
        float yPos = INITIAL_Y - (row * Y_INCREMENT);

        WireController result = Instantiate(wirePrefab);

        result.transform.position = new Vector3(this.colZeroXPos+(this.colWidth*column), yPos, 0);
        return result;
    }

    private InitiatorController constructInitiator(int column, int row) {
        float yPos = INITIAL_Y - (row * Y_INCREMENT);

        InitiatorController result = Instantiate(initiatorPrefab);

        result.transform.position = new Vector3(this.colZeroXPos+(this.colWidth*column), yPos, 0);
        return result;
    }

    private SwapperController constructSwapper(int column, int row) {
        float yPos = INITIAL_Y - (row * Y_INCREMENT);

        SwapperController result = Instantiate(swapperPrefab);

        result.transform.position = new Vector3(this.colZeroXPos+(this.colWidth*column), yPos, 0);
        return result;
    }

    private TerminatorController constructTerminator(int column, int row) {
        float yPos = INITIAL_Y - (row * Y_INCREMENT);

        TerminatorController result = Instantiate(terminatorPrefab);

        result.transform.position = new Vector3(this.colZeroXPos+(this.colWidth*column), yPos, 0);
        return result;
    }

    private ConnectorController constructConnector(Connector modelInput, int column, int row) {
        float yPos = INITIAL_Y - (row * Y_INCREMENT);

        ConnectorController result;
        if (modelInput.getOutputs().Count == 1) {
            result = Instantiate(this.connectorOneOutputPrefab);
        }
        else if (modelInput.getOutputs().Count == 2) {
            result = Instantiate(this.connectorTwoOutputPrefab);
        }
        else {
            throw new Exception("Invalid number of outputs for Connector");
        }

        result.transform.position = new Vector3(this.colZeroXPos+(this.colWidth*column), yPos, 0);
        return result;
    }

    private void constructInputViews(BoardObject[,] board) {
        for (int i = 0 ; i < board.GetLength(0) ; i++) {
            for (int j = 0 ; j < board.GetLength(1) ; j++) {
                if (board[i,j] != null) {
                    constructBoardObjectView(board[i,j], i, j);
                }
            }
        }
    }

    private void constructBoardObjectView(BoardObject modelInput, int column, int row) {
        if (modelInput is Wire) {
            this.boardViews[column, row] = constructWire(column, row);
            return;
        }

        if (modelInput is Connector) {
            ConnectorController boardObject;

            List<BoardObject> modelOutputs = modelInput.getOutputs();
            if (modelOutputs.Count == 1) {
                this.boardViews[column, row] = constructConnector((Connector)modelInput, column, row);
            }
            else if (modelOutputs.Count == 2) {
                this.boardViews[column, row] = constructConnector((Connector)modelInput, column, row);
            }
            else {
                throw new Exception("Unexpected number of outputs in Connector");
            }

            return;
        }

        if (modelInput is Initiator) {
            this.boardViews[column, row] = constructInitiator(column, row);
            return;
        }

        if (modelInput is Swapper) {
            this.boardViews[column, row] = constructSwapper(column, row);
            return;
        }

        if (modelInput is Terminator) {
            this.boardViews[column, row] = constructTerminator(column, row);
            return;
        }

        throw new Exception("Unexpected model object type");
    }

    public List<TargetController> getTargets()
    {
        return this.targets;
    }

    public AbstractBoardObjectController[,] getBoard()
    {
        return this.boardViews;
    }

    public PlayerController createPlayer()
    {
        this.player = Instantiate<PlayerController>(playerPrefab); 
        return this.player;
    }

    public List<PlayerController> createLives(int numberOfLives)
    {
        float posY = LIVES_INITIAL_Y;

        for (int i = 1 ; i < numberOfLives ; i++) {
            PlayerController player = Instantiate<PlayerController>(playerPrefab);
            player.transform.position = new Vector3(LIVES_X, posY, 0);
            posY = posY - player.transform.localScale.y;
            player.enabled = false;
            playerLives.Add(player);
        }

        return this.playerLives;
    }

    public void onPlayerMoved(int position)
    {
        this.player.onPlayerMoved(position);
    }

    public void onBoardObjectActivated(AbstractBoardObjectController boardObjectController)
    {
        boardObjectController.onActivated();
    }

    public void onZapFired()
    {
        // move the player one space to the right, 
        Transform t = this.player.transform;
        t.position += new Vector3(t.localScale.x, 0, 0);

        // disable it's script.
        this.player.enabled = false;

        // move it to the list of old players
        oldPlayers.Add(this.player);

        // take the last player object from the Lives list and 'reset' it to be at the initial player position (first wire).
        if (this.playerLives.Count > 0) {
            this.player = this.playerLives[this.playerLives.Count-1];
            this.playerLives.RemoveAt(this.playerLives.Count-1);
            this.player.reset();

            // then enable it's script.
            this.player.enabled = true;
        }
    }
}