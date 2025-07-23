/*using System.Collections;
using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

public class GameFlowSystem : AbstractSystem
{
    private GameCoreModel gameCoreModel;
    private MapModel mapModel;
    private GameState currentGameState;

    // ��Ϸ���̿����� - MonoBehaviour
    private GameFlowController gameFlowController;

    protected override void OnInit()
    {
        gameCoreModel = this.GetModel<GameCoreModel>();
        mapModel = this.GetModel<MapModel>();

        currentGameState = GameState.MainMenu;

        // ��������ʼ�� GameFlowController
        CreateGameFlowController();

        // ע���¼�����
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
        // ������ͼ��������¼�
        this.RegisterEvent<MapGenerationCompletedEvent>(OnMapGenerationCompleted);

        // ������ҳ�ʼ������¼�
        this.RegisterEvent<PlayerInitializedEvent>(OnPlayerInitialized);

        // ������������¼�
        this.RegisterEvent<PlayerEnteredRegionEvent>(OnPlayerEnteredRegion);

        // ����������������¼�
        this.RegisterEvent<RegionClearedEvent>(OnRegionCleared);

        // ������Ϸʤ���¼�
        this.RegisterEvent<GameVictoryEvent>(OnGameVictory);

        // ������Ϸʧ���¼�
        this.RegisterEvent<GameOverEvent>(OnGameOver);
    }

    #region ��Ϸ״̬����

    public void StartNewGame(MapSize mapSize)
    {
        ChangeGameState(GameState.MapGeneration);

        // ִ�е�ͼ��������
        this.SendCommand(new CreateMapCommand(mapSize));
    }

    public void ChangeGameState(GameState newState)
    {
        if (currentGameState == newState) return;

        Debug.Log($"��Ϸ״̬�л�: {currentGameState} -> {newState}");

        var previousState = currentGameState;
        currentGameState = newState;

        // ������״ִ̬����Ӧ�߼�
        OnGameStateChanged(previousState, newState);

        // ����״̬�仯�¼�
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

    #region ״̬������

    private void HandleMainMenuState()
    {
        // ���˵�״̬����
        if (SceneManager.GetActiveScene().name != GameScene.MainMenuScene.ToString())
        {
            gameFlowController.StartCoroutine(LoadSceneAsync(GameScene.MainMenuScene));
        }
    }

    private void HandleMapGenerationState()
    {
        // ��ͼ����״̬ - �ȴ���ͼ�������
        Debug.Log("��ʼ��ͼ����...");
    }

    private void HandlePlayerInitState()
    {
        // ��ҳ�ʼ��״̬
        gameFlowController.StartCoroutine(InitializePlayer());
    }

    private void HandleGamePlayingState()
    {
        // ��Ϸ����״̬
        Debug.Log("��Ϸ��ʼ��");
        // ������Գ�ʼ����ϷUI����Ч��
    }

    private void HandleGameOverState()
    {
        // ��Ϸ����״̬
        gameFlowController.StartCoroutine(ShowGameOverSequence());
    }

    #endregion

    #region �¼�����

    private void OnMapGenerationCompleted(MapGenerationCompletedEvent e)
    {
        Debug.Log("��ͼ������ɣ�׼����ʼ�����");
        ChangeGameState(GameState.PlayerInit);
    }

    private void OnPlayerInitialized(PlayerInitializedEvent e)
    {
        Debug.Log("��ҳ�ʼ����ɣ���Ϸ��ʽ��ʼ");
        ChangeGameState(GameState.GamePlaying);
    }

    private void OnPlayerEnteredRegion(PlayerEnteredRegionEvent e)
    {
        // ��ҽ������������ɹ���
        gameFlowController.StartCoroutine(HandleRegionEntry(e.region));
    }

    private void OnRegionCleared(RegionClearedEvent e)
    {
        // ����������ɣ����轱��
        gameFlowController.StartCoroutine(HandleRegionReward(e.region));
    }

    private void OnGameVictory(GameVictoryEvent e)
    {
        Debug.Log("��Ϸʤ����");
        ChangeGameState(GameState.GameOver);
    }

    private void OnGameOver(GameOverEvent e)
    {
        Debug.Log("��Ϸʧ�ܣ�");
        ChangeGameState(GameState.GameOver);
    }

    #endregion

    #region Э�̷���

    private IEnumerator LoadSceneAsync(GameScene scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString());
        yield return operation;
    }

    private IEnumerator InitializePlayer()
    {
        // �ȴ������������
        yield return new WaitForSeconds(0.1f);

        // ������ʼ����
        var startRegion = FindStartRegion();
        if (startRegion != null)
        {
            // ����ʼλ�÷������
            PlacePlayerAtPosition(startRegion.GetSpawnPosition());

            // ������ҳ�ʼ������¼�
            this.SendEvent<PlayerInitializedEvent>();
        }
        else
        {
            Debug.LogError("δ�ҵ���ʼ����");
        }
    }

    private IEnumerator HandleRegionEntry(HexRealm region)
    {
        // ����Ƿ�Ϊ��ʼ�����޹��
        if (IsStartRegion(region))
        {
            Debug.Log("������ʼ�����������ɹ���");
            yield break;
        }

        // ����Ƿ�Ϊ�յ�����Bossս��
        if (IsEndRegion(region))
        {
            Debug.Log("�����յ���������Boss");
            yield return SpawnBoss(region);
        }
        else
        {
            Debug.Log($"������ͨ�������ɹ��ﲨ��");
            yield return SpawnEnemyWaves(region);
        }
    }

    private IEnumerator HandleRegionReward(HexRealm region)
    {
        // �������Buff����
        Debug.Log($"���� {region.GetRealmBiome().BiomeName} ������ɣ���ý���");

        // �������ʵ��Buffϵͳ�Ľ����߼�
        // var buff = GetRandomBuff(region);
        // Player.AddBuff(buff);

        yield return new WaitForSeconds(1f);

        // ����Ƿ�Ϊ����Bossսʤ��
        if (IsEndRegion(region))
        {
            this.SendEvent<GameVictoryEvent>();
        }
    }

    private IEnumerator SpawnEnemyWaves(HexRealm region)
    {
        // ���������������ɲ�ͬ�Ĺ��ﲨ��
        int waveCount = GetWaveCountForRegion(region);

        for (int i = 0; i < waveCount; i++)
        {
            Debug.Log($"���ɵ� {i + 1} ������");

            // ���ɹ���ľ����߼�
            // SpawnEnemiesForWave(i, region);

            // �ȴ��Ⲩ���ﱻ����
            // yield return WaitForWaveCleared();

            yield return new WaitForSeconds(2f); // ��ʱ�ȴ�
        }

        // ���в����������
        this.SendEvent(new RegionClearedEvent(region));
    }

    private IEnumerator SpawnBoss(HexRealm region)
    {
        Debug.Log("��������Boss");

        // ����Boss�ľ����߼�
        // var boss = SpawnFinalBoss(region);

        // �ȴ�Boss������
        // yield return WaitForBossDefeated(boss);

        yield return new WaitForSeconds(5f); // ��ʱ�ȴ�

        // Bossսʤ��
        this.SendEvent(new RegionClearedEvent(region));
    }

    private IEnumerator ShowGameOverSequence()
    {
        // ������Ϸ��������
        Debug.Log("������Ϸ��������");

        yield return new WaitForSeconds(3f);

        // �������˵�
        ChangeGameState(GameState.MainMenu);
    }

    #endregion

    #region ��������

    private HexRealm FindStartRegion()
    {
        // ������ʼ������߼�
        var realms = mapModel.HexGrid.GetHexRealms();
        // ����Ӧ�ø���ʵ���߼��ҵ���ʼ����
        return realms.Count > 0 ? realms[0] : null;
    }

    private void PlacePlayerAtPosition(Vector3 position)
    {
        // ��ָ��λ�÷������
        var player = Object.FindObjectOfType<Player>();
        if (player != null)
        {
            player.transform.position = position;
        }
    }

    private bool IsStartRegion(HexRealm region)
    {
        // �ж��Ƿ�Ϊ��ʼ������߼�
        // ���Ը��������ǻ�λ�����ж�
        return false; // ��ʱ����
    }

    private bool IsEndRegion(HexRealm region)
    {
        // �ж��Ƿ�Ϊ�յ�������߼�
        return false; // ��ʱ����
    }

    private int GetWaveCountForRegion(HexRealm region)
    {
        // ���������Ѷȷ��ع��ﲨ��
        return Random.Range(2, 5);
    }

    #endregion

    public GameState GetCurrentGameState()
    {
        return currentGameState;
    }
}*/