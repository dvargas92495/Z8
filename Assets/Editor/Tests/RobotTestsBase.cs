﻿using UnityEngine;
using NUnit.Framework;

public class RobotTestsBase
{
    internal static Game testgame;

    public void BeforeAllTests(string[] t1, string[] t2)
    {
        testgame = new Game();
        testgame.board = new Map(
            "5 8\n" +
            "A B V C D\n" +
            "W W P W W\n" +
            "W W W W W\n" +
            "W W W W W\n" +
            "W W W W W\n" +
            "W W W W W\n" +
            "W W p W W\n" +
            "a b V c d\n"
        );
        testgame.Join(t1, "primary", 1);
        testgame.Join(t2, "secondary", 2);
    }

    public void BeforeEachTest()
    {
        BeforeEachTest(new Dictionary<short, Vector2Int>(0));
    }

    public void BeforeEachTest(Dictionary<short, Vector2Int> pos)
    {
        Reset(testgame.primary, true, pos);
        Reset(testgame.secondary, false, pos);
    }

    private static void Reset(Game.Player p, bool isPrimary, Dictionary<short, Vector2Int> pos)
    {
        p.battery = GameConstants.POINTS_TO_WIN;
        p.team.ForEach(r =>
        {
            r.health = r.startingHealth;
            r.position = pos.Contains(r.id) ? pos.Get(r.id) : Map.NULL_VEC;
        });
    }

    internal static Command.Spawn SpawnCommand(byte d, short r)
    {
        Command.Spawn s = new Command.Spawn(d);
        s.robotId = r;
        return s;
    }

    internal static Command.Move MoveCommand(byte d, short r)
    {
        Command.Move m = new Command.Move(d);
        m.robotId = r;
        return m;
    }

    internal static Command.Attack AttackCommand(byte d, short r)
    {
        Command.Attack a = new Command.Attack(d);
        a.robotId = r;
        return a;
    }

    internal static SpawnEvent SuccessSpawnEvent(Vector2Int v, short r, byte p, int pb, int sb)
    {
        return new SpawnEvent() { destinationPos = v, robotId = r, priority = p, primaryBatteryCost = (short)pb, secondaryBatteryCost = (short)sb, success = true };
    }

    internal static SpawnEvent FailSpawnEvent(Vector2Int v, short robotId, byte p, int pb, int sb)
    {
        return new SpawnEvent(v, robotId, p, (short)pb, (short)sb, false);
    }

    internal static MoveEvent SuccessMoveEvent(Vector2Int vs, Vector2Int vd, short robotId, int p, int pb, int sb)
    {
        return new MoveEvent(vs, vd, robotId, (byte) p, (short)pb, (short)sb, true);
    }

    internal static MoveEvent FailMoveEvent(Vector2Int vs, Vector2Int vd, short robotId, byte p, int pb, int sb)
    {
        return new MoveEvent(vs, vd, robotId, p, (short)pb, (short)sb, false);
    }

    internal static AttackEvent SuccessAttackEvent(Vector2Int l, short r, byte p, int pb, int sb)
    {
        return new AttackEvent(l, r, p, (short)pb, (short)sb, true);
    }

    internal static MissEvent SuccessMissEvent(Vector2Int l, short r, byte p)
    {
        return new MissEvent(l, r, p, 0, 0, true);
    }

    internal static BlockEvent SuccessBlockEvent(Vector2Int v, short robotId, string blocker, byte p)
    {
        return new BlockEvent(robotId, blocker, v, p, 0, 0, true);
    }
    
    internal static DamageEvent SuccessDamageEvent(short robotId, short h, int rh, byte p)
    {
        return new DamageEvent(robotId, h, (short) rh, p, 0, 0, true);
    }

    internal static PushEvent SuccessPushEvent(short robotId, short v, Vector2Int d, byte p)
    {
        return new PushEvent(robotId, v, d, p, 0, 0, true);
    }

    internal static PushEvent FailPushEvent(short robotId, short v, Vector2Int d, byte p)
    {
        return new PushEvent(robotId, v, d, p, 0, 0, false);
    }

    internal static ResolveEvent SpawnResolveEvent(int p)
    {
        return new ResolveEvent(Command.SPAWN_COMMAND_ID, (byte) p);
    }

    internal static ResolveEvent MoveResolveEvent(int p)
    {
        return new ResolveEvent(Command.MOVE_COMMAND_ID, (byte)p);
    }

    internal static ResolveEvent AttackResolveEvent(int p)
    {
        return new ResolveEvent(Command.ATTACK_COMMAND_ID, (byte)p);
    }

    internal static List<GameEvent> SimulateCommands(params Command[] cmds)
    {
        List<Command> primaryCmds = new List<Command>();
        List<Command> secondaryCmds = new List<Command>();
        Util.ToList(cmds).ForEach(c =>
        {
            if (testgame.primary.team.Any(r => r.id == c.robotId)) primaryCmds.Add(c);
            else secondaryCmds.Add(c);
        });
        testgame.primary.StoreCommands(primaryCmds);
        testgame.secondary.StoreCommands(secondaryCmds);
        return new List<GameEvent>(testgame.CommandsToEvents().ToArray());
    }

    internal static void AssertExpectedGameEvents(List<GameEvent> actual, params GameEvent[] expected)
    {
        Assert.AreEqual(expected.Length, actual.GetLength(), "Actual List: {0}", actual);
        Util.ToIntList(expected.Length).ForEach(i => Assert.AreEqual(expected[i], actual.Get(i), "Failed event index: {0}", i));
    }

}
