using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void GameStartEventHandler(View sender, GameStartEventArgs e);

public class GameStartEventArgs : EventArgs {

    public Side player1Side;

    public GameStartEventArgs(Side player1Side) {
        this.player1Side = player1Side;
    }
}

public class View : MonoBehaviour
{
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
    public WinLoseController winLoseController;

    public event GameStartEventHandler gameStart;

    private List<TargetController> targets = new List<TargetController>();
    private TargetSummaryController targetSummary;
    private AbstractBoardObjectController[,] leftBoardViews = new AbstractBoardObjectController[3, ROW_COUNT];
    private AbstractBoardObjectController[,] rightBoardViews = new AbstractBoardObjectController[3, ROW_COUNT];

    private ZapController player1;
    private ZapController player2;
    private List<ZapController> player1Lives = new List<ZapController>();
    private List<ZapController> player2Lives = new List<ZapController>();
    private Dictionary<PlayerNumber, Dictionary<int, ZapController>> oldPlayers = new Dictionary<PlayerNumber, Dictionary<int, ZapController>>();

    private float player1ColZeroXPos;
    private float player2ColZeroXPos;
    private float colWidth;

    private Side player1Side = Side.LEFT;
    private Side player2Side = Side.RIGHT;

    public void init(BoardObject[,] leftBoardModel, BoardObject[,] rightBoardModel) {
        this.player1ColZeroXPos = targetPrefab.transform.position.x - (targetPrefab.transform.localScale.x/2)
            - (wirePrefab.transform.localScale.x/2) - (wirePrefab.transform.localScale.x * 2);

        this.player2ColZeroXPos = targetPrefab.transform.position.x + (targetPrefab.transform.localScale.x/2)
            + (wirePrefab.transform.localScale.x/2) + (wirePrefab.transform.localScale.x * 2);

        this.colWidth = wirePrefab.transform.localScale.x;

        constructTargetViews();
        constructTargetSummary();
        constructInputViews(leftBoardModel, leftBoardViews, PlayerNumber.PLAYER1);
        constructInputViews(rightBoardModel, rightBoardViews, PlayerNumber.PLAYER2);

        oldPlayers.Add(PlayerNumber.PLAYER1, new Dictionary<int, ZapController>());
        oldPlayers.Add(PlayerNumber.PLAYER2, new Dictionary<int, ZapController>());

        timeLeftText.gamePhaseChange += onGamePhaseChange;
    }

    private void onGameStart(GameStartEventArgs eventArgs) {
        Debug.Log("onGameStart: " + eventArgs);

        if (gameStart != null)
            gameStart(this, eventArgs);
    }

    private void onGamePhaseChange(TimeLeftController sender, GamePhaseChangeEventArgs e)
    {
        if (e.gamePhase == GamePhase.MAIN_GAME) {
            onGameStart(new GameStartEventArgs(this.player1Side));

            AbstractBoardObjectController[,] enemyBoardViews;
            if (this.player1Side == Side.LEFT) {
                enemyBoardViews = this.rightBoardViews;
            }
            else {
                enemyBoardViews = this.leftBoardViews;
            }

            // set the board on the enemy players
            ((EnemyController)this.player2).setBoard(enemyBoardViews); 
            for (int i = 0 ; i < this.player2Lives.Count ; i++) {
                ((EnemyController)this.player2Lives[i]).setBoard(enemyBoardViews);
            }
        }

        this.player1.onGamePhaseChange(e.gamePhase);
        this.player2.onGamePhaseChange(e.gamePhase);

        for (int i = 0 ; i < this.player1Lives.Count ; i++) {
            this.player1Lives[i].onGamePhaseChange(e.gamePhase);
        }

        for (int i = 0 ; i < this.player2Lives.Count ; i++) {
            this.player2Lives[i].onGamePhaseChange(e.gamePhase);
        }

        if (e.gamePhase == GamePhase.GAME_OVER) {
            PlayerNumber winner = this.targetSummary.getWinner();
            this.winLoseController.activate(winner);
        }
    }

    private void constructTargetSummary() {
        // Initially, the target summary should be black
        this.targetSummary = Instantiate(targetSummaryPrefab);
        float gapBetweenTargets = ViewConstants.Y_INCREMENT - this.targets[0].transform.localScale.y;
        float posY = ViewConstants.INITIAL_Y + this.targets[0].transform.localScale.y/2 + this.targetSummary.transform.localScale.y/2 + gapBetweenTargets*2;
        this.targetSummary.transform.position = new Vector3(0, posY, 0);
    }

    private void constructTargetViews() {
        float yPos = ViewConstants.INITIAL_Y;

        for (int index = 0 ; index < ROW_COUNT ; index++) {
            TargetController newTarget = Instantiate(targetPrefab);
            newTarget.transform.position = new Vector3(0, yPos, 0);

            this.targets.Add(newTarget);

            yPos = yPos - ViewConstants.Y_INCREMENT;
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

    private T constructGameObject<T>(int column, int row, PlayerNumber playerNumber, T prefab) where T:AbstractBoardObjectController {
        float yPos = ViewConstants.INITIAL_Y - (row * ViewConstants.Y_INCREMENT);

        T result = Instantiate(prefab);

        result.setOwner(playerNumber);
        result.transform.position = new Vector3(computeXPos(column, playerNumber), yPos, 0);
        result.transform.Rotate(computeRotation(playerNumber));
        return result;
    }

    private WireController constructWire(int column, int row, PlayerNumber playerNumber) {
        return constructGameObject(column, row, playerNumber, wirePrefab);
    }

    private InitiatorController constructInitiator(int column, int row, PlayerNumber playerNumber) {
        return constructGameObject(column, row, playerNumber, initiatorPrefab);
    }

    private SwapperController constructSwapper(int column, int row, PlayerNumber playerNumber) {
        return constructGameObject(column, row, playerNumber, swapperPrefab);
    }

    private TerminatorController constructTerminator(int column, int row, PlayerNumber playerNumber) {
        return constructGameObject(column, row, playerNumber, terminatorPrefab);
    }

    private ConnectorController constructConnector(Connector modelInput, int column, int row, PlayerNumber playerNumber) {
        if (modelInput.getOutputs().Count == 1) {
            return constructGameObject(column, row, playerNumber, this.connectorOneOutputPrefab);
        }
        else if (modelInput.getOutputs().Count == 2) {
            return constructGameObject(column, row, playerNumber, this.connectorTwoOutputPrefab);
        }
        else {
            throw new Exception("Invalid number of outputs for Connector");
        }
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

    public AbstractBoardObjectController[,] getLeftBoard()
    {
        return this.leftBoardViews;
    }

    public AbstractBoardObjectController[,] getRightBoard()
    {
        return this.rightBoardViews;
    }

    // TODO: Refactor these - create the players in 'init', and turn these into getter methods
    public ZapController createPlayer1()
    {
        this.player1 = Instantiate<PlayerController>(playerPrefab);
        return this.player1;
    }

    public ZapController createPlayer2()
    {
        this.player2 = Instantiate<EnemyController>(enemyPrefab);
        return this.player2;
    }

    private List<ZapController> createPlayerLives(List<ZapController> playerLives, ZapController prefab, float livesXPos, 
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
        return createPlayerLives(this.player2Lives, enemyPrefab, PLAYER_2_LIVES_X, numberOfLives);
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
        boardObjectController.onActivated(playerNumber, this.player1Side);
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

        float x;
        if (PlayerNumber.PLAYER1 == playerNumber) {
            if (Side.LEFT == this.player1Side) {
                x = t.localScale.x;
            }
            else {
                x = -t.localScale.x;
            }
        }
        else {
            if (Side.LEFT == this.player1Side) {
                x = -t.localScale.x;
            }
            else {
                x = t.localScale.x;
            }
        }

        return new Vector3(x, 0, 0);
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
        // Find the entry in oldPlayers that corresponds to the specified playerPosition, then set it inactive.
        Dictionary<int, ZapController> oldPlayerList = this.oldPlayers[playerNumber];
        oldPlayerList[playerPosition].setActive(false);
        oldPlayerList.Remove(playerPosition);
    }

    public void onTargetSummaryUpdated(PlayerNumber playerNumber)
    {
        this.targetSummary.onUpdated(playerNumber, this.player1Side);
    }

    public void onSideChanged(Side side)
    {
        this.player2Side = this.player1Side;
        this.player1Side = side;

        this.player1.onSideChanged(this.player1Side);
        this.player2.onSideChanged(this.player2Side);

        for (int i = 0 ; i < this.player1Lives.Count ; i++) {
            this.player1Lives[i].onSideChanged(this.player1Side);
        }

        for (int i = 0 ; i < this.player2Lives.Count ; i++) {
            this.player2Lives[i].onSideChanged(this.player2Side);
        }


    }
}