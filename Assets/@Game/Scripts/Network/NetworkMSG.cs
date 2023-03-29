using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMSG
{


}

public enum NetworkCode
{
    C2S_CONNECT_MANAGER = 1,
    S2C_CONNECT_MANAGER_OK,
    S2C_GAME_READY,
    C2S_GAME_READY_OK,

    S2C_GAME_START,
    S2C_ANNOUNCE_STARTPLAYER,
    C2S_ANNOUNCE_STARTPLAYER_OK,
    S2C_START_TURN,
    C2S_IS_MYTURN,
    C2S_ATTACK_INFO,
    S2C_ATTACK_RESULT,
    C2S_END_TURN,

    C2S_HP_ZERO,
    S2C_ENDGAME,
    C2S_ENDGAME_OK,

    C2S_PLAYER_STATE,
}