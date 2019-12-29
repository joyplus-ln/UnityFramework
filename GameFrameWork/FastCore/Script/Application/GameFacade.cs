using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFacade : MonoBehaviour
{
    private GameManagers _gameManagers = new GameManagers();
    private GameSystems _gameSystems = new GameSystems();
    private Action GameStarted = null;
    private static GameFacade _facade = null;
    public static GameFacade gameFacade
    {
        get
        {
            if(_facade == null)
            {
                _facade = FindObjectOfType<GameFacade>();
            }

            return _facade;
        }

    }
    
    /// <summary>
    /// game的唯一启动入口
    /// </summary>
    public void StartGame(Action GameStarted)
    {
        this.GameStarted = GameStarted;
        InitManagers();
    }

    /// <summary>
    /// 初始化相关managers
    /// </summary>
    private void InitManagers()
    {
        _gameManagers.Init(InitSystems);
    }

    /// <summary>
    /// 初始化各个系统
    /// </summary>
    private void InitSystems()
    {
        _gameSystems.Init(GameStartFinished);
    }

    /// <summary>
    /// 启动成功
    /// </summary>
    private void GameStartFinished()
    {
        GameStarted?.Invoke();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        _gameManagers.GamePause(pauseStatus);
        _gameSystems.GamePause(pauseStatus);
    }

    private void OnApplicationQuit()
    {
        _gameManagers.ExitGame();
        _gameSystems.ExitGame();
    }

    private void Update()
    {
        _gameManagers.Update();
        _gameSystems.Update();
    }
}