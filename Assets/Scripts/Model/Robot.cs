﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Robot
{
    internal readonly string name;
    internal readonly string description;
    internal byte priority;
    internal short startingHealth;
    internal short health;
    internal short attack;
    internal Rating rating;
    internal short id;
    internal Vector2Int position;
    internal Orientation orientation;
    internal byte queueSpot;
    internal Robot(string _name, string _description)
    {
        name = _name;
        description = _description;
    }
    internal Robot(string _name, string _description, byte _priority, short _health, short _attack, Rating _rating)
    {
        name = _name;
        description = _description;
        priority = _priority;
        startingHealth = health = _health;
        attack = _attack;
        rating = _rating;
        position = Vector2Int.zero;
        orientation = Orientation.NORTH;
    }
    internal static Robot create(string robotName)
    {
        switch(robotName)
        {
            case Slinkbot._name:
                return new Slinkbot();
            case Pithon._name:
                return new Pithon();
            case Virusbot._name:
                return new Virusbot();
            case Jaguar._name:
                return new Jaguar();
            case Flybot._name:
                return new Flybot();
            case BronzeGrunt._name:
                return new BronzeGrunt();
            case SilverGrunt._name:
                return new SilverGrunt();
            case GoldenGrunt._name:
                return new GoldenGrunt();
            case PlatinumGrunt._name:
                return new PlatinumGrunt();
            default:
                throw new Exception("Invalid Robot Name: " + robotName);
        }
    }
    public void Serialize(NetworkWriter writer)
    {
        writer.Write(name);
        writer.Write(description);
        writer.Write(priority);
        writer.Write(health);
        writer.Write(attack);
        writer.Write((byte)rating);
        writer.Write(id);
        writer.Write(position.x);
        writer.Write(position.y);
        writer.Write((byte)orientation);
        writer.Write(queueSpot);
    }
    public static Robot Deserialize(NetworkReader reader)
    {
        string _name = reader.ReadString();
        string _description = reader.ReadString();
        Robot robot = new Robot(_name, _description);
        robot.priority = reader.ReadByte();
        robot.health = reader.ReadInt16();
        robot.attack = reader.ReadInt16();
        robot.rating = (Rating)reader.ReadByte();
        robot.id = reader.ReadInt16();
        robot.position = new Vector2Int();
        robot.position.x = reader.ReadInt32();
        robot.position.y = reader.ReadInt32();
        robot.orientation = (Orientation)reader.ReadByte();
        robot.queueSpot = reader.ReadByte();
        return robot;
    }
    public static Vector2Int OrientationToVector (Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.NORTH:
                return Vector2Int.up;
            case Orientation.SOUTH:
                return Vector2Int.down;
            case Orientation.WEST:
                return Vector2Int.left;
            case Orientation.EAST:
                return Vector2Int.right;
            default:
                return Vector2Int.zero;
        }
    }
    public enum Orientation
    {
        NORTH,
        SOUTH,
        WEST,
        EAST
    }
    internal enum Rating
    {
        PLATINUM = 4,
        GOLD = 3,
        SILVER = 2,
        BRONZE = 1
    }
    internal bool IsFacing(Vector2Int diff)
    {
        return diff.Equals(OrientationToVector(orientation));
    }
    internal List<Vector2Int> GetVictimLocations()
    {
        return new List<Vector2Int>() { position + OrientationToVector(orientation) };
    }

    internal List<GameEvent> Rotate(Command.Direction dir, bool isPrimary)
    {
        GameEvent.Rotate evt = new GameEvent.Rotate();
        evt.sourceDir = orientation;
        evt.destinationDir = Command.DirectionToOrientation(dir);
        evt.primaryRobotId = id;
        evt.primaryBattery = (isPrimary ? GameConstants.DEFAULT_ROTATE_POWER : (short)0);
        evt.secondaryBattery = (isPrimary ? (short)0 : GameConstants.DEFAULT_ROTATE_POWER);
        return new List<GameEvent>() { evt };
    }
    internal virtual List<GameEvent> Move(Command.Direction dir, bool isPrimary)
    {
        GameEvent.Move evt = new GameEvent.Move();
        evt.sourcePos = position;
        evt.destinationPos = position + Command.DirectionToVector(dir);
        evt.primaryRobotId = id;
        evt.primaryBattery = (isPrimary ? GameConstants.DEFAULT_MOVE_POWER : (short)0);
        evt.secondaryBattery = (isPrimary ? (short)0 : GameConstants.DEFAULT_MOVE_POWER);
        return new List<GameEvent>() { evt };
    }
    internal virtual List<GameEvent> Attack(bool isPrimary)
    {
        GameEvent.Attack evt = new GameEvent.Attack();
        evt.locs = GetVictimLocations().ToArray();
        evt.primaryRobotId = id;
        evt.primaryBattery = (isPrimary ? GameConstants.DEFAULT_ATTACK_POWER : (short)0);
        evt.secondaryBattery = (isPrimary ? (short)0 : GameConstants.DEFAULT_ATTACK_POWER);
        return new List<GameEvent>() { evt };
    }
    internal virtual List<GameEvent> Damage(Robot victim)
    {
        GameEvent.Damage evt = new GameEvent.Damage();
        evt.primaryRobotId = victim.id;
        evt.damage = attack;
        evt.remainingHealth = (short)(victim.health - attack);
        return new List<GameEvent>() { evt };
    }
    internal virtual List<GameEvent> CheckFail(Command c, Game.RobotTurnObject rto)
    {
        List<GameEvent> evts = new List<GameEvent>();
        byte limit = Game.RobotTurnObject.limit[c.GetType()];
        byte num = rto.num[c.GetType()];
        if (num < limit)
        {
            rto.num[c.GetType()]++;
        }
        else
        {
            evts.Add(Fail(c));
        }
        return evts;
    }
    internal GameEvent Fail(Command c)
    {
        GameEvent.Fail fail = new GameEvent.Fail();
        fail.failedCmd = c.GetType().ToString().Substring("Command.".Length);
        fail.primaryRobotId = c.robotId;
        return fail;
    }

    private class Slinkbot : Robot
    {
        internal const string _name = "Slinkbot";
        internal const string _description = "Forward Moves are 2 Spaces";
        internal Slinkbot() : base(
            _name,
            _description,
            6, 4, 3,
            Rating.SILVER
        )
        {}

        internal override List<GameEvent> Move(Command.Direction dir, bool isPrimary)
        {
            List<GameEvent> events = base.Move(dir, isPrimary);
            GameEvent.Move first = events[0] as GameEvent.Move;
            Vector2Int diff = first.destinationPos - first.sourcePos;
            if (IsFacing(diff))
            {
                GameEvent.Move second = new GameEvent.Move();
                second.primaryRobotId = first.primaryRobotId;
                second.sourcePos = first.destinationPos;
                second.destinationPos = first.destinationPos + diff;
                events.Add(second);
            }
            return events;
        }
    }

    private class Pithon : Robot
    {
        internal const string _name = "Pithon";
        internal const string _description = "Poison";
        internal Pithon() : base(
            _name,
            _description,
            6, 6, 2,
            Rating.SILVER
        )
        { }

        internal override List<GameEvent> Damage(Robot victim)
        {
            List<GameEvent> events = base.Damage(victim);
            GameEvent.Poison evt = new GameEvent.Poison();
            evt.primaryRobotId = victim.id;
            events.Add(evt);
            return events;
        }
    }

    private class Virusbot : Robot
    {
        internal const string _name = "Virusbot";
        internal const string _description = "Enemies damaged by this bot cannot move next turn";
        internal Virusbot() : base(
            _name,
            _description,
            7, 4, 1,
            Rating.SILVER
        )
        { }
    }

    private class Jaguar : Robot
    {
        internal const string _name = "Jaguar";
        internal const string _description = "Can Move a Third Time (-1 Attack)";
        internal Jaguar() : base(
            _name,
            _description,
            6, 4, 4,
            Rating.SILVER
        )
        { }

        internal override List<GameEvent> CheckFail(Command c, Game.RobotTurnObject rto)
        {
            if (c is Command.Rotate || c is Command.Special)
            {
                return base.CheckFail(c, rto);
            }
            else if (c is Command.Move)
            {
                if (rto.num[typeof(Command.Attack)] == Game.RobotTurnObject.limit[typeof(Command.Attack)])
                {
                    return base.CheckFail(c, rto);
                }
                else if (rto.num[c.GetType()] < Game.RobotTurnObject.limit[c.GetType()] + 1)
                {
                    rto.num[c.GetType()]++;
                    return new List<GameEvent>();
                }
                else
                {
                    return new List<GameEvent>() { Fail(c) };
                }
            } else
            {
                if (rto.num[typeof(Command.Move)] <= Game.RobotTurnObject.limit[typeof(Command.Move)])
                {
                    return base.CheckFail(c, rto);
                }
                else
                {
                    return new List<GameEvent>() { Fail(c) };
                }
            }
        }
    }

    private class Flybot : Robot
    {
        internal const string _name = "Flybot";
        internal const string _description = "Can't be damaged, poisoned";
        internal Flybot() : base(
            _name,
            _description,
            6, 6, 2,
            Rating.SILVER
        )
        { }
    }

    private class BronzeGrunt : Robot
    {
        internal const string _name = "Bronze Grunt";
        internal const string _description = "No Ability";
        internal BronzeGrunt() : base(
            _name,
            _description,
            5, 3, 2,
            Rating.BRONZE
        )
        { }
    }

    private class SilverGrunt : Robot
    {
        internal const string _name = "Silver Grunt";
        internal const string _description = "No Ability";
        internal SilverGrunt() : base(
            _name,
            _description,
            6, 8, 3,
            Rating.SILVER
        )
        { }
    }

    private class GoldenGrunt : Robot
    {
        internal const string _name = "Golden Grunt";
        internal const string _description = "No Ability";
        internal GoldenGrunt(): base(
            _name,
            _description,
            7, 10, 5,
            Rating.GOLD
        )
        { }
    }

    private class PlatinumGrunt : Robot
    {
        internal const string _name = "Platinum Grunt";
        internal const string _description = "No Ability";
        internal PlatinumGrunt() : base(
            _name,
            _description,
            8, 15, 6,
            Rating.PLATINUM
        )
        { }
    }


}