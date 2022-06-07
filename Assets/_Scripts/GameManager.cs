using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private Entity _player;
    public PlayerStatInterface statText;
    public InventoryInterface inventoryInterface;
    private GameState _gameState;
    private MessageLog _log;
    private int _currentActorId = 0;
    private Action _deferredAction;

    [Header("Entites")]
    private EntityMap _entityMap;
    private EntityMap _entityMapBackground;


    [Header("Floor")]
    private GroundMap _groundMap;

    public LevelDataScriptableObject levelData;

    [Header("Systems")]
    private FieldOfViewSystem fovSystem;

    [Header("Settings")]
    public float cameraAdjustmentPercent = 0.793f;
    public float viewportWidth = 10f;
    public int playerViewDistance = 10;

    private List<Actor> _actors;

    void Start()
    {
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;

        var levelBuilder = new LevelBuilder();
        levelBuilder.Generate(levelData);
        _groundMap = levelBuilder.GetGroundMap();
        _entityMap = levelBuilder.GetEntityMap();
        _entityMapBackground = levelBuilder.GetPassiveEntityMap();
        _actors = levelBuilder.GetActors();

        var startLocation = levelBuilder.GetStartPosition();
        // Build Player
        _player = Entity.CreateEntity().Init(startLocation.Clone(), spriteType: SpriteType.Soldier_Sword, color: Color.green, name: "player", blocks: true);
        _player.gameObject.AddComponent<Player>().owner = _player;
        _player.gameObject.AddComponent<Fighter>().Init(30, 2, 5).owner = _player;
        _player.gameObject.AddComponent<Inventory>().Init(capacity: 10).owner = _player;
        _actors.Add(new Actor(_player));
        _entityMap.AddEntity(_player);

        SetDesiredScreenSize();
        Camera.main.transform.position = new Vector3(_player.position.x + CalculateCameraAdjustment(), _player.position.y, Camera.main.transform.position.z);

        // Setup Systems
        fovSystem = new FieldOfViewSystem(_groundMap);
        fovSystem.Run(new Vector2Int(_player.position.x, _player.position.y), playerViewDistance);

        RunVisibilitySystem();

        // Final Setup
        _groundMap.UpdateTiles();

        statText.SetPlayer(_player);
        inventoryInterface.SetInventory(_player.GetComponent<Inventory>());
        _gameState = GameState.Global_LevelScene;

        _log = FindObjectOfType<MessageLog>();
    }

    void Update()
    {
        // Handle User Input (yes we're doing this elsewhere too, plan on fixing that)
        HandleUserInput();

        var turnResults = ProcessTurn();
        ProcessTurnResults(turnResults);

    }

    ActionResult ProcessTurn()
    {
        // The deferred action exists because something in the main loop needs to happen.
        if( _deferredAction != null ) { return new ActionResult(); }

        ActionResult actionResult;
        var actor = _actors.ElementAt(_currentActorId);
        var action = actor.GetAction(GetMapDTO());
        var actionToTake = action;
        if (action == null) { return new ActionResult(); }

        do
        {
            actionResult = actionToTake.PerformAction(GetMapDTO());

            // Cleanup to handle after player potentially changes position
            Camera.main.transform.position = new Vector3(_player.position.x + CalculateCameraAdjustment(), _player.position.y, Camera.main.transform.position.z);

            if( actionResult.status == ActionResultType.Success || actionResult.status == ActionResultType.TurnDeferred ){
                TransitionFrom(_gameState);
                TransitionTo(actionResult.TransitionToStateOnSuccess);
            }

            actionToTake = actionResult.NextAction;
        }
        while (actionResult.NextAction != null && actionResult.status != ActionResultType.TurnDeferred);


        if (actionResult.status == ActionResultType.Success)
        {
            _currentActorId = (_currentActorId + 1) % _actors.Count();
            if (actor.entity == _player)
            {
                fovSystem.Run(new Vector2Int(_player.position.x, _player.position.y), playerViewDistance);
                _groundMap.UpdateTiles();
            }
        }

        if( actionResult.status == ActionResultType.TurnDeferred)
        {
            _deferredAction = actionToTake;
        }

        ProcessNewState();

        return actionResult;
    }

    void ProcessTurnResults(ActionResult results)
    {
        foreach (var message in results.GetMessages()) { _log.AddMessage(message); }
        var deadEntities = results.GetEntityEvent("dead");
        if (deadEntities.Count() > 0)
        {
            var actionResult = new ActionResult();
            foreach (var dead in deadEntities)
            {
                if (dead == _player)
                {
                    actionResult.Append(dead.ConvertToDeadPlayer());
                    _gameState = GameState.Global_PlayerDead;
                }
                else
                {
                    actionResult.Append(dead.ConvertToDeadMonster());
                }

                _entityMap.SwapEntityToMap(dead, _entityMapBackground);
                _actors.Remove(dead.actor);
            }
            foreach (var message in actionResult.GetMessages()) { _log.AddMessage(message); }
        }
    }

    void ProcessNewState()
    {
        RunVisibilitySystem();
    }

    public MapDTO GetMapDTO()
    {
        return new MapDTO { EntityMap = _entityMap, EntityFloorMap = _entityMapBackground, GroundMap = _groundMap };
    }

    void RunVisibilitySystem()
    {
        _entityMapBackground.RenderAll();
        _entityMap.RenderAll();
    }

    private void ReportObjectsAtPosition(CellPosition pos)
    {
        var entityNames = _entityMap.GetEntities().Where(e => e.position == pos).Select(e => e.GetColoredName());
        var backgroundNames = _entityMapBackground.GetEntities().Where(e => e.position == pos).Select(e => e.GetColoredName());

        var entitiesToLog = entityNames.Concat(backgroundNames);
        var message = "There is nothing there.";

        if (entitiesToLog.Count() > 0)
        {
            var names = string.Join(", ", entitiesToLog);
            message = $"You see: {names}";
        }

        _log.AddMessage(new Message(message, null));
    }

    void SetMoveDirection(Vector2Int direction)
    {
        CellPosition newPosition = new CellPosition(_player.position.x + direction.x, _player.position.y + direction.y);
        var action = new WalkAction(_player.actor, new TargetData { targetPosition = newPosition });
        _player.actor.SetNextAction(action);
    }

    void HandleUserInput()
    {
        if (_gameState == GameState.Global_LevelScene)
        {
            // Look Action
            if (Input.GetMouseButtonDown(0))
            {
                ReportObjectsAtPosition(MouseUtilities.GetCellPositionAtMousePosition(_groundMap));
            }

            HandleMovementKeys();

            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                var action = new WaitAction(_player.actor);
                _player.actor.SetNextAction(action);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                // Pickup!
                var action = new PickupItemAction(_player.actor);
                _player.actor.SetNextAction(action);
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                TransitionFrom(_gameState);
                TransitionTo(GameState.Global_InventoryMenu);

                // Open inventory of player
                _log.AddMessage(new Message($"{_player.actor.entity.GetColoredName()} opens their inventory...", null));
                var result = inventoryInterface.DescribeInventory();
                ProcessTurnResults(result);
            }
        }

        else if (_gameState == GameState.Global_InventoryMenu )
        {
            if (Input.GetKeyDown(KeyCode.I))
            { 
                TransitionFrom(_gameState);
                TransitionTo(GameState.Global_LevelScene);
            }
            else
            {
                inventoryInterface.HandleItemKeyPress();
            }
        }
        else if ( _gameState == GameState.Global_ActionHandlerDeferred)
        {
            if(_deferredAction.UpdateHandler(GetMapDTO()))
            {
                var actor = _actors.ElementAt(_currentActorId);
                actor.SetNextAction(_deferredAction);
                _deferredAction = null;
            }
        }

    }

    public void TransitionTo(GameState gameState) {
        switch (gameState)
        {
            case GameState.Global_LevelScene:
                _gameState = gameState;
                break;
            case GameState.Global_InventoryMenu:
                _gameState = gameState;
                inventoryInterface.Show();
                break;
            case GameState.Global_ActionHandlerDeferred:
                _gameState = gameState;
                break;
            default:
                return;
        }
    }

    public void TransitionFrom(GameState gameState){
        if( gameState == GameState.Global_InventoryMenu ){
            inventoryInterface.Hide();
        }
    }

    void HandleMovementKeys()
    {
        Vector2Int direction = Vector2Int.zero;
        // Cardinals
        if (Input.GetKeyDown(KeyCode.Keypad4))
            direction.x = -1;
        if (Input.GetKeyDown(KeyCode.Keypad6))
            direction.x = 1;
        if (Input.GetKeyDown(KeyCode.Keypad2))
            direction.y = -1;
        if (Input.GetKeyDown(KeyCode.Keypad8))
            direction.y = 1;

        // Diagonals
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            direction.x = -1; direction.y = 1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            direction.x = -1; direction.y = -1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            direction.x = 1; direction.y = -1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            direction.x = 1; direction.y = 1;
        }

        if (direction.x != 0 || direction.y != 0)
        {
            SetMoveDirection(direction);
        }
    }

    void SetDesiredScreenSize()
    {
        float aspect = (float)Screen.width / (float)Screen.height;
        var totalWidth = 1f + (float)(viewportWidth * 2f / cameraAdjustmentPercent);
        var screenSize = (float)totalWidth / 2f / aspect;

        Camera.main.orthographicSize = screenSize;
    }

    float CalculateCameraAdjustment()
    {
        var value = 0f;

        float aspect = (float)Screen.width / (float)Screen.height;
        var totalWidth = (float)viewportWidth * 2f / cameraAdjustmentPercent;
        // var screenSize = totalWidth / 2 / aspect;
        value = totalWidth - (viewportWidth * 2f);

        return value / 2;
    }
}
