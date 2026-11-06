using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public abstract class AnimatronicAPI : MonoBehaviour
{
    [Header("Personality")]
    [SerializeField] protected AnimatronicPersonality Personality;
    [Range(0, 20)]
    [SerializeField] protected int AILevel = 5;
    [SerializeField] protected bool AutoMove = false;
    [SerializeField] private float MoveTimer;
    [SerializeField, ReadOnly] protected NodeInfo CurrentNode;
    [SerializeField, ReadOnly] protected NodeInfo PreviousNode;
    protected float timerInterval = 1f / 60f;
    
    #region On

        public UnityEvent OnKill;
        public UnityEvent OnMove;
        public UnityEvent OnReset;

    #endregion

    #region Do

    public void DoToggleAutoMove()
    {
        AutoMove = !AutoMove;
    }
    
    public void DoStopMovement()
    {
        AutoMove = false;
    }
    
    public void DoStartMovement()
    {
        AutoMove = true;
    }
    
    public void DoResetMoveTimer()
    {
        MoveTimer =Personality.GeneralInfo.MoveInterval;
    }

    public void DoKill()
    {
        OnKill.Invoke();
    }

    public void DoMoveEvent()
    {
        OnMove.Invoke();
        // later when we set up actual movement, we can rename it DoMove and add logic or just keep it like this
    }

    public void DoResetEvent()
    {
        OnReset.Invoke();
        // same with DoMoveEvent, and also DoReset should be triggered when they get resetted like Freddy getting sent back to the stage when he bonks the door
    }

    public bool DoAttemptMove()
    {
        if (AILevel <= 0)
            return false;

        int Roll = Random.Range(1, 21);

        if (Roll > AILevel)
            return false;
        return true;
    }

    public void DoUpdatePos(Vector3 Position, Transform MovingObject)
    {
        MovingObject.position = Position;
    }

    public void DoUpdateRotation(Quaternion Rotation, Transform MovingObject)
    {
        MovingObject.rotation = Rotation;
    }

    public void DoChangeParent(GameObject Parent, GameObject Target)
    {
        Target.transform.parent = Parent.transform;
    }

    public void DoMoveTowards(Pose Position, Transform MovingObject)
    {
        DoUpdatePos(Position.position, MovingObject);
        DoUpdateRotation(Position.rotation, MovingObject);
    }

    public void DoMoveToNode(GameObject Node, GameObject Target)
    {
        if (DoUpdateNode(Node.GetComponent<NodeInfo>(), Target.GetComponentInParent<NodeInfo>(),
                Target.GetComponent<AnimatronicAPI>()))
        {
            DoChangeParent(Node, Target);
            DoMoveTowards(new Pose(Node.transform.position, Node.transform.rotation), Target.transform);
            DoMoveEvent();
        }
        else
        {
            Debug.LogWarning("Node couldn't update!");
        }
    }

    public bool DoUpdateNode(NodeInfo Destination, NodeInfo Current, AnimatronicAPI animatronic)
    {
        if (Current.OccupyingAnimatronic == animatronic && Destination.OccupyingAnimatronic == null)
        {
            PreviousNode = Current;
            CurrentNode = Destination;
            
            Destination.OccupyingAnimatronic = animatronic;
            Current.OccupyingAnimatronic = null;
            return true;
        }
        if (Current.OccupyingAnimatronic == null && Destination.OccupyingAnimatronic == null)
        {
            Debug.LogWarning("Current did not assign correctly!");
            PreviousNode = Current;
            CurrentNode = Destination;
            Current.OccupyingAnimatronic = null;
            Destination.OccupyingAnimatronic = animatronic;
            return true;
        }
        if (Current.OccupyingAnimatronic == animatronic && Destination.OccupyingAnimatronic != null && Destination.OccupyingAnimatronic != animatronic)
        {
            Debug.LogWarning("Destination is occupied!");
            return false;
        }
        if (Destination.OccupyingAnimatronic == animatronic)
        {
            Debug.LogWarning("Already at destination!");
            PreviousNode = Current;
            CurrentNode = Destination;
            return true;
        }

        return false;
    }

    public float DoTryNode(NodeInfo Node, NodeInfo CurrentNode, AnimatronicPersonality personality)
    {
        float num = 0;
        if (Node == null || CurrentNode == null)
            return -1f;

        ConnectedNode Connection = CurrentNode.ConnectedNodes
            .FirstOrDefault(cn => cn.Node == Node.gameObject);

        if (Connection == null || !personality.AllowConnection.Allowed ||
            !personality.AllowConnection.Connection.Contains(Connection.Connection))
            return -1f;

        if (personality.AllowEnteringOccupiedRooms.Allowed == false && Node.gameObject.transform.parent.GetComponentsInChildren<NodeInfo>().Any(r => r.OccupyingAnimatronic != null &&
                r.OccupyingAnimatronic != this))
            return -1f;
        
        if (!personality.AllowBacktrack.Allowed && Node == PreviousNode)
            return -1f;
        
        if (!personality.AllowSameRoomMovement.Allowed &&
            Node.transform.parent == CurrentNode.transform.parent)
            return -1f;

        if (!personality.AllowOfficeEntry.Allowed && Node.transform.parent.name.Contains("Office"))
            return -1f;

        PreferConnection preference = personality.PreferConnection
            .Find(n => n.Connection == Connection.Connection);

        if (preference != null)
            num += preference.PreferWeight;
        
        return num;
    }

    #endregion

    #region Set

        public void SetAILevel(int Level)
        {
            AILevel = Mathf.Clamp(Level, 0, 20);
        }
        
        public void SetAutoMove(bool Enabled)
        {
            AutoMove = Enabled;
        }
        
        public void SetMoveInterval(string Seconds)
        {
            if (float.TryParse(Seconds, out float parsedFloat))
            {
                Personality.GeneralInfo.MoveInterval = Mathf.Max(0.1f, parsedFloat);
            }
        }
        
        public void SetMoveTimer(float Seconds)
        {
            MoveTimer = Mathf.Max(0f, Seconds);
        }
        
        public void SetActive(bool Active, GameObject Target)
        {
            Target.SetActive(Active);
        }

    #endregion

    #region Get

        public bool GetAllowConnection(ConnectionType Connection) => Personality.AllowConnection.Allowed && Personality.AllowConnection.Connection.Contains(Connection);
        public int GetAILevel() => AILevel;
        public float GetMoveInterval() => Personality.GeneralInfo.MoveInterval;
        public bool GetAutoMove() => AutoMove;
        public float GetMoveTimer() => MoveTimer;
        public bool GetAllowConnection() => Personality.AllowConnection.Allowed;
        public List<ConnectionType> GetAllowConnectionType() => Personality.AllowConnection.Connection;
        public bool GetAllowBacktrack() => Personality.AllowBacktrack.Allowed;
        public bool GetAllowEnteringOccupiedRooms() => Personality.AllowEnteringOccupiedRooms.Allowed;
        public bool GetAllowCenterNavigate() => Personality.AllowCenterNavigate.Allowed;
        public bool GetAllowCamMove() => Personality.AllowCamMove.Allowed;
        public bool GetAllowWander() => Personality.AllowWander.Allowed; 
        public bool GetAllowAwayMovement() => Personality.AllowAwayMovement.Allowed; 
        public bool GetAllowIndirectMovement() => Personality.AllowIndirectMovement.Allowed; 
        public bool GetAllowOfficeEntry() => Personality.AllowOfficeEntry.Allowed; 
        public bool GetAllowSameRoomMovement() => Personality.AllowSameRoomMovement.Allowed; 
        public bool GetAllowOfficeKill() => Personality.AllowOfficeKill.Allowed; 
        public bool GetAllowDoorBlock() => Personality.AllowDoorBlock.Allowed; 
        public bool GetAllowProceduralAnimatedMoving() => Personality.AllowProceduralAnimatedMoving.Allowed; 
        public bool GetAllowMovementWhileMoving() => Personality.AllowMovementWhileMoving.Allowed; 
        public bool GetAllowLightDetection() => Personality.AllowLightDetection.Allowed; 
        public bool GetAllowCameraDetection() => Personality.AllowCameraDetection.Allowed; 
        public bool GetAllowAudioDetection() => Personality.AllowAudioDetection.Allowed; 
        public bool GetAllowPlayerDetection() => Personality.AllowPlayerDetection.Allowed; 
        public bool GetAllowPlayerActionInterruption() => Personality.AllowPlayerActionInterruption.Allowed; 
        public bool GetAllowOfficeAttack() => Personality.AllowOfficeAttack.Allowed; 
        public bool GetAllowRemoteAttack() => Personality.AllowRemoteAttack.Allowed; 
        public bool GetAllowInstantKill() => Personality.AllowInstantKill.Allowed; 
        public bool GetAllowAttackInterruption() => Personality.AllowAttackInterruption.Allowed; 
        public bool GetAllowAttackCooldown() => Personality.AllowAttackCooldown.Allowed; 
        public bool GetAllowMultipleAttacks() => Personality.AllowMultipleAttacks.Allowed; 
        public bool GetAllowAttackRetreat() => Personality.AllowAttackRetreat.Allowed;
        public bool GetAllowPlayerEscape() => Personality.AllowPlayerEscape.Allowed; 
        public bool GetAllowAwarenessChange() => Personality.AllowAwarenessChange.Allowed; 
        public bool GetAllowAggressionChange() => Personality.AllowAggressionChange.Allowed; 
        public bool GetAllowBehaviorEscalation() => Personality.AllowBehaviorEscalation.Allowed; 
        public bool GetAllowBehaviorDeescalation() => Personality.AllowBehaviorDeescalation.Allowed; 
        public bool GetAllowPlayerHabitDetection() => Personality.AllowPlayerHabitDetection.Allowed;
        
        public bool GetKnowMap() => Personality.Intelligence.KnowMap;
        public bool GetKnowAnimatronics() => Personality.Intelligence.KnowAnimatronics;
        public bool GetKnowOffice() => Personality.Intelligence.KnowOffice;
        public bool GetKnowUpgrades() => Personality.Intelligence.KnowUpgrades;
        public bool GetForgetAfterNight() => Personality.Intelligence.ForgetAfterNight;
        public bool GetAntNavigation() => Personality.Intelligence.AntNavigation;

    #endregion

    #region Misc.

    public virtual void InitializeAI()
    {

    }

    #endregion

}