
using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[CreateAssetMenu(
    fileName = "New Animatronic Personality",
    menuName = "FazTech/Animatronic Personality"
)]
public class AnimatronicPersonality : ScriptableObject
{
    // GENERAL INFORMATION

    [BoxGroup("General Information")]
    public GeneralInfo GeneralInfo = new GeneralInfo();


    // RULES

    [BoxGroup("Rules/Movement")]
    public AllowConnection AllowConnection = new AllowConnection();

    [BoxGroup("Rules/Movement")]
    public AllowBacktrack AllowBacktrack = new AllowBacktrack();

    [BoxGroup("Rules/Movement")]
    public AllowEnteringOccupiedRooms AllowEnteringOccupiedRooms = new AllowEnteringOccupiedRooms();

    [BoxGroup("Rules/Movement")]
    public AllowCenterNavigate AllowCenterNavigate = new AllowCenterNavigate();

    [BoxGroup("Rules/Movement")]
    public AllowCamMove AllowCamMove = new AllowCamMove();

    [BoxGroup("Rules/Movement")]
    public AllowWander AllowWander = new AllowWander();

    [BoxGroup("Rules/Movement")]
    public AllowAwayMovement AllowAwayMovement = new AllowAwayMovement();

    [BoxGroup("Rules/Movement")]
    public AllowIndirectMovement AllowIndirectMovement = new AllowIndirectMovement();

    [BoxGroup("Rules/Movement")]
    public AllowSameRoomMovement AllowSameRoomMovement = new AllowSameRoomMovement();

    [BoxGroup("Rules/Office")]
    public AllowOfficeEntry AllowOfficeEntry = new AllowOfficeEntry();

    [BoxGroup("Rules/Office")]
    public AllowOfficeKill AllowOfficeKill = new AllowOfficeKill();

    [BoxGroup("Rules/Office")]
    public AllowDoorBlock AllowDoorBlock = new AllowDoorBlock();

    [BoxGroup("Rules/Office")]
    public AllowOfficeAttack AllowOfficeAttack = new AllowOfficeAttack();

    [BoxGroup("Rules/Movement")]
    public AllowProceduralAnimatedMoving AllowProceduralAnimatedMoving = new AllowProceduralAnimatedMoving();

    [BoxGroup("Rules/Movement")]
    public AllowMovementWhileMoving AllowMovementWhileMoving = new AllowMovementWhileMoving();

    [BoxGroup("Rules/Detection")]
    public AllowLightDetection AllowLightDetection = new AllowLightDetection();

    [BoxGroup("Rules/Detection")]
    public AllowCameraDetection AllowCameraDetection = new AllowCameraDetection();

    [BoxGroup("Rules/Detection")]
    public AllowAudioDetection AllowAudioDetection = new AllowAudioDetection();

    [BoxGroup("Rules/Detection")]
    public AllowPlayerDetection AllowPlayerDetection = new AllowPlayerDetection();

    [BoxGroup("Rules/Detection")]
    public AllowPlayerActionInterruption AllowPlayerActionInterruption = new AllowPlayerActionInterruption();

    [BoxGroup("Rules/Attacks")]
    public AllowRemoteAttack AllowRemoteAttack = new AllowRemoteAttack();

    [BoxGroup("Rules/Attacks")]
    public AllowInstantKill AllowInstantKill = new AllowInstantKill();

    [BoxGroup("Rules/Attacks")]
    public AllowAttackInterruption AllowAttackInterruption = new AllowAttackInterruption();

    [BoxGroup("Rules/Attacks")]
    public AllowAttackCooldown AllowAttackCooldown = new AllowAttackCooldown();

    [BoxGroup("Rules/Attacks")]
    public AllowMultipleAttacks AllowMultipleAttacks = new AllowMultipleAttacks();

    [BoxGroup("Rules/Attacks")]
    public AllowAttackRetreat AllowAttackRetreat = new AllowAttackRetreat();

    [BoxGroup("Rules/Attacks")]
    public AllowPlayerEscape AllowPlayerEscape = new AllowPlayerEscape();

    [BoxGroup("Rules/Behavior")]
    public AllowAwarenessChange AllowAwarenessChange = new AllowAwarenessChange();

    [BoxGroup("Rules/Behavior")]
    public AllowAggressionChange AllowAggressionChange = new AllowAggressionChange();

    [BoxGroup("Rules/Behavior")]
    public AllowBehaviorEscalation AllowBehaviorEscalation = new AllowBehaviorEscalation();

    [BoxGroup("Rules/Behavior")]
    public AllowBehaviorDeescalation AllowBehaviorDeescalation = new AllowBehaviorDeescalation();

    [BoxGroup("Rules/Behavior")]
    public AllowPlayerHabitDetection AllowPlayerHabitDetection = new AllowPlayerHabitDetection();


    // PREFERENCES

    [BoxGroup("Preferences")]
    public List<PreferDirection> PreferDirection = new List<PreferDirection>();

    [BoxGroup("Preferences")]
    public List<PreferFollowing> PreferFollowing = new List<PreferFollowing>();

    [BoxGroup("Preferences")]
    public List<PreferConnection> PreferConnection = new List<PreferConnection>();


    // INTELLIGENCE

    [BoxGroup("Intelligence")]
    public Intelligence Intelligence = new Intelligence();
}
