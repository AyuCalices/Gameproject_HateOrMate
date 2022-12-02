using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Battle;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BattleManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private BattleData_SO battleData;
    
    private StateMachine _stageStateMachine;
    
    private void Awake()
    {
        _stageStateMachine = new StateMachine();
        _stageStateMachine.Initialize(new PausedState(this));
    }
    
    public void EnterPausedState()
    {
        _stageStateMachine.ChangeState(new PausedState(this));
    }

    public void EnterRunningState()
    {
        _stageStateMachine.ChangeState(new RunningState(this));
    }

    public void NextStage()
    {
        //TODO: implement here
    }

    public void RestartStage()
    {
        //TODO: implement here
    }

    public void OnEvent(EventData photonEvent)
    {
        throw new System.NotImplementedException();
    }
}
