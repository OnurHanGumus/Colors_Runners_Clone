using System;
using System.Collections.Generic;
using Controllers;
using Enums;
using Signals;
using TMPro;
using UnityEngine;
using Commands;


namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables
        [SerializeField] private List<GameObject> panels;
        // [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshPro scoreTMP;


        #endregion

        #region Private Variables
        private UIPanelController _uiPanelController;
        private Text2xController text2xController;
        private bool _isReadyForIdleGame = false;
        #endregion

        #endregion

        private void Awake()
        {
            Init();
            _uiPanelController = new UIPanelController();
        }

        private void Init()
        {
            text2xController = GetComponent<Text2xController>();

        }

        #region Event Subscriptions

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onPlay += OnPlay;
            CoreGameSignals.Instance.onChangeGameState += OnChangeGameState;
            UISignals.Instance.onOpenPanel += OnOpenPanel;
            UISignals.Instance.onClosePanel += OnClosePanel;
//            UISignals.Instance.onSetLevelText += OnSetLevelText;
            UISignals.Instance.onSetScoreText += OnSetScoreText;
            LevelSignals.Instance.onLevelFailed += OnLevelFailed;
            LevelSignals.Instance.onLevelSuccessful += OnLevelSuccessful;
            GunPoolSignals.Instance.onGunPoolExit += text2xController.Show2XText;
            StackSignals.Instance.onLastCollectableAddedToPlayer += OnLastCollectableAddedToPlayer;
        }

        private void UnsubscribeEvents()
        {
            CoreGameSignals.Instance.onPlay -= OnPlay;
            CoreGameSignals.Instance.onChangeGameState -= OnChangeGameState;
            UISignals.Instance.onOpenPanel -= OnOpenPanel;
            UISignals.Instance.onClosePanel -= OnClosePanel;
//            UISignals.Instance.onSetLevelText -= OnSetLevelText;
            UISignals.Instance.onSetScoreText -= OnSetScoreText;
            LevelSignals.Instance.onLevelFailed -= OnLevelFailed;
            LevelSignals.Instance.onLevelSuccessful -= OnLevelSuccessful;
            GunPoolSignals.Instance.onGunPoolExit -= text2xController.Show2XText;
            StackSignals.Instance.onLastCollectableAddedToPlayer -= OnLastCollectableAddedToPlayer;

        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        #region Event Methods

        private void OnOpenPanel(UIPanels panelParam)
        {
            _uiPanelController.OpenPanel(panelParam , panels);
        }

        private void OnClosePanel(UIPanels panelParam)
        {
            _uiPanelController.ClosePanel(panelParam , panels);
        }
        
        private void OnSetScoreText(int value)
        {
            scoreTMP.text = (value.ToString());
        }
        
        private void OnPlay()
        {
            UISignals.Instance.onClosePanel?.Invoke(UIPanels.StartPanel);
            UISignals.Instance.onOpenPanel?.Invoke(UIPanels.LevelPanel);
        }

        private void OnLevelFailed()
        {
            UISignals.Instance.onClosePanel?.Invoke(UIPanels.LevelPanel);
            UISignals.Instance.onOpenPanel?.Invoke(UIPanels.FailPanel);
        }

        private void OnLevelSuccessful()
        {
            UISignals.Instance.onClosePanel?.Invoke(UIPanels.LevelPanel);
            UISignals.Instance.onOpenPanel?.Invoke(UIPanels.WinPanel);
        }

        private void OnChangeGameState()
        {
            UISignals.Instance.onClosePanel?.Invoke(UIPanels.WinPanel);
            UISignals.Instance.onOpenPanel?.Invoke(UIPanels.IdlePanel);
        }

        #region Useless

        // private void OnSetLevelText(int value)
        // {
        //     //evelText.text = "Level " + (value + 1);
        // }

        #endregion

       

        #endregion

        #region Buttons

        public void Play()
        {
            CoreGameSignals.Instance.onPlay?.Invoke();
        }
        
        public void RestartLevel()
        {
            LevelSignals.Instance.onRestartLevel?.Invoke();
            UISignals.Instance.onClosePanel?.Invoke(UIPanels.FailPanel);
            UISignals.Instance.onOpenPanel?.Invoke(UIPanels.StartPanel);
        }
        
        public void NextLevel()
        {
            LevelSignals.Instance.onNextLevel?.Invoke();
            UISignals.Instance.onClosePanel?.Invoke(UIPanels.IdlePanel);
            UISignals.Instance.onOpenPanel?.Invoke(UIPanels.LevelPanel);
            UISignals.Instance.onOpenPanel?.Invoke(UIPanels.StartPanel);
        }

        public void Claim()
        {
            if (_isReadyForIdleGame)
            {
                CoreGameSignals.Instance.onChangeGameState?.Invoke();
                ScoreSignals.Instance.onSendFinalScore?.Invoke();
            }
        }

        public void NoThanks()
        {
            if (_isReadyForIdleGame)
            {
                CoreGameSignals.Instance.onChangeGameState?.Invoke();
            }
        }

        private void OnLastCollectableAddedToPlayer(bool isReady)
        {
            _isReadyForIdleGame = isReady;
        }

        
        #endregion
    }
}