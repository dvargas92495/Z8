﻿using UnityEngine.Events;
using UnityEngine.Networking;

public class LocalGameClient : GameClient
{

    protected override void Send(short msgType, MessageBase message)
    {
    }

    internal void ConnectToGameServer()
    {
    }

    internal void Receive(short msgType, MessageBase message)
    {
        NetworkMessage netMsg = new NetworkMessage();
        NetworkWriter writer = new NetworkWriter();
        message.Serialize(writer);
        NetworkReader reader = new NetworkReader(writer);
        netMsg.msgType = msgType;
        netMsg.reader = reader;
        GetHandler(msgType)(netMsg);
    }

    internal void SendLocalGameRequest(string[] myRobots, string[] opponentRobots, string myname, string opponentname, UnityAction<List<Robot>, List<Robot>, string, Map> readyCallback)
    {
        Messages.StartLocalGameMessage msg = new Messages.StartLocalGameMessage();
        msg.myRobots = myRobots;
        msg.opponentRobots = opponentRobots;
        msg.myName = myname;
        msg.opponentName = opponentname;
        gameReadyCallback = readyCallback;
        Send(Messages.START_LOCAL_GAME, msg);
    }
}
