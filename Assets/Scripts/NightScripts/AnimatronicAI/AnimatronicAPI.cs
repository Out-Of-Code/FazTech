using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public abstract class AnimatronicAPI : MonoBehaviour
{
    [Range(0, 20)]
    [SerializeField] protected int AILevel = 5;
    [SerializeField] protected bool AutoMove = false;
    [SerializeField] protected float MoveInterval = 5f;
    [SerializeField] private float MoveTimer;
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
        MoveTimer = MoveInterval;
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

    public void DoMoveTowards(Pose Position, Transform MovingObject)
    {
        DoUpdatePos(Position.position, MovingObject);
        DoUpdateRotation(Position.rotation, MovingObject);
    }

    public bool DoUpdateNode(NodeInfo Destination, NodeInfo Current, AnimatronicAPI animatronic)
    {
        if (Current.OccupyingAnimatronic == animatronic && Destination.OccupyingAnimatronic == null)
        {
            Destination.OccupyingAnimatronic = animatronic;
            Current.OccupyingAnimatronic = null;
            return true;
        }
        if (Current.OccupyingAnimatronic == null && Destination.OccupyingAnimatronic == null)
        {
            Debug.LogWarning("Current did not assign correctly!");
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
            return true;
        }

        return false;
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
                MoveInterval = Mathf.Max(0.1f, parsedFloat);
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

        public int GetAILevel()
        {
            return AILevel;
        }

        public float GetMoveInterval()
        {
            return MoveInterval;
        }

        public bool GetAutoMove()
        {
            return AutoMove;
        }

        public float GetMoveTimer()
        {
            return MoveTimer;
        }
    

    #endregion

    #region Misc.

    public virtual void InitializeAI()
    {

    }

    #endregion

}