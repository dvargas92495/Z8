﻿using System;
using UnityEngine;
using UnityEngine.Networking;

public class Messages {
    public const short START_LOCAL_GAME = MsgType.Highest + 1;
    public const short START_GAME = MsgType.Highest + 2;
    public const short GAME_READY = MsgType.Highest + 3;
    public const short SUBMIT_COMMANDS = MsgType.Highest + 4;
    public const short TURN_EVENTS = MsgType.Highest + 5;
    public class EmptyMessage : MessageBase { }
    public static EmptyMessage EMPTY = new EmptyMessage();
    public class StartLocalGameMessage : MessageBase
    {
        public String[] myRobots;
        public String[] opponentRobots;
    }
    public class StartGameMessage : MessageBase
    {
        public String[] myRobots;
    }
    public class GameReadyMessage : MessageBase
    {
        public String myname;
        public String opponentname;
        public int numRobots;
        public string[] robotNames;
        public int[] robotHealth;
        public int[] robotAttacks;
        public int[] robotPriorities;
        public bool[] robotIsOpponents;
    }
    public class SubmitCommandsMessage : MessageBase
    {

    }
    public class TurnEventsMessage : MessageBase
    {

    }
}
