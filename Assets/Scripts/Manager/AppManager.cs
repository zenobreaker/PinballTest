using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.GridLayoutGroup;

public class AppManager
    : Singleton<AppManager>
{
    public Action OnAwaked;

    //private DataBaseManager databaseManager;
    //private SkillManager skillManager;
    //private SkillTreeManager skillTree;
    //private RewardManager rewardManager;
    //private RecordManager recordManager;
    //private ExploreManager exploreManager;

    [SerializeField] private bool bCheat;
    public bool Cheat => bCheat;

    // 패시브 스킬을 처리하는 시스템 클래스 
    //private PassiveSystem passiveSystem = new();

    protected override void Awake()
    {
        base.Awake();

        if (IsDuplicate) return; 

        if (Instance != this)
            return;

        //databaseManager = GetComponent<DataBaseManager>();
        //skillManager = GetComponent<SkillManager>();
        //skillTree = SkillTreeManager.Instance;
        //rewardManager = GetComponent<RewardManager>();
        //recordManager = GetComponent<RecordManager>();
        //exploreManager = GetComponent<ExploreManager>();

        //if (IsInitialized == false)
        //{
        //    skillManager.OnDataChanaged += () => { PlayerManager.Instance.SetDirty(); };
        //    InventoryManager.Instance?.OnInit();
        //    PlayerManager.Instance?.OnInit();
        //    CurrencyManager.Instance?.OnInit((CurrencyInventory)InventoryManager.Instance?.GetInvetory(ItemCategory.CURRENCY));
        //    recordManager.OnInit();
        //    SceneManager.sceneUnloaded += OnUnloadScene;
        //}

        //if (exploreManager != null)
        //{
        //    ManagerWaiter.WaitForManager<UIManager>((uiManager) =>
        //    {
        //        uiManager.OnReturnedStageSelect += exploreManager.OnReturnedStageSelectScene;
        //    });
        //}

        //passiveSystem?.OnInit();

        //if (GameManager.Instance != null)
        //{
        //    GameManager.Instance.OnBeginStage += OnBeginStage;
        //    GameManager.Instance.OnUpdated += OnUpdate;
        //    GameManager.Instance.OnFinishStage += FinishStageProcess;
        //}

        //if (exploreManager != null)
        //{
        //    exploreManager.OnExploreStart += HandleExploreStart;
        //    exploreManager.OnReturnToMain += HandleReturnToMain;
        //    exploreManager.OnInStage += HandleInStage;
        //    exploreManager.OnStageClear += HandleStageClear;
        //    exploreManager.OnExploreFinish += HanldeExploreFinish;
        //}

        //OnAwaked?.Invoke();
        //OnAwaked = null;

        //PauseManager.Reset();
    }

    private void OnApplicationQuit()
    {
        SaveIfDirty();
    }

    protected override void SyncDataFromSingleton()
    {
        // 기존 싱글톤 인스턴스가 자기 자신이 아니면
        //if (Instance != this)
        //{
        //    skillManager = Instance.skillManager;
        //    skillTree = Instance.skillTree;
        //    databaseManager = Instance.databaseManager;
        //    recordManager = Instance.recordManager;
        //    exploreManager = Instance.exploreManager;

        //    bCheat = Instance.bCheat;

        //    OnAwaked = Instance.OnAwaked;
        //}
    }

    private void OnDisable()
    {
        if (Instance != null) return;

        if (GameManager.Instance == null) return;

        //GameManager.Instance.OnBeginStage -= OnBeginStage;
        //GameManager.Instance.OnUpdated -= OnUpdate;
        //GameManager.Instance.OnFinishStage -= FinishStageProcess;
    }

    #region Skill 

    public void EquipSavedClassActiveSkill(int classID, List<int> skillIDs)
    {
        //if (skillManager == null || skillTree == null) return;

        //int slot = 0;
        //foreach (int skillID in skillIDs)
        //{
        //    SkillRuntimeData runtimeData = skillTree.GetSkillRuntimeData(classID, skillID);
        //    EquipActiveSkill(classID, slot, runtimeData);
        //    slot++;
        //}
    }

    //public void EquipActiveSkill(int charId, int slot, SkillRuntimeData skill)
    //{
    //    if (skillManager == null) return;

    //    skillManager.EquipActiveSkill(charId, slot, skill);
    //}

    //public void UnequipActiveSkill(int charId, int slot)
    //{
    //    if (skillManager == null) return;

    //    skillManager.EquipActiveSkill(charId, slot, null);
    //}

    //public List<SkillRuntimeData> GetEquippedActiveSkillListByCharID(int charId)
    //{
    //    if (skillManager == null) return null;

    //    return skillManager.GetActiveSkillList(charId);
    //}

    //public List<int> GetEquippedActiveSkillIDListByCharID(int charID)
    //{
    //    if (skillManager == null) return null;
    //    return skillManager.GetActiveSkillIDList(charID);
    //}

    //public void SetActiveSkills(int jobID, SkillComponent skillComp)
    //{
    //    if (skillManager == null || skillComp == null) return;
    //    skillManager.SetActiveSkills(jobID, skillComp);
    //}

    //public PassiveSystem GetPassiveSystem() { return passiveSystem; }

    //public void AddPassiveSkill(int jobID, PassiveSkill passiveSkill)
    //{
    //    if (passiveSystem == null) return;
    //    passiveSystem.Add(jobID, passiveSkill);
    //}

    //public void RemovePassiveSkill(int jobID, PassiveSkill passiveSkill)
    //{
    //    if (passiveSystem == null) return;
    //    passiveSystem.Remove(jobID, passiveSkill);
    //}

    //public void OnApplyStaticEffct(int jobID, Character owner)
    //{
    //    passiveSystem?.OnApplyStaticEffect(jobID, owner);
    //}

    //public void OnAcquire(int jobID, Character owner)
    //{
    //    passiveSystem?.OnAcquire(jobID, owner);
    //}

    //public void OnLose(int jobID, Character owner)
    //{
    //    passiveSystem?.OnLose(jobID, owner);
    //}

    //public void OnUpdate(float dt)
    //{
    //    passiveSystem?.OnUpdate(dt);
    //}

    //public void OnChangedLevelPassiveSkill(int jobID, SkillRuntimeData data)
    //{
    //    passiveSystem?.OnChangedLevel(jobID, data);
    //}

    #endregion


    #region Record Data 
    //public void TriggerRecordUI(List<RecordData> records, bool canReroll = true, RecordUIMode mode = RecordUIMode.DRAFT)
    //{
    //    PauseManager.RequestPause();
    //    UIManager.Instance.SafeInvoke(v=>v.OpenRecordSelectPopUp(records, canReroll, mode));
    //}

    //public void OnRecordSelected(RecordData selected)
    //{
    //    // 선택된 레코드 알림 
    //    recordManager.SafeInvoke(v=>v.SelectedRecord(selected));

    //    OnSelectedRecordCard?.Invoke();
    //}

    

    //public void GenerateRecord_Test(int recordCount, bool canReroll = true)
    //{
    //    recordManager.SafeInvoke(v => v.GenerateRewardRecords(recordCount, canReroll));
    //}


    //public RecordManager GetRecordManager() { return recordManager; }

    // TODO : 레코드 매니저에게 매개변수 없이 전달하면 내부에서 알아서 처리하게
    // 레코드 등장 개수가 특정 이벤트에 의해서 적어지거나 하는 가변적일 수 있음 
    // 레코드 매니저, 리워드 매니저 등 팝업을 띄우는 대상은 UI매니저의 인큐 함수를 
    // 초기에 등록시켜서 하면 좋을 듯

    #endregion

    #region Scene Navigation (UI Event)
    // 일반 스테이지 결과창에서 [다음으로] 버튼 클릭 시
    public void MoveToNextNodeScene()
    {
        // 탐사 맵 씬으로 이동
        SceneManager.LoadScene(1);
    }

    // 총 결산창에서 [로비로 돌아가기] 버튼 클릭 시
    public void ReturnToLobbyScene()
    {
        
        // 탐사 데이터 완전 초기화
        //ResetData();
        // 로비 씬으로 이동
        SceneManager.LoadScene(0);
    }
    #endregion

    #region Save

    private void OnBeginStage()
    {
        //TODO : 선택한 휠러의 직업들을 가져와 세팅해야 한다. 
    }

    public void OnUnloadScene(Scene scene)
    {
        SaveIfDirty();
    }

    public void SaveIfDirty()
    {
       // skillTree.SafeInvoke(v => v.SaveIfDirty());
        InventoryManager.Instance.SafeInvoke(v => v.SaveIfDirty());
        //PlayerManager.Instance.SafeInvoke(v => v.SaveIfDirty());
        //exploreManager.SafeInvoke(v => v.SaveExploreMap());
        //recordManager.SafeInvoke(v => v.SaveIfDirty());
    }

    public void SaveExploreMap()
    {
        //exploreManager.SafeInvoke(v => v.SaveExploreMap());
    }
    #endregion

    public Sprite GetStageIcon(StageType type)
    {
        return null; // databaseManager.SafeInvoke(v => v.GetStageIcon(type));
    }
}
