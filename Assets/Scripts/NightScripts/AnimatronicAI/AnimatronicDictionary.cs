using UnityEngine;
using System;
using System.Collections.Generic;
using NaughtyAttributes;

public enum Direction
{
    Left,
    Right,
    Back,
    Other
}
[Serializable]
public class RuleDefinition
{
    public bool Allowed = true;
}
[Serializable]
public class AllowConnection : RuleDefinition // allows/disallows going through certain types of connection
{
    public List<ConnectionType> Connection;
}

[Serializable]
public class AllowBacktrack : RuleDefinition // allows/disallows going to the immediate previous node
{
    public AllowBacktrack()
    {
        Allowed = false;
    }
}

[Serializable]
public class AllowEnteringOccupiedRooms : RuleDefinition // allows/disallows going into rooms where one or more node has an animatronic in it
{}

[Serializable]
public class AllowCenterNavigate : RuleDefinition // allows/disallows the AI to try to navigate to the center of the map rather than the edges or certain sides
{}

[Serializable]
public class AllowCamMove : RuleDefinition // allows/disallows moving while cams are on them
{
    public AllowCamMove()
    {
        Allowed = false;
    }
}

[Serializable]
public class AllowWander : RuleDefinition // false = they take the shortest possible path directly to your office, true = they can wander (bonnie/chica vs freddy in FNaF 1)
{}

[Serializable]
public class AllowAwayMovement : RuleDefinition // allows/disallows moving away from the office if there are other options
{}

[Serializable]
public class AllowIndirectMovement : RuleDefinition // allows/disallows moving to not directly connected rooms
{
    public AllowIndirectMovement()
    {
        Allowed = false;
    }
}

[Serializable]
public class AllowOfficeEntry : RuleDefinition // allows/disallows moving into the office
{}

[Serializable]
public class AllowSameRoomMovement : RuleDefinition // allows/disallows moving between nodes in the same room
{
    
}

[Serializable]
public class AllowOfficeKill : RuleDefinition // allows/disallows them to kill you when they enter the office
{
    
}

[Serializable]
public class AllowDoorBlock : RuleDefinition // if a door stops them, true = it does, false = it dont
{
    
}

[Serializable]
public class AllowProceduralAnimatedMoving : RuleDefinition // actual moving (a NavMesh finding a path to the node as best as it can, animating it via procedural/kinematic animation) vs snapping
{}

[Serializable]
public class AllowMovementWhileMoving : RuleDefinition // if they can get another target to go towards while moving (if they use procedural movement)
{}

[Serializable]
public class AllowLightDetection : RuleDefinition // allows/disallows detecting the player through lights
{}

[Serializable]
public class AllowCameraDetection : RuleDefinition // allows/disallows detecting camera activity
{}

[Serializable]
public class AllowAudioDetection : RuleDefinition // allows/disallows detecting audio cues
{}

[Serializable]
public class
    AllowPlayerDetection : RuleDefinition // allows/disallows directly detecting the player's presence (gives verbose info about the player)
{}

[Serializable]
public class AllowPlayerActionInterruption : RuleDefinition // allows/disallows player actions interrupting current behavior
{}

[Serializable]
public class AllowOfficeAttack : RuleDefinition // allows/disallows initiating an attack from inside the office
{}

[Serializable]
public class AllowRemoteAttack : RuleDefinition // allows/disallows attacking without physically entering the office
{}

[Serializable]
public class AllowInstantKill : RuleDefinition // allows/disallows killing the player immediately when attack conditions are met
{}

[Serializable]
public class AllowAttackInterruption : RuleDefinition // allows/disallows interrupting an attack once it begins
{}

[Serializable]
public class AllowAttackCooldown : RuleDefinition // allows/disallows having a cooldown between attacks
{}

[Serializable]
public class AllowMultipleAttacks : RuleDefinition // allows/disallows attacking multiple times during one encounter
{}

[Serializable]
public class AllowAttackRetreat : RuleDefinition // allows/disallows retreating after reaching an attack position
{}

[Serializable]
public class AllowPlayerEscape : RuleDefinition // allows/disallows the player escaping after an encounter begins
{}

[Serializable]
public class AllowAwarenessChange : RuleDefinition // allows/disallows awareness changing based on player behavior/time
{}

[Serializable]
public class AllowAggressionChange : RuleDefinition // allows/disallows aggression changing over time
{}

[Serializable]
public class AllowBehaviorEscalation : RuleDefinition // allows/disallows entering more dangerous behavior states
{}

[Serializable]
public class AllowBehaviorDeescalation : RuleDefinition // allows/disallows returning to less dangerous behavior states
{}

[Serializable]
public class AllowPlayerHabitDetection : RuleDefinition // allows/disallows recognizing repeated player behavior
{}

[Serializable]
public class PreferDirection // prefers attacking the office from a certain direction
{
    public Direction Direction = Direction.Left;
    public float PreferWeight = 1;
}

[Serializable]
public class PreferFollowing // prefers following a certain Animatronic
{
    public AnimatronicAPI Follow;
    public float PreferWeight = 1;
}

[Serializable]
public class PreferConnection // prefers going through certain types of connections
{
    public ConnectionType Connection;
    public float PreferWeight = 1;
}

[Serializable]
public class Intelligence
{
    public bool KnowMap = true;
    public bool KnowAnimatronics = true; // whether they know the whole roster that is activated or not
    public bool KnowOffice = true; // whether they know what office the player is using
    public bool KnowUpgrades = true; // whether they know what office upgrades the player has (better flashlight, better doors, better battery, etc.)
    public bool ForgetAfterNight = false; // true = once the night ends, they have to relearn it all, false = they remember the map for every night after, even if the map changes
    public bool AntNavigation = false; // whether or not they navigate via the ant pheromone method or just via regular node preference strategies, not using the ant method
}

// misc. not Prefer or Allow or Enum or Type

[Serializable]
public class GeneralInfo
{
    public float MoveInterval = 5;
    public string DisplayName = "Example";
    public string ShopDescription = "Does Basic Movement";
    public bool Lethal = true;
}