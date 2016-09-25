using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// extends MonoBehaviour so that we can wire it into the Application object in Unity
public class View : MonoBehaviour
{
    private const float INITIAL_Y = 3f;
    private const float Y_INCREMENT = 0.60f;

    private const float PLAYER_1_LIVES_X = -8f;
    private const float PLAYER_2_LIVES_X = 8f;
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
    public EnemyController enemyPrefab;
    public TimeLeftController timeLeftText;
    public Text gameOverText;

    private List<TargetController> targets = new List<TargetController>();
    private TargetSummaryController targetSummary;
    private AbstractBoardObjectController[,] player1BoardViews = new AbstractBoardObjectController[3, ROW_COUNT];
    private AbstractBoardObjectController[,] player2BoardViews = new AbstractBoardObjectController[3, ROW_COUNT];

    private ZapController player1;
    private ZapController player2;
    private List<ZapController> player1Lives = new List<ZapController>();
    private List<ZapController> player2Lives = new List<ZapController>();
    private Dictionary<PlayerNumber, Dictionary<int, ZapController>> oldPlayers = new Dictionary<PlayerNumber, Dictionary<int, ZapController>>();

    private float player1ColZeroXPos;
    private float player2ColZeroXPos;
    private float colWidth;

    public void init(BoardObject[,] player1BoardModel, BoardObject[,] player2BoardModel) {
        this.player1ColZeroXPos = targetPrefab.transform.position.x - (targetPrefab.transform.localScale.x/2)
            - (wirePrefab.transform.localScale.x/2) - (wirePrefab.transform.localScale.x * 2);

        this.player2ColZeroXPos = targetPrefab.transform.position.x + (targetPrefab.transform.localScale.x/2)
            + (wirePrefab.transform.localScale.x/2) + (wirePrefab.transform.localScale.x * 2);

        this.colWidth = wirePrefab.transform.localScale.x;

        constructTargetViews();
        constructTargetSummary();
        constructInputViews(player1BoardModel, player1BoardViews, PlayerNumber.PLAYER1);
        constructInputViews(player2BoardModel, player2BoardViews, PlayerNumber.PLAYER2);

        oldPlayers.Add(PlayerNumber.PLAYER1, new Dictionary<int, ZapController>());
        oldPlayers.Add(PlayerNumber.PLAYER2, new Dictionary<int, ZapController>());

        timeLeftText.gameOver += onGameOver;
    }

    private void onGameOver(TimeLeftController sender, EventArgs e)
    {
        PlayerNumber winner = this.targetSummary.getWinner();

        this.gameOverText.text = "Game Over.\n";
        if (PlayerNumber.PLAYER1 == winner) {
            this.gameOverText.text = this.gameOverText.text + "You Win!";
        }
        else if (PlayerNumber.PLAYER2 == winner) {
            this.gameOverText.text = this.gameOverText.text + "You Lose!";
        }
        else {
            this.gameOverText.text = this.gameOverText.text + "Draw!";
        }

        this.gameOverText.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    private void constructTargetSummary() {
        // Initially, the target summary should be black
        this.targetSummary = Instantiate(targetSummaryPrefab);
        float gapBetweenTargets = Y_INCREMENT - this.targets[0].transform.localScale.y;
        float posY = INITIAL_Y + this.targets[0].transform.localScale.y/2 + this.targetSummary.transform.localScale.y/2 + gapBetweenTargets;
        this.targetSummary.transform.position = new Vector3(0, posY, 0);
    }

    private void constructTargetViews() {
        // TODO: Initially, the targets should alternate between yellow and blue
        float yPos = INITIAL_Y;

        for (int index = 0 ; index < ROW_COUNT ; index++) {
            TargetController newTarget = Instantiate(targetPrefab);
            newTarget.transform.position = new Vector3(0, yPos, 0);

            this.targets.Add(newTarget);

            yPos = yPos - Y_INCREMENT;
        }
    }

    private float computeXPos(int column, PlayerNumber playerNumber) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            return this.player1ColZeroXPos+(this.colWidth*column);
        }

        return this.player2ColZeroXPos-(this.colWidth*column);
    }

    private Vector3 computeRotation(PlayerNumber playerNumber) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            return new Vector3(0, 0, 0);
        }

        return new Vector3(0, 0, 180);
    }

    // TODO: See if these methods can be collapsed into a single one
    private WireController constructWire(int column, int row, PlayerNumber playerNumber) {
        float yPos = INITIAL_Y - (row * Y_INCREMENT);

        WireController result = Instantiate(wirePrefab);

        result.setOwner(playerNumber);
        result.transform.position = new Vector3(computeXPos(column, playerNumber), yPos, 0);
        result.transform.Rotate(computeRotation(playerNumber));
        return result;
    }

    private InitiatorController constructInitiator(int column, int row, PlayerNumber playerNumber) {
        float yPos = INITIAL_Y - (row * Y_INCREMENT);

        InitiatorController result = Instantiate(initiatorPrefab);

        result.setOwner(playerNumber);
        result.transform.position = new Vector3(computeXPos(column, playerNumber), yPos, 0);
        result.transform.Rotate(computeRotation(playerNumber));
        return result;
    }

    private SwapperController constructSwapper(int column, int row, PlayerNumber playerNumber) {
        float yPos = INITIAL_Y - (row * Y_INCREMENT);

        SwapperController result = Instantiate(swapperPrefab);

        result.setOwner(playerNumber);
        result.transform.position = new Vector3(computeXPos(column, playerNumber), yPos, 0);
        result.transform.Rotate(computeRotation(playerNumber));
        return result;
    }

    private TerminatorController constructTerminator(int column, int row, PlayerNumber playerNumber) {
        float yPos = INITIAL_Y - (row * Y_INCREMENT);

        TerminatorController result = Instantiate(terminatorPrefab);

        result.setOwner(playerNumber);
        result.transform.position = new Vector3(computeXPos(column, playerNumber), yPos, 0);
        result.transform.Rotate(computeRotation(playerNumber));
        return result;
    }

    private ConnectorController constructConnector(Connector modelInput, int column, int row, PlayerNumber playerNumber) {
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

        result.setOwner(playerNumber);
        result.transform.position = new Vector3(computeXPos(column, playerNumber), yPos, 0);
        result.transform.Rotate(computeRotation(playerNumber));
        return result;
    }

    private void constructInputViews(BoardObject[,] boardModel, AbstractBoardObjectController[,] boardViews, PlayerNumber playerNumber) {
        for (int i = 0 ; i < boardModel.GetLength(0) ; i++) {
            for (int j = 0 ; j < boardModel.GetLength(1) ; j++) {
                if (boardModel[i,j] != null) {
                    constructBoardObjectView(boardViews, boardModel[i,j], i, j, playerNumber);
                }
            }
        }
    }

    private void constructBoardObjectView(AbstractBoardObjectController[,] boardViews, BoardObject modelInput, int column, int row, 
        PlayerNumber playerNumber) {

        if (modelInput is Wire) {
            boardViews[column, row] = constructWire(column, row, playerNumber);
            return;
        }

        if (modelInput is Connector) {
            List<BoardObject> modelOutputs = modelInput.getOutputs();
            if (modelOutputs.Count == 1) {
                boardViews[column, row] = constructConnector((Connector)modelInput, column, row, playerNumber);
            }
            else if (modelOutputs.Count == 2) {
                boardViews[column, row] = constructConnector((Connector)modelInput, column, row, playerNumber);
            }
            else {
                throw new Exception("Unexpected number of outputs in Connector");
            }

            return;
        }

        if (modelInput is Initiator) {
            boardViews[column, row] = constructInitiator(column, row, playerNumber);
            return;
        }

        if (modelInput is Swapper) {
            boardViews[column, row] = constructSwapper(column, row, playerNumber);
            return;
        }

        if (modelInput is Terminator) {
            boardViews[column, row] = constructTerminator(column, row, playerNumber);
            return;
        }

        throw new Exception("Unexpected model object type");
    }

    public List<TargetController> getTargets()
    {
        return this.targets;
    }

    public AbstractBoardObjectController[,] getPlayer1Board()
    {
        return this.player1BoardViews;
    }

    public AbstractBoardObjectController[,] getPlayer2Board()
    {
        return this.player2BoardViews;
    }

    public ZapController createPlayer1()
    {
        this.player1 = Instantiate<PlayerController>(playerPrefab);
        return this.player1;
    }

    public ZapController createPlayer2()
    {
        EnemyController player = Instantiate<EnemyController>(enemyPrefab);
        player.setBoard(this.player2BoardViews); 
        this.player2 = player;
        return player;
    }

    public List<ZapController> createPlayerLives(List<ZapController> playerLives, ZapController prefab, float livesXPos, 
        int numberOfLives)
    {
        float posY = LIVES_INITIAL_Y;

        for (int i = 1 ; i < numberOfLives ; i++) {
            ZapController player = Instantiate<ZapController>(prefab);
            player.transform.position = new Vector3(livesXPos, posY, 0);
            posY = posY - player.transform.localScale.y;
            player.enabled = false;
            playerLives.Add(player);
        }

        return playerLives;
    }

    public List<ZapController> createPlayer1Lives(int numberOfLives) {
        return createPlayerLives(this.player1Lives, playerPrefab, PLAYER_1_LIVES_X, numberOfLives);
    }

    public List<ZapController> createPlayer2Lives(int numberOfLives) {
        List<ZapController> players = createPlayerLives(this.player2Lives, enemyPrefab, PLAYER_2_LIVES_X, numberOfLives);

        for (int i = 0 ; i < players.Count ; i++) {
            ((EnemyController)players[i]).setBoard(this.player2BoardViews);
        }

        return players;
    }

    private ZapController getPlayerForPlayerNumber(PlayerNumber playerNumber) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            return this.player1;
        }

        return this.player2;
    }
        
    public void onPlayerMoved(PlayerNumber playerNumber, int position) {
        ZapController player = getPlayerForPlayerNumber(playerNumber);
        player.onPlayerMoved(position);
    }

    public void onBoardObjectActivated(AbstractBoardObjectController boardObjectController, PlayerNumber playerNumber) {
        boardObjectController.onActivated(playerNumber);
    }

    public void onBoardObjectDeactivated(AbstractBoardObjectController boardObjectController) {
        boardObjectController.onDeactivated();
    }

    private List<ZapController> getPlayerLivesForPlayerNumber(PlayerNumber playerNumber) {
        if (PlayerNumber.PLAYER1 == playerNumber) {
            return this.player1Lives;
        }

        return this.player2Lives;
    }

    private Vector3 getMovementForPlayerNumber(PlayerNumber playerNumber) {
        ZapController player = getPlayerForPlayerNumber(playerNumber);
        Transform t = player.transform;

        if (PlayerNumber.PLAYER1 == playerNumber) {
            return new Vector3(t.localScale.x, 0, 0);
        }

        return new Vector3(-t.localScale.x, 0, 0);
    }

    public void onZapFired(PlayerNumber playerNumber)
    {
        ZapController player = getPlayerForPlayerNumber(playerNumber);
        List<ZapController> playerLives = getPlayerLivesForPlayerNumber(playerNumber);

        // move the player one space to the right, (or to the left, for player2)
        Transform t = player.transform;
        t.position += getMovementForPlayerNumber(playerNumber);

        // disable it's script.
        player.enabled = false;

        // move it to the list of old players
        this.oldPlayers[playerNumber].Add(player.getPlayerPosition(), player);

        // take the last player object from the Lives list and 'reset' it to be at the initial player position (first wire).
        if (playerLives.Count > 0) {
            player = playerLives[playerLives.Count-1];
            playerLives.RemoveAt(playerLives.Count-1);
            player.reset();

            if (PlayerNumber.PLAYER1 == playerNumber) {
                this.player1 = player;
            }
            else {
                this.player2 = player;
            }

            // then enable it's script.
            player.enabled = true;
        }
    }

    public void onZapExpired(PlayerNumber playerNumber, int playerPosition) {
        // TODO: Make this work for both players

        // Find the entry in oldPlayers that corresponds to the specified playerPosition, then set it inactive.
        Dictionary<int, ZapController> oldPlayerList = this.oldPlayers[playerNumber];
        oldPlayerList[playerPosition].setActive(false);
        oldPlayerList.Remove(playerPosition);
    }

    public void onTargetSummaryUpdated(PlayerNumber playerNumber)
    {
        this.targetSummary.onUpdated(playerNumber);
    }

}