using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class LegacyAnimatronic : MonoBehaviour
{
    [Header("AI Settings")]
    [Range(0, 20)]
    [SerializeField] private int AILevel = 5;
        [SerializeField] private AnimatronicAiType AIType =
            AnimatronicAiType.Freddy;
    
    [Header("Office Doors")]
    [Required] [SerializeField] private DoorController LeftDoor;
    [Required] [SerializeField] private DoorController RightDoor;
    
    [Header("Controllers")]
    [Required] [SerializeField] private NightManager Night;
    
    [HorizontalLine(color: EColor.Green, height: 5)]
    
    [Header("Animatronic Setup")]
    [Required] [Foldout("FREDDY/BONNIE/CHICA")] [SerializeField] private GameObject FreddyCubeObject;
    [Foldout("FREDDY/BONNIE/CHICA")] [SerializeField] private NodeInfo StartingNode;
    [Foldout("FREDDY/BONNIE/CHICA")] [SerializeField] private NodeInfo StageNode;
    
    [Header("Movement Settings")]
    
    [Foldout("FREDDY/BONNIE/CHICA")] [SerializeField] private bool AutoMove = false;
    [Foldout("FREDDY/BONNIE/CHICA")] [SerializeField] private float MoveInterval = 5f;
    
    
    
    [Foldout("FOXY")] public GameObject[] FoxyCurtainStates;
    [Foldout("FOXY")] [SerializeField] private GameObject FoxyCamera; // Pirate Cove
    [Foldout("FOXY")] [SerializeField] private GameObject LeftHallCamera;
    
    [Header("Foxy Progression")]
    [Foldout("FOXY")] [SerializeField] private int Stage = 1;
    [Foldout("FOXY")] [SerializeField, ReadOnly] private int H = 0;
    [Foldout("FOXY")] [SerializeField, ReadOnly] private int Points = 0;
    [Foldout("FOXY")] private float HallwayGraceTimer = 25f;
    [Foldout("FOXY")] [SerializeField, ReadOnly] private bool FoxyRushing = false;
    [Foldout("FOXY")] [SerializeField, ReadOnly] private float RushTimer = 1.7f;
    
    
    private NodeInfo CurrentNode;
    private NodeInfo PreviousNode;
    [ProgressBar("Time Left Till Move", "MoveInterval", EColor.Red)] [SerializeField] private float MoveTimer;
    private bool PlayerDead = false;
    private float timerInterval = 1f / 60f;
    private float timeAccumulator = 0f;
    [FormerlySerializedAs("OnFreddyKill")] public UnityEvent OnKill;
    

    void Start()
    {
        UpdateFoxyCurtains(Stage);
        if (Night != null)
        {
            SetAILevel(Night.GetAILevel());
        }
        if ((AIType == AnimatronicAiType.Bonnie || AIType == AnimatronicAiType.Freddy || AIType == AnimatronicAiType.Chica) &&
            (StartingNode == null || FreddyCubeObject == null))
        {
            Debug.LogError($"{AIType} is missing his starting node or model!");
            enabled = false;
            return;
        }

        if (AIType == AnimatronicAiType.Bonnie || AIType == AnimatronicAiType.Freddy || AIType == AnimatronicAiType.Chica)
        {
            CurrentNode = StartingNode;
            //CurrentNode.OccupyingAnimatronic = this;
            MoveToNode(CurrentNode);
            MoveTimer = MoveInterval;
        }
    }

    void Update()
    {
        if (AIType == AnimatronicAiType.Foxy)
        {
            bool WatchingFoxyCamera =
                FoxyCamera != null && FoxyCamera.activeInHierarchy;

            timeAccumulator += Time.deltaTime;

            while (timeAccumulator >= timerInterval)
            {
                timeAccumulator -= timerInterval;

                if (WatchingFoxyCamera)
                {
                    H = 0;
                    continue;
                }

                if (Stage >= 4)
                    continue;

                Points += 1 + H;

                int Threshold = 1000 - (AILevel * 10);

                if (Points >= Threshold)
                {
                    Points = 0;

                    if (Random.Range(0, 3) < 2)
                    {
                        Stage++;
                        H++;

                        UpdateFoxyCurtains(Stage);

                        Debug.Log($"Foxy advanced to stage {Stage}");
                    }
                }
            }

            if (Stage >= 4 && !FoxyRushing)
            {
                HallwayGraceTimer -= Time.deltaTime;

                bool WatchingLeftHall =
                    LeftHallCamera != null &&
                    LeftHallCamera.activeInHierarchy;

                if (WatchingLeftHall || HallwayGraceTimer <= 0f)
                {
                    FoxyRushing = true;
                    RushTimer = 1.7f;
                    Debug.Log("FOXY IS RUSHING!");
                }
            }
            if (FoxyRushing && !PlayerDead)
            {
                RushTimer -= Time.deltaTime;

                if (RushTimer <= 0f)
                {
                    if (LeftDoor.IsClosed)
                    {
                        Debug.Log("Foxy was blocked!");

                        Stage = Random.Range(0, 2) == 0 ? 1 : 2;

                        H = 0;
                        Points = 0;
                        HallwayGraceTimer = 25f;
                        RushTimer = 1.7f;
                        FoxyRushing = false;

                        UpdateFoxyCurtains(Stage);

                        Debug.Log($"Foxy returned to Stage {Stage}");
                    }
                    else
                    {
                        PlayerDead = true;
                        Debug.Log("FOXY KILLED THE PLAYER!");
                        OnKill.Invoke();
                    }
                }
            }
            return;
        }

        if (!AutoMove)
            return;

        MoveTimer -= Time.deltaTime;

        if (MoveTimer <= 0f)
        {
            FreddyMoveEvent();
            MoveTimer = MoveInterval;
        }
    }
    #region Freddy / Bonnie / Chica
    public void FreddyMoveEvent()
    {
        if (AILevel <= 0)
            return;
        if (CurrentNode == null || CurrentNode.ConnectedNodes == null || PlayerDead)
            return;
        int Roll = Random.Range(1, 21);

        Debug.Log($"{AIType} rolled " + Roll +
                  " against AI level " + AILevel);

        if (Roll > AILevel)
        {
            Debug.Log($"{AIType} did not move.");
            return;
        }
        List<NodeInfo> PossibleNodes = new List<NodeInfo>();

        foreach (ConnectedNode Connection in CurrentNode.ConnectedNodes)
        {
            if (Connection.Node == null)
                continue;

            //if (Connection.AI_Type_Allowed == null ||
            //    !System.Array.Exists(
            //        Connection.AI_Type_Allowed,
           //         type => type == AIType))
            //{
           //     continue;
            //}

            NodeInfo Destination =
                Connection.Node.GetComponent<NodeInfo>();

            if (Destination != null)
            {
                bool IsBlocked =
                    (CurrentNode.name == "Freddy_Left_Door" &&
                     Destination.name == "Freddy_Left_Door_Enter_Point" &&
                     LeftDoor.IsClosed)
                    ||
                    (CurrentNode.name == "Freddy_Right_Door" &&
                     Destination.name == "Freddy_Right_Door_Enter_Point" &&
                     RightDoor.IsClosed);

                if (!IsBlocked &&
                    (Destination.OccupyingAnimatronic == null ||
                     Destination.OccupyingAnimatronic == this))
                {
                    PossibleNodes.Add(Destination);
                }
            }
        }
        if ((CurrentNode.name == "Freddy_Left_Door" && LeftDoor.IsClosed &&
             HasConnectionTo("Freddy_Left_Door_Enter_Point")) ||
            (CurrentNode.name == "Freddy_Right_Door" && RightDoor.IsClosed &&
             HasConnectionTo("Freddy_Right_Door_Enter_Point")))
        {
            Debug.Log($"BONK! {AIType} got sent back to the stage.");
            PreviousNode = null;
            CurrentNode.OccupyingAnimatronic = null;
            CurrentNode = StageNode;
            //StageNode.OccupyingAnimatronic = this;
            MoveToNode(CurrentNode);
            return;
        }

        // Avoid immediately returning to the previous node
        // when another destination is available.
        if (PossibleNodes.Count > 1 && PreviousNode != null)
        {
            PossibleNodes.Remove(PreviousNode);
        }

        if (PossibleNodes.Count == 0)
        {
            Debug.Log($"{AIType} has nowhere to move from " +
                      CurrentNode.name);
            return;
        }

        int RandomIndex = Random.Range(0, PossibleNodes.Count);
        NodeInfo NextNode = PossibleNodes[RandomIndex];
        PreviousNode = CurrentNode;
        PreviousNode.OccupyingAnimatronic = null;

        CurrentNode = NextNode;
        //CurrentNode.OccupyingAnimatronic = this;

        MoveToNode(CurrentNode);

        if (CurrentNode.name == "Freddy_Left_Door_Enter_Point" ||
            CurrentNode.name == "Freddy_Right_Door_Enter_Point")
        {
            PlayerDead = true;
            AutoMove = false;
            Debug.Log($"YOU DIED! {AIType} entered the office.");
            OnKill.Invoke();
            return;
        }

        Debug.Log($"{AIType} moved to " + CurrentNode.name);
    }

    private void MoveToNode(NodeInfo Node)
    {
        FreddyCubeObject.transform.SetParent(null);

        FreddyCubeObject.transform.position = Node.transform.position;
        FreddyCubeObject.transform.rotation = Node.transform.rotation;

        FreddyCubeObject.transform.SetParent(Node.transform);
    }
    private bool HasConnectionTo(string NodeName)
    {
        foreach (ConnectedNode Connection in CurrentNode.ConnectedNodes)
        {
            //if (Connection.Node != null &&
            //    Connection.Node.name == NodeName &&
            //    Connection.AI_Type_Allowed != null &&
             //   System.Array.Exists(
             //       Connection.AI_Type_Allowed,
            //        type => type == AIType))
           // {
          //      return true;
          //  }
        }

        return false;
    }
    #endregion
    #region Helpers
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
    public void StopMovement()
    {
        AutoMove = false;
    }
    
    public void SetAILevel(int Level)
    {
        AILevel = Mathf.Clamp(Level, 0, 20);
    }
        public void ToggleAutoMove()
        {
            AutoMove = !AutoMove;
        }
    
        public void ToggleMoveInterval(float time)
        {
            MoveTimer = time;
        }
    #endregion
    #region Foxy
    public void UpdateFoxyCurtains(int stage)
    {
        if (FoxyCurtainStates == null)
            return;
        for (int i = 0; i < FoxyCurtainStates.Length; i++)
        {
            if (FoxyCurtainStates[i] != null)
                FoxyCurtainStates[i].SetActive(i == stage-1);
        }
    }
    #endregion

}