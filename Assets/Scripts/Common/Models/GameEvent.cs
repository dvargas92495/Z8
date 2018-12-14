﻿using UnityEngine;
using UnityEngine.Networking;

public abstract class GameEvent
{
    internal short primaryRobotId;
    internal byte priority;
    internal short primaryBattery;
    internal short secondaryBattery;
    internal bool success = true;
    public GameEvent() { }
    public void FinishMessage(NetworkWriter writer)
    {
        writer.Write(primaryRobotId);
        writer.Write(priority);
        writer.Write(primaryBattery);
        writer.Write(secondaryBattery);
        writer.Write(success);
    }
    public abstract void Serialize(NetworkWriter writer);
    public static GameEvent Deserialize(NetworkReader reader)
    {
        byte eventId = reader.ReadByte();
        GameEvent evt;
        switch (eventId)
        {
            case Spawn.EVENT_ID:
                evt = Spawn.Deserialize(reader);
                break;
            case Move.EVENT_ID:
                evt = Move.Deserialize(reader);
                break;
            case Attack.EVENT_ID:
                evt = Attack.Deserialize(reader);
                break;
            case Block.EVENT_ID:
                evt = Block.Deserialize(reader);
                break;
            case Push.EVENT_ID:
                evt = Push.Deserialize(reader);
                break;
            case Miss.EVENT_ID:
                evt = Miss.Deserialize(reader);
                break;
            case Battery.EVENT_ID:
                evt = Battery.Deserialize(reader);
                break;
            case Death.EVENT_ID:
                evt = Death.Deserialize(reader);
                break;
            case Poison.EVENT_ID:
                evt = Poison.Deserialize(reader);
                break;
            case Damage.EVENT_ID:
                evt = Damage.Deserialize(reader);
                break;
            case Resolve.EVENT_ID:
                evt = Resolve.Deserialize(reader);
                break;
            case End.EVENT_ID:
                evt = End.Deserialize(reader);
                break;
            case Collision.EVENT_ID:
                evt = End.Deserialize(reader);
                break;
            default:
                throw new ZException("Unknown Event Id to deserialize: " + eventId);
        }
        evt.primaryRobotId = reader.ReadInt16();
        evt.priority = reader.ReadByte();
        evt.primaryBattery = reader.ReadInt16();
        evt.secondaryBattery = reader.ReadInt16();
        evt.success = reader.ReadBoolean();
        return evt;
    }
    public override string ToString()
    {
        return "Empty Event";
    }
    public string ToString(string message)
    {
        return "Robot " + primaryRobotId + " " + message;
    }
    public void Transfer(GameEvent g)
    {
        primaryRobotId = g.primaryRobotId;
        primaryBattery = g.primaryBattery;
        secondaryBattery = g.secondaryBattery;
    }
    public void Flip()
    {
        short battery = primaryBattery;
        primaryBattery = secondaryBattery;
        secondaryBattery = battery;
    }
    public class Empty : GameEvent
    {
        internal const byte EVENT_ID = 0;
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
        }
    }

    public class Spawn : GameEvent
    {
        internal const byte EVENT_ID = 1;
        internal Vector2Int destinationPos;
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(destinationPos.x);
            writer.Write(destinationPos.y);
        }
        public new static Spawn Deserialize(NetworkReader reader)
        {
            Spawn evt = new Spawn();
            evt.destinationPos = new Vector2Int();
            evt.destinationPos.x = reader.ReadInt32();
            evt.destinationPos.y = reader.ReadInt32();
            return evt;
        }
        public override string ToString()
        {
            return ToString("moved");
        }
    }

    public class Move : GameEvent
    {
        internal const byte EVENT_ID = 2;
        internal Vector2Int sourcePos;
        internal Vector2Int destinationPos;
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(sourcePos.x);
            writer.Write(sourcePos.y);
            writer.Write(destinationPos.x);
            writer.Write(destinationPos.y);
        }
        public new static Move Deserialize(NetworkReader reader)
        {
            Move evt = new Move();
            evt.sourcePos = new Vector2Int();
            evt.sourcePos.x = reader.ReadInt32();
            evt.sourcePos.y = reader.ReadInt32();
            evt.destinationPos = new Vector2Int();
            evt.destinationPos.x = reader.ReadInt32();
            evt.destinationPos.y = reader.ReadInt32();
            return evt;
        }
        public override string ToString()
        {
            return ToString("moved");
        }
    }

    public class Attack : GameEvent
    {
        internal const byte EVENT_ID = 3;
        internal List<Vector2Int> locs; 
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(locs.GetLength());
            locs.ForEach(l =>
            {
                writer.Write(l.x);
                writer.Write(l.y);
            });
        }
        public new static Attack Deserialize(NetworkReader reader)
        {
            Attack evt = new Attack();
            int length = reader.ReadInt32();
            evt.locs = new List<Vector2Int>();
            for (int i = 0;i < length; i++)
            {
                evt.locs.Add(new Vector2Int(reader.ReadInt32(), reader.ReadInt32()));
            }
            return evt;
        }
        public override string ToString()
        {
            return ToString("attacked");
        }
    }

    public class Block : GameEvent
    {
        internal const byte EVENT_ID = 4;
        internal string blockingObject;
        internal Vector2Int deniedPos;
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(blockingObject);
            writer.Write(deniedPos.x);
            writer.Write(deniedPos.y);
        }
        public new static Block Deserialize(NetworkReader reader)
        {
            Block evt = new Block();
            evt.blockingObject = reader.ReadString();
            evt.deniedPos = new Vector2Int();
            evt.deniedPos.x = reader.ReadInt32();
            evt.deniedPos.y = reader.ReadInt32();
            return evt;
        }
        public override string ToString()
        {
            return ToString("was blocked by " + blockingObject);
        }
    }

    public class Push : GameEvent
    {
        internal const byte EVENT_ID = 5;
        internal short victim;
        internal Vector2Int direction;
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(victim);
            writer.Write(direction.x);
            writer.Write(direction.y);
        }
        public new static Push Deserialize(NetworkReader reader)
        {
            Push evt = new Push();
            evt.victim = reader.ReadInt16();
            evt.direction = new Vector2Int();
            evt.direction.x = reader.ReadInt32();
            evt.direction.y = reader.ReadInt32();
            return evt;
        }
        public override string ToString()
        {
            return ToString("pushed " + victim);
        }
    }

    public class Miss : GameEvent
    {
        internal const byte EVENT_ID = 6;
        internal List<Vector2Int> locs;
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(locs.GetLength());
            locs.ForEach(l =>
            {
                writer.Write(l.x);
                writer.Write(l.y);
            });
        }
        public new static Miss Deserialize(NetworkReader reader)
        {
            Miss evt = new Miss();
            int length = reader.ReadInt32();
            evt.locs = new List<Vector2Int>();
            for (int i = 0; i < length; i++)
            {
                evt.locs.Add(new Vector2Int(reader.ReadInt32(), reader.ReadInt32()));
            }
            return evt;
        }
        public override string ToString()
        {
            return ToString("attacked but missed");
        }
    }

    public class Battery : GameEvent
    {
        internal const byte EVENT_ID = 7;
        internal short damage;
        internal bool isPrimary;
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(damage);
            writer.Write(isPrimary);
        }
        public new static Battery Deserialize(NetworkReader reader)
        {
            Battery evt = new Battery();
            evt.damage = reader.ReadInt16();
            evt.isPrimary = reader.ReadBoolean();
            return evt;
        }
        public override string ToString()
        {
            return ToString("attacked " + (isPrimary ? "opponent's":"its own") + " battery with " + damage + " damage");
        }
    }

    // OPEN ID AT 8

    public class Death: GameEvent
    {
        internal const byte EVENT_ID = 9;
        internal short returnHealth;
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(returnHealth);
        }
        public new static Death Deserialize(NetworkReader reader)
        {
            Death evt = new Death();
            evt.returnHealth = reader.ReadInt16();
            return evt;
        }
        public override string ToString()
        {
            return ToString("dies and returns to queue");
        }
    }

    public class Poison: GameEvent
    {
        internal const byte EVENT_ID = 10;
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
        }
        public new static Poison Deserialize(NetworkReader reader)
        {
            return new Poison();
        }
        public override string ToString()
        {
            return ToString("was poisoned");
        }
    }

    public class Damage : GameEvent
    {
        internal const byte EVENT_ID = 11;
        internal short damage;
        internal short remainingHealth;
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(damage);
            writer.Write(remainingHealth);
        }
        public new static Damage Deserialize(NetworkReader reader)
        {
            Damage evt = new Damage();
            evt.damage = reader.ReadInt16();
            evt.remainingHealth = reader.ReadInt16();
            return evt;
        }
        public override string ToString()
        {
            return ToString("was damaged " + damage + " health down to " + remainingHealth);
        }
    }

    public class Resolve : GameEvent
    {
        internal const byte EVENT_ID = 12;
        internal byte commandType;
        
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(commandType);
        }
        public new static Resolve Deserialize(NetworkReader reader)
        {
            Resolve evt = new Resolve();
            evt.commandType = reader.ReadByte();
            return evt;
        }
    }

    public class End : GameEvent
    {
        internal const byte EVENT_ID = 13;
        internal bool primaryLost;
        internal bool secondaryLost;
        internal short turnCount;
        internal int timeTaken;
        internal Dictionary<short, Game.RobotStat> primaryTeamStats;
        internal Dictionary<short, Game.RobotStat> secondaryTeamStats;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(primaryLost);
            writer.Write(secondaryLost);
            writer.Write(turnCount);
            writer.Write(timeTaken);
            writer.Write(primaryTeamStats.GetLength());
            primaryTeamStats.ForEach((k, stat) =>
            {
                writer.Write(k);
                stat.Serialize(writer);
            });
            writer.Write(secondaryTeamStats.GetLength());
            secondaryTeamStats.ForEach((k, stat) =>
            {
                writer.Write(k);
                stat.Serialize(writer);
            });


        }
        public new static End Deserialize(NetworkReader reader)
        {
            End evt = new End();
            evt.primaryLost = reader.ReadBoolean();
            evt.secondaryLost = reader.ReadBoolean();
            evt.turnCount = reader.ReadInt16();
            evt.timeTaken = reader.ReadInt32();
            evt.primaryTeamStats = new Dictionary<short, Game.RobotStat>(reader.ReadInt32());
            for (int i = 0; i < evt.primaryTeamStats.GetLength(); i++)
            {
                short k = reader.ReadInt16();
                Game.RobotStat stat = new Game.RobotStat();
                stat.Deserialize(reader);
                evt.primaryTeamStats.Add(k, stat);
            }
            evt.secondaryTeamStats = new Dictionary<short, Game.RobotStat>(reader.ReadInt32());
            for (int i = 0; i < evt.secondaryTeamStats.GetLength(); i++)
            {
                short k = reader.ReadInt16();
                Game.RobotStat stat = new Game.RobotStat();
                stat.Deserialize(reader);
                evt.secondaryTeamStats.Add(k, stat);
            }
            return evt;
        }
    }

    public class Collision : GameEvent
    {
        internal const byte EVENT_ID = 14;
        internal List<short> collidingRobots;
        internal Vector2Int deniedPos;
        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EVENT_ID);
            writer.Write(collidingRobots.GetLength());
            collidingRobots.ForEach(writer.Write);
            writer.Write(deniedPos.x);
            writer.Write(deniedPos.y);
        }
        public new static Collision Deserialize(NetworkReader reader)
        {
            Collision evt = new Collision();
            evt.collidingRobots = new List<short>();
            int length = reader.ReadInt32();
            for (int i = 0; i < length; i++)
            {
                evt.collidingRobots.Add(reader.ReadInt16());
            }
            evt.deniedPos = new Vector2Int();
            evt.deniedPos.x = reader.ReadInt32();
            evt.deniedPos.y = reader.ReadInt32();
            return evt;
        }
        public override string ToString()
        {
            return ToString("was blocked by " + string.Join(",",collidingRobots));
        }
    }
}
