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
public class AllowConnection // allows/disallows going through certain types of connection
{
    public ConnectionType Connection;
    public bool Allowed = true;
}

[Serializable]
public class AllowBacktrack // allows/disallows going to the immediate previous node
{
    public bool Allowed = false;
}

[Serializable]
public class AllowEnteringOccupiedRooms // allows/disallows going into rooms where one or more node has an animatronic in it
{
    public bool Allowed = true;
}

[Serializable]
public class AllowCenterNavigate // allows/disallows the AI to try to navigate to the center of the map rather than the edges or certain sides
{
    public bool Allowed = true;
}

[Serializable]
public class AllowCamMove // allows/disallows moving while cams are on them
{
    public bool Allowed = false;
}

[Serializable]
public class AllowWander // false = they take the shortest possible path directly to your office, true = they can wander (bonnie/chica vs freddy in FNaF 1)
{
    public bool Allowed = true;
}

[Serializable]
public class AllowAwayMovement // allows/disallows moving away from the office if there are other options
{
    public bool Allowed = true;
}

[Serializable]
public class AllowIndirectMovement // allows/disallows moving to not directly connected rooms
{
    public bool Allowed = false;
}

[Serializable]
public class AllowOfficeEntry // allows/disallows moving into the office
{
    public bool Allowed = true;
}
[Serializable]
public class AllowSameRoomMovement // allows/disallows moving between nodes in the same room
{
    public bool Allowed = false;
}

[Serializable]
public class AllowOfficeKill // allows/disallows them to kill you when they enter the office
{
    public bool Allowed = true;
}

[Serializable]
public class AllowDoorBlock // if a door stops them, true = it does, false = it dont
{
    public bool Allowed = true;
}

[Serializable]
public class AllowProceduralAnimatedMoving // actual moving (a NavMesh finding a path to the node as best as it can, animating it via procedural/kinematic animation) vs snapping
{
    public bool Allowed = false;
}

[Serializable]
public class AllowMovementWhileMoving // if they can get another target to go towards while moving (if they use procedural movement)
{
    public bool Allowed = true;
}

[Serializable]
public class AllowLightDetection // allows/disallows detecting the player through lights
{
    public bool Allowed = true;
}

[Serializable]
public class AllowCameraDetection // allows/disallows detecting camera activity
{
    public bool Allowed = true;
}

[Serializable]
public class AllowAudioDetection // allows/disallows detecting audio cues
{
    public bool Allowed = true;
}

[Serializable]
public class
    AllowPlayerDetection // allows/disallows directly detecting the player's presence (gives verbose info about the player)
{
    public bool Allowed = true;
}

[Serializable]
public class AllowPlayerActionInterruption // allows/disallows player actions interrupting current behavior
{
    public bool Allowed = true;
}

[Serializable]
public class AllowOfficeAttack // allows/disallows initiating an attack from inside the office
{
    public bool Allowed = true;
}

[Serializable]
public class AllowRemoteAttack // allows/disallows attacking without physically entering the office
{
    public bool Allowed = false;
}

[Serializable]
public class AllowInstantKill // allows/disallows killing the player immediately when attack conditions are met
{
    public bool Allowed = true;
}

[Serializable]
public class AllowAttackInterruption // allows/disallows interrupting an attack once it begins
{
    public bool Allowed = true;
}

[Serializable]
public class AllowAttackCooldown // allows/disallows having a cooldown between attacks
{
    public bool Allowed = true;
}

[Serializable]
public class AllowMultipleAttacks // allows/disallows attacking multiple times during one encounter
{
    public bool Allowed = true;
}

[Serializable]
public class AllowAttackRetreat // allows/disallows retreating after reaching an attack position
{
    public bool Allowed = true;
}

[Serializable]
public class AllowPlayerEscape // allows/disallows the player escaping after an encounter begins
{
    public bool Allowed = true;
}

[Serializable]
public class AllowAwarenessChange // allows/disallows awareness changing based on player behavior/time
{
    public bool Allowed = true;
}

[Serializable]
public class AllowAggressionChange // allows/disallows aggression changing over time
{
    public bool Allowed = true;
}

[Serializable]
public class AllowBehaviorEscalation // allows/disallows entering more dangerous behavior states
{
    public bool Allowed = true;
}

[Serializable]
public class AllowBehaviorDeescalation // allows/disallows returning to less dangerous behavior states
{
    public bool Allowed = true;
}

[Serializable]
public class AllowPlayerHabitDetection // allows/disallows recognizing repeated player behavior
{
    public bool Allowed = true;
}

[Serializable]
public class PreferDirection // prefers attacking the office from a certain direction
{
    public Direction Direction = Direction.Left;
    public float PreferWeight = 1;
}

[Serializable]
public class PreferFollowing // prefers following a certain Animatronic
{
    public Animatronic Follow;
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