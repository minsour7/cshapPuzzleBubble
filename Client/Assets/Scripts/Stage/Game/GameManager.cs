using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Ãß°¡
using UnityEngine.UI;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    GameStateManager _StateManager = new GameStateManager();

    public GameObject BallSample;


    //RepeatedField<global::Google.Protobuf.Protocol.ColsBubbles> _colsBubbles;

    //public RepeatedField<global::Google.Protobuf.Protocol.ColsBubbles> ColsBubbles { get { return _colsBubbles; } }

    //public void SetColsBubbles(RepeatedField<global::Google.Protobuf.Protocol.ColsBubbles> colsBubbles)
    //{
    //    _colsBubbles = colsBubbles;
    //}

    public GameStateManager GetGameStateManager()
    {
        return _StateManager;
    }

    override protected void OnStart()
    {
        _StateManager.SetGameState(GameStateManager.E_GAME_STATE.READY);
    }

    override protected void OnUpdate()
    {
        base.OnUpdate();

        _StateManager.OnUpdate();
    }

}
