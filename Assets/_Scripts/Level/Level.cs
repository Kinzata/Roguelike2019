using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level
{
    // Maps
    private EntityMap _entityMap;
    private EntityMap _entityMapBackground;
    private GroundMap _groundMap;
    private MiscMap _miscMap;

    // Actors
    private List<Actor> _actors;
    private Entity _player;

    public LevelDataScriptableObject levelData;

    public void Update()
    {
        // Handle User Input (yes we're doing this elsewhere too, plan on fixing that)

    }

    public Level BuildLevel(LevelDataScriptableObject levelData)
    {
        var levelBuilder = new LevelBuilder();
        this.levelData = levelData;
        levelBuilder.Generate(levelData);

        _groundMap = levelBuilder.GetGroundMap();
        _entityMap = levelBuilder.GetEntityMap();
        _entityMapBackground = levelBuilder.GetPassiveEntityMap();
        _miscMap = levelBuilder.GetMiscMap();
        _actors = levelBuilder.GetActors();

        RunVisibilitySystem();

        return this;
    }

    public void FinalSetup()
    {
        RunVisibilitySystem();
        OnTurnSuccess();
    }

    public MapDTO GetMapDTO()
    {
        return new MapDTO { EntityMap = _entityMap, EntityFloorMap = _entityMapBackground, GroundMap = _groundMap, MiscMap = _miscMap };
    }

    public List<Actor> GetActors()
    {
        return _actors;
    }

    public void OnTurnSuccess()
    {
        _groundMap.UpdateTiles();
    }

    public void SetPlayer(Entity player)
    {
        _player = player;
        _actors.Add(new Actor(_player));
        _entityMap.AddEntity(_player);
    }

    public Entity GetPlayer()
    {
        return _player;
    }

    public CellPosition GetEntryPosition()
    {
        return _groundMap.rooms.First().center;
    }

    public void RunVisibilitySystem()
    {
        _entityMapBackground.RenderAll();
        _entityMap.RenderAll();
    }

    public ActionResult HandleDeadEntities(Entity[] deadEntities)
    {
        var actionResult = new ActionResult();
        foreach (var dead in deadEntities)
        {
            if (dead == _player)
            {
                actionResult.Append(dead.ConvertToDeadPlayer());
                actionResult.TransitionToStateOnSuccess = GameState.Global_PlayerDead;
            }
            else
            {
                actionResult.Append(dead.ConvertToDeadMonster());
            }

            _entityMap.SwapEntityToMap(dead, _entityMapBackground);
            _actors.Remove(dead.actor);
        }
        return actionResult;
        
    }
}
