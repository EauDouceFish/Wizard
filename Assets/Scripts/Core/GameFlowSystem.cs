/*using System.Collections;
using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

public class GameFlowSystem : AbstractSystem
{
    private GameCoreModel gameCoreModel;
    private MapModel mapModel;
    private GameState currentGameState;

    // 游戏流程控制器 - MonoBehaviour
    private GameFlowController gameFlowController;

    protected override void OnInit()
    {
        gameCoreModel = this.GetModel<GameCoreModel>();
        mapModel = this.GetModel<MapModel>();

        currentGameState = GameState.MainMenu;

        // 创建并初始化 GameFlowController
        CreateGameFlowController();

        // 注册事件监听
        RegisterEvents();
    }

    private void CreateGameFlowController()
    {
        GameObject controllerGO = new GameObject("GameFlowController");
        gameFlowController = controllerGO.AddComponent<GameFlowController>();
        gameFlowController.Initialize(this);
        Object.DontDestroyOnLoad(controllerGO);
    }

    private void RegisterEvents()
    {
        // 监听地图生成完成事件
        this.RegisterEvent<MapGenerationCompletedEvent>(OnMapGenerationCompleted);

        // 监听玩家初始化完成事件
        this.RegisterEvent<PlayerInitializedEvent>(OnPlayerInitialized);

        // 监听区域进入事件
        this.RegisterEvent<PlayerEnteredRegionEvent>(OnPlayerEnteredRegion);

        // 监听区域清理完成事件
        this.RegisterEvent<RegionClearedEvent>(OnRegionCleared);

        // 监听游戏胜利事件
        this.RegisterEvent<GameVictoryEvent>(OnGameVictory);

        // 监听游戏失败事件
        this.RegisterEvent<GameOverEvent>(OnGameOver);
    }

    #region 游戏状态控制

    public void StartNewGame(MapSize mapSize)
    {
        ChangeGameState(GameState.MapGeneration);

        // 执行地图生成命令
        this.SendCommand(new CreateMapCommand(mapSize));
    }

    public void ChangeGameState(GameState newState)
    {
        if (currentGameState == newState) return;

        Debug.Log($"游戏状态切换: {currentGameState} -> {newState}");

        var previousState = currentGameState;
        currentGameState = newState;

        // 根据新状态执行相应逻辑
        OnGameStateChanged(previousState, newState);

        // 发送状态变化事件
        this.SendEvent(new GameStateChangedEvent(previousState, newState));
    }

    private void OnGameStateChanged(GameState previousState, GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenuState();
                break;
            case GameState.MapGeneration:
                HandleMapGenerationState();
                break;
            case GameState.PlayerInit:
                HandlePlayerInitState();
                break;
            case GameState.GamePlaying:
                HandleGamePlayingState();
                break;
            case GameState.GameOver:
                HandleGameOverState();
                break;
        }
    }

    #endregion

    #region 状态处理方法

    private void HandleMainMenuState()
    {
        // 主菜单状态处理
        if (SceneManager.GetActiveScene().name != GameScene.MainMenuScene.ToString())
        {
            gameFlowController.StartCoroutine(LoadSceneAsync(GameScene.MainMenuScene));
        }
    }

    private void HandleMapGenerationState()
    {
        // 地图生成状态 - 等待地图生成完成
        Debug.Log("开始地图生成...");
    }

    private void HandlePlayerInitState()
    {
        // 玩家初始化状态
        gameFlowController.StartCoroutine(InitializePlayer());
    }

    private void HandleGamePlayingState()
    {
        // 游戏进行状态
        Debug.Log("游戏开始！");
        // 这里可以初始化游戏UI、音效等
    }

    private void HandleGameOverState()
    {
        // 游戏结束状态
        gameFlowController.StartCoroutine(ShowGameOverSequence());
    }

    #endregion

    #region 事件处理

    private void OnMapGenerationCompleted(MapGenerationCompletedEvent e)
    {
        Debug.Log("地图生成完成，准备初始化玩家");
        ChangeGameState(GameState.PlayerInit);
    }

    private void OnPlayerInitialized(PlayerInitializedEvent e)
    {
        Debug.Log("玩家初始化完成，游戏正式开始");
        ChangeGameState(GameState.GamePlaying);
    }

    private void OnPlayerEnteredRegion(PlayerEnteredRegionEvent e)
    {
        // 玩家进入新区域，生成怪物
        gameFlowController.StartCoroutine(HandleRegionEntry(e.region));
    }

    private void OnRegionCleared(RegionClearedEvent e)
    {
        // 区域清理完成，给予奖励
        gameFlowController.StartCoroutine(HandleRegionReward(e.region));
    }

    private void OnGameVictory(GameVictoryEvent e)
    {
        Debug.Log("游戏胜利！");
        ChangeGameState(GameState.GameOver);
    }

    private void OnGameOver(GameOverEvent e)
    {
        Debug.Log("游戏失败！");
        ChangeGameState(GameState.GameOver);
    }

    #endregion

    #region 协程方法

    private IEnumerator LoadSceneAsync(GameScene scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString());
        yield return operation;
    }

    private IEnumerator InitializePlayer()
    {
        // 等待场景加载完成
        yield return new WaitForSeconds(0.1f);

        // 查找起始区域
        var startRegion = FindStartRegion();
        if (startRegion != null)
        {
            // 在起始位置放置玩家
            PlacePlayerAtPosition(startRegion.GetSpawnPosition());

            // 发送玩家初始化完成事件
            this.SendEvent<PlayerInitializedEvent>();
        }
        else
        {
            Debug.LogError("未找到起始区域！");
        }
    }

    private IEnumerator HandleRegionEntry(HexRealm region)
    {
        // 检查是否为起始区域（无怪物）
        if (IsStartRegion(region))
        {
            Debug.Log("进入起始区域，无需生成怪物");
            yield break;
        }

        // 检查是否为终点区域（Boss战）
        if (IsEndRegion(region))
        {
            Debug.Log("进入终点区域，生成Boss");
            yield return SpawnBoss(region);
        }
        else
        {
            Debug.Log($"进入普通区域，生成怪物波次");
            yield return SpawnEnemyWaves(region);
        }
    }

    private IEnumerator HandleRegionReward(HexRealm region)
    {
        // 给予玩家Buff奖励
        Debug.Log($"区域 {region.GetRealmBiome().BiomeName} 清理完成，获得奖励");

        // 这里可以实现Buff系统的奖励逻辑
        // var buff = GetRandomBuff(region);
        // Player.AddBuff(buff);

        yield return new WaitForSeconds(1f);

        // 检查是否为最终Boss战胜利
        if (IsEndRegion(region))
        {
            this.SendEvent<GameVictoryEvent>();
        }
    }

    private IEnumerator SpawnEnemyWaves(HexRealm region)
    {
        // 根据区域类型生成不同的怪物波次
        int waveCount = GetWaveCountForRegion(region);

        for (int i = 0; i < waveCount; i++)
        {
            Debug.Log($"生成第 {i + 1} 波怪物");

            // 生成怪物的具体逻辑
            // SpawnEnemiesForWave(i, region);

            // 等待这波怪物被清理
            // yield return WaitForWaveCleared();

            yield return new WaitForSeconds(2f); // 临时等待
        }

        // 所有波次清理完成
        this.SendEvent(new RegionClearedEvent(region));
    }

    private IEnumerator SpawnBoss(HexRealm region)
    {
        Debug.Log("生成最终Boss");

        // 生成Boss的具体逻辑
        // var boss = SpawnFinalBoss(region);

        // 等待Boss被击败
        // yield return WaitForBossDefeated(boss);

        yield return new WaitForSeconds(5f); // 临时等待

        // Boss战胜利
        this.SendEvent(new RegionClearedEvent(region));
    }

    private IEnumerator ShowGameOverSequence()
    {
        // 播放游戏结束动画
        Debug.Log("播放游戏结束动画");

        yield return new WaitForSeconds(3f);

        // 返回主菜单
        ChangeGameState(GameState.MainMenu);
    }

    #endregion

    #region 辅助方法

    private HexRealm FindStartRegion()
    {
        // 查找起始区域的逻辑
        var realms = mapModel.HexGrid.GetHexRealms();
        // 这里应该根据实际逻辑找到起始区域
        return realms.Count > 0 ? realms[0] : null;
    }

    private void PlacePlayerAtPosition(Vector3 position)
    {
        // 在指定位置放置玩家
        var player = Object.FindObjectOfType<Player>();
        if (player != null)
        {
            player.transform.position = position;
        }
    }

    private bool IsStartRegion(HexRealm region)
    {
        // 判断是否为起始区域的逻辑
        // 可以根据区域标记或位置来判断
        return false; // 临时返回
    }

    private bool IsEndRegion(HexRealm region)
    {
        // 判断是否为终点区域的逻辑
        return false; // 临时返回
    }

    private int GetWaveCountForRegion(HexRealm region)
    {
        // 根据区域难度返回怪物波数
        return Random.Range(2, 5);
    }

    #endregion

    public GameState GetCurrentGameState()
    {
        return currentGameState;
    }
}*/