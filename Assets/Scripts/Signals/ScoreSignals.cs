using System;
using Extentions;
using UnityEngine;
using UnityEngine.Events;

namespace Signals
{
    public class ScoreSignals : MonoSingleton<ScoreSignals>
    {
        public UnityAction<int> onSetScore = delegate { };
        public UnityAction<int> onSetTotalScore = delegate { };
        public UnityAction<int> onSendFinalScore = delegate { };
        public UnityAction<float> onSendMoney = delegate { };
        public UnityAction<bool> onVisibleScore = delegate {  };
        public UnityAction<GameObject> onSetLeadPosition = delegate {  };
    }
}