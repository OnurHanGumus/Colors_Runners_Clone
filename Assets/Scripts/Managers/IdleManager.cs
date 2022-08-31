﻿using System;
using System.Collections.Generic;
using System.Linq;
using Data.UnityObject;
using Data.ValueObject;
using Enums;
using JetBrains.Annotations;
using Keys;
using Signals;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Managers
{
    public class IdleManager : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        

        #endregion

        #region Serialized Variables

        [SerializeField] private Transform idleLevelHolder;

        #endregion

        #region Private Variables

        private int _idleLevel;
        private int _currentLevel;
        private int _currentScore;
        private LevelData _levelsData;
        private List<LevelBuildingData> _levelBuildingDatas;
        private GameObject _planeGO;

        private List<GameObject> _mainAreas=new List<GameObject>();
        [ShowInInspector]private List<List<GameObject>> _sideAreas = new List<List<GameObject>>();

        private List<int> _mainCurrentScore;
        private List<int> _sideCurrentScore;
        private List<BuildingState> _mainBuildingState;
        private List<BuildingState> _sideBuildingState;
        private SaveIdleDataParams _saveIdleDataParams;

        private int _mainCache=0;
        private int _sideCache=0;
        //private List<GameObject> sideCache=new List<GameObject>();
        private bool _isMainSide;

        #endregion

        #endregion

        private void Awake()
        {
            _levelsData = GetIdleLevelBuildingData();
        }

        #region Event Subscription

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            SaveSignals.Instance.onSaveIdleParams += OnGetIdleSaveDatas;
            SaveSignals.Instance.onLoadIdleGame += LoadIdleDatas;
            LevelSignals.Instance.onNextLevel += OnNextLevel;
            IdleSignals.Instance.onMainSideComplete += OnMainSideComplete;
        }

        private void UnsubscribeEvents()
        {
            SaveSignals.Instance.onSaveIdleParams -= OnGetIdleSaveDatas;
            SaveSignals.Instance.onLoadIdleGame -= LoadIdleDatas;
            LevelSignals.Instance.onNextLevel -= OnNextLevel;
            IdleSignals.Instance.onMainSideComplete -= OnMainSideComplete;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion
        
        private void Start()
        {
            Init();
        }

        private void Init()
        {
            SaveSignals.Instance.onLoadIdle?.Invoke();
            GetCurrentLevelData();
            EmptyListChack();
            InstantiateLevelItems();
        }

        private void LoadIdleDatas(SaveIdleDataParams saveIdleDataParams)
        {
            _saveIdleDataParams = saveIdleDataParams;
            _idleLevel = _saveIdleDataParams.IdleLevel % _levelsData.LevelBuildings.Count;
            _mainCurrentScore = new List<int>(_saveIdleDataParams.MainCurrentScore);
            _sideCurrentScore = new List<int>(_saveIdleDataParams.SideCurrentScore);
            _mainBuildingState = new List<BuildingState>(_saveIdleDataParams.MainBuildingState);
            _sideBuildingState = new List<BuildingState>(_saveIdleDataParams.SideBuildingState);
        }

        private LevelData GetIdleLevelBuildingData() => Resources.Load<CD_LevelBuildingData>("Data/CD_IdleLevelBuild").Levels;

        private void GetCurrentLevelData()
        {
            _levelBuildingDatas = _levelsData.LevelBuildings[_idleLevel].LevelBuildingDatas;
            _planeGO = _levelsData.LevelBuildings[_idleLevel].LevelPlane;
        }

        private void EmptyListChack()
        {
            if (_mainCurrentScore.IsNullOrEmpty())
            {
                foreach (var levelBuildingDatas in _levelBuildingDatas)
                {
                    _mainCurrentScore.Add(0);
                    _mainBuildingState.Add(BuildingState.Uncompleted);
                    foreach (var VARIABLE in levelBuildingDatas.sideBuildindData)
                    {
                        _sideCurrentScore.Add(0);
                        _sideBuildingState.Add(BuildingState.Uncompleted);
                    }
                }
            }
        }
        
        private void InstantiateLevelItems()
        {
            CreateLevelPlane();
            foreach (var levelBuildingData in _levelBuildingDatas)
            {
                CreateLevelBuildings(levelBuildingData);
                _mainCache++;
            }
        }

        private void CreateLevelPlane()
        {
            Instantiate(_planeGO, idleLevelHolder.position,_planeGO.transform.rotation, idleLevelHolder);
        }
        
        private void CreateLevelBuildings(LevelBuildingData levelBuildingData)
        {
            CreateSideBuilding(levelBuildingData);
            CreateMainBuilding(levelBuildingData);
        }
        
        private void CreateMainBuilding(LevelBuildingData levelBuildingData)
        {
            var mainBuild = levelBuildingData.mainBuildingData.Building;
            var buildingPrice = levelBuildingData.mainBuildingData.MainBuildingScore;
            var position = idleLevelHolder.position;
            GameObject obj= Instantiate(mainBuild,position+ levelBuildingData.mainBuildingData.InstantitatePos,mainBuild.transform.rotation,idleLevelHolder);
            _mainAreas.Add(obj);
            MainSetReferance(obj, buildingPrice);
        }

        private void MainSetReferance(GameObject mainBuild, int buildingPrice)
        {
            mainBuild.GetComponent<IdleAreaManager>().SetBuildRef(_mainCache,true, buildingPrice,_mainCurrentScore[_mainCache],_mainBuildingState[_mainCache],this);
        }

        private void CreateSideBuilding(LevelBuildingData levelBuildingData)
        {
            List<GameObject> sideCache = new List<GameObject>();
            foreach (var sideBuilding in levelBuildingData.sideBuildindData)
            {
                var sideBuild = sideBuilding.Building;
                var buildingPrice = sideBuilding.SideBuildingScore;
                var position = idleLevelHolder.position;
                GameObject obj = Instantiate(sideBuild,position+ sideBuilding.InstantitatePos, Quaternion.identity,idleLevelHolder);
                sideCache.Add(obj);
                SideSetReferance(obj, buildingPrice);
                _sideCache++;
            }
            _sideAreas.Add(sideCache);
        }
        private void SideSetReferance(GameObject sideBuild, int buildingPrice)
        {
            sideBuild.GetComponent<IdleAreaManager>().SetBuildRef(_sideCache,false ,buildingPrice,_sideCurrentScore[_sideCache],_sideBuildingState[_sideCache],this);
        }

        private void OnMainSideComplete(int MainID)
        {
            Debug.Log(MainID);
            Debug.Log(_sideAreas[MainID][0]);
            foreach (var VARIABLE in _sideAreas[MainID])
            {
                
                VARIABLE.GetComponent<IdleAreaManager>().MainSideComplete();
            }
        }
        
        
        
        private void SaveParametres()
        {
            _isMainSide = true;
            foreach (var VARIABLE in _mainAreas)
            {
                VARIABLE.GetComponent<IdleAreaManager>().SendReftoIdleManager();
            }
            _isMainSide = false;
            
            foreach (var VARIABLE in _sideAreas.SelectMany(side => side))
            {
                VARIABLE.GetComponent<IdleAreaManager>().SendReftoIdleManager();
            }
            
            SaveSignals.Instance.onIdleSaveData?.Invoke();
        }


        public void SetSaveDatas(int areaID,int CurrentScore,BuildingState buildingState)
        {
            if (_isMainSide)
            {
                _mainCurrentScore[areaID] = CurrentScore;
                _mainBuildingState[areaID]= buildingState;
            }
            else
            {
                _sideCurrentScore[areaID]=CurrentScore;
                _sideBuildingState[areaID]=buildingState;
            }
        }

        private SaveIdleDataParams OnGetIdleSaveDatas()
        {
            return new SaveIdleDataParams()
            {
                IdleLevel = _idleLevel,
                CollectablesCount = 0,
                MainCurrentScore = _mainCurrentScore,
                SideCurrentScore = _sideCurrentScore,
                MainBuildingState = _mainBuildingState,
                SideBuildingState = _sideBuildingState
            };
        }

        private void OnNextLevel()
        {
            SaveParametres();
        }
    }
}