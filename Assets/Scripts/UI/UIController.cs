﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class UIController : MonoBehaviour {

	public TextMesh ScoreModel;

    public TMP_Text opponentNameText;
    internal TextMesh opponentScore;
    public GameObject OpponentsRobots;

    public TMP_Text userNameText;
    internal TextMesh userScore;
    public GameObject UsersRobots;
    public GameObject RobotPanel;
    public CommandSlotController CommandSlot;
    public GameObject priorityArrow;

    public Button OpponentSubmit;
    public Button SubmitCommands;
    public Button StepBackButton;
    public Button StepForwardButton;
    public Button BackToPresent;
    public Text EventTitle;
    
    public Sprite[] sprites;
    public Camera boardCamera;

    private static Color NO_COMMAND = new Color(0.25f, 0.25f, 0.25f);
    private static Color HIGHLIGHTED_COMMAND = new Color(0.5f, 0.5f, 0.5f);
    private static Color SUBMITTED_COMMAND = new Color(0.75f, 0.75f, 0.75f);
    private static Color OPEN_COMMAND = new Color(1, 1, 1);
    private Dictionary<short, GameObject> robotIdToPanel = new Dictionary<short, GameObject>();

    void Start()
    {
        Interpreter.InitializeUI(this);
    }

    void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                SceneManager.LoadScene("Initial");
            }
        }
    }

    //Loads the UICanvas and it's child components
    public void InitializeUICanvas(Game.Player[] playerObjects, bool isPrimary)
    {
        SetOpponentPlayerPanel(playerObjects[1]);
        SetUsersPlayerPanel(playerObjects[0]);

        if (GameConstants.LOCAL_MODE) {
            OpponentSubmit.gameObject.SetActive(true);
            OpponentSubmit.onClick.AddListener(Interpreter.SubmitActions);
        }
        SubmitCommands.onClick.AddListener(Interpreter.SubmitActions);
        BackToPresent.onClick.AddListener(Interpreter.BackToPresent);
        StepBackButton.onClick.AddListener(Interpreter.StepBackward);
        StepForwardButton.onClick.AddListener(Interpreter.StepForward);

        userScore = Instantiate(ScoreModel, Interpreter.boardController.GetVoidTile(isPrimary).transform);
        userScore.GetComponent<MeshRenderer>().sortingOrder = 1;
        opponentScore = Instantiate(ScoreModel, Interpreter.boardController.GetVoidTile(!isPrimary).transform);
        opponentScore.GetComponent<MeshRenderer>().sortingOrder = 1;
        SetBattery(playerObjects[0].battery, playerObjects[1].battery);
        PositionCamera(isPrimary);
    }

    void SetOpponentPlayerPanel(Game.Player opponentPlayer)
    {
        opponentNameText.SetText(opponentPlayer.name);
        for (int i = 0; i < opponentPlayer.team.Length; i++)
        {
            SetRobotPanel(opponentPlayer.team[i], OpponentsRobots.transform);
        }
    }

    void SetUsersPlayerPanel(Game.Player userPlayer)
    {
        userNameText.SetText(userPlayer.name);
        for (int i = 0; i < userPlayer.team.Length; i++)
        {
            robotIdToPanel[userPlayer.team[i].id] = SetRobotPanel(userPlayer.team[i], UsersRobots.transform);
        }
    }

    public GameObject SetRobotPanel(Robot r, Transform parent)
    {
        GameObject panel = Instantiate(RobotPanel, parent);
        panel.name = "Robot" + r.id;
        Transform icon = panel.transform.GetChild(1);
        icon.GetComponentInChildren<Image>().sprite = Array.Find(sprites, (Sprite s) => s.name.Equals(r.name));
        TMP_Text[] fields = panel.GetComponentsInChildren<TMP_Text>();
        fields[0].SetText(r.name);
        fields[1].SetText(r.description);
        AddCommandSlots(panel.transform.GetChild(3), r.id, r.priority);
        robotIdToPanel[r.id] = panel;
        return panel;
    }

    private void AddCommandSlots(Transform panel, short id, byte p)
    {
        int minI = 0;
        for (int i = GameConstants.MAX_PRIORITY; i > 0; i--)
        {
            CommandSlotController cmd = Instantiate(CommandSlot, panel);
            if (i > p)
            {
                cmd.RobotImage.color = NO_COMMAND;
            }
            else if (minI == 0) minI = i;
            cmd.deletable = false;
            SetOnClickClear(cmd.Delete, id, i, minI);
        }
    }

    private void SetOnClickClear(Button b, short id, int i, int minI)
    {
        b.onClick.AddListener(() =>
        {
            ClearCommands(b.transform.parent.parent);
            Interpreter.DeleteCommand(id, minI - i);
        });
    }

    public void SetPriority(int priority)
    {
        if (priority == -1)
        {
            priorityArrow.SetActive(false);
            return;
        }
        else if (priority == 0)
        {
            return;
        }
        int pos = GameConstants.MAX_PRIORITY - priority;
        Transform lastRobotPanal = UsersRobots.transform.GetChild(UsersRobots.transform.childCount - 1);
        RectTransform anchor = lastRobotPanal.GetChild(3).GetChild(pos).GetComponent<RectTransform>();
        RectTransform arrowRect = priorityArrow.GetComponent<RectTransform>();
        arrowRect.sizeDelta = new Vector2(anchor.rect.width, anchor.rect.height);
        arrowRect.position = anchor.position;
        Vector2 translation = new Vector2(anchor.rect.width + 10, 0);
        arrowRect.anchoredPosition += translation;
        priorityArrow.SetActive(true);
    }

    public void ClearCommands(Transform panel)
    {
        for (int i = 0; i < panel.childCount; i++)
        {
            CommandSlotController child = panel.GetChild(i).GetComponent<CommandSlotController>();
            child.deletable = false;
            child.RobotImage.sprite = null;
            if (!child.RobotImage.color.Equals(NO_COMMAND))
            {
                child.RobotImage.color = OPEN_COMMAND;
                child.RobotImage.rectTransform.rotation = Quaternion.Euler(Vector3.zero);
            }
        }
    }

    public void ClearCommands(short id)
    {
        ClearCommands(robotIdToPanel[id].transform.GetChild(3));
    }

    public void HighlightCommands(Type t, byte p)
    {
        foreach (short id in robotIdToPanel.Keys)
        {
            Transform commandPanel = robotIdToPanel[id].transform.GetChild(3);
            if (commandPanel.childCount - p < 0) continue;
            CommandSlotController cmd = commandPanel.GetChild(commandPanel.childCount - p).GetComponent<CommandSlotController>();
            if (cmd.RobotImage.sprite != null && cmd.RobotImage.sprite.name.StartsWith(t.ToString().Substring("Command.".Length)))
            {
                cmd.RobotImage.color = HIGHLIGHTED_COMMAND;
            }
        }
    }

    public void ColorCommandsSubmitted(short id)
    {
        Transform panel = robotIdToPanel[id].transform.GetChild(3);
        for (int i = 0; i < panel.childCount; i++)
        {
            CommandSlotController cmd = panel.GetChild(i).GetComponent<CommandSlotController>();
            if (cmd.RobotImage.color.Equals(OPEN_COMMAND) || cmd.RobotImage.color.Equals(HIGHLIGHTED_COMMAND))
            {
                cmd.RobotImage.color = SUBMITTED_COMMAND;
                cmd.deletable = false;
            }
        }
    }

    public void addSubmittedCommand(Sprite cmd, byte d, short id)
    {
        Transform panel = robotIdToPanel[id].transform.GetChild(3);
        for (int i = 0; i < panel.childCount; i++)
        {
            CommandSlotController child = panel.GetChild(i).GetComponent<CommandSlotController>();
            if (!child.RobotImage.color.Equals(NO_COMMAND) && child.RobotImage.sprite == null)
            {
                child.RobotImage.sprite = cmd;
                Rect size = child.RobotImage.rectTransform.rect;
                child.RobotImage.rectTransform.Rotate(Vector3.forward * d * 90);
                if (d % 2 == 1) child.RobotImage.rectTransform.rect.Set(size.x, size.y, size.height, size.width);
                child.deletable = child.RobotImage.color.Equals(OPEN_COMMAND);
                break;
            }
        }
    }

    public Tuple<string, byte>[] getCommandsSerialized(short id)
    {
        Transform panel = robotIdToPanel[id].transform.GetChild(3);
        List<Tuple<string, byte>> content = new List<Tuple<string, byte>>();
        for (int i = 0; i < panel.childCount; i++)
        {
            CommandSlotController child = panel.GetChild(i).GetComponent<CommandSlotController>();
            if (child.RobotImage.color.Equals(NO_COMMAND)) continue;
            if (child.RobotImage.sprite == null) break;
            content.Add(new Tuple<string, byte>(child.RobotImage.sprite.name, (byte)(child.RobotImage.rectTransform.localRotation.eulerAngles.z / 90)));
        }
        return content.ToArray();
    }

    public void SetBattery(int a, int b)
    {
        userScore.text = a.ToString();
        opponentScore.text = b.ToString();
    }

    public int GetUserBattery()
    {
        return int.Parse(userScore.text);
    }

    public int GetOpponentBattery()
    {
        return int.Parse(opponentScore.text);
    }

    public void PositionCamera(bool isPrimary)
    {
        float xMin = UsersRobots.transform.parent.GetComponent<RectTransform>().anchorMax.x;
        float xMax = OpponentsRobots.transform.parent.GetComponent<RectTransform>().anchorMin.x;
        boardCamera.rect = new Rect(xMin, 0, xMax - xMin, 1);
        boardCamera.transform.localPosition = new Vector3(Interpreter.boardController.boardCellsWide-1, Interpreter.boardController.boardCellsHeight-1,-20)/2;
        int iterations = 0;
        float diff;
        while (iterations < 20)
        {
            diff = (boardCamera.ViewportToWorldPoint(Vector3.back * boardCamera.transform.position.z).x + 0.5f);
            if (diff == 0) break;
            boardCamera.transform.position -= Vector3.forward * diff;
            iterations++;
        }
        if (!isPrimary) Interpreter.Flip();
    }

    public void Flip()
    {
        boardCamera.transform.Rotate(new Vector3(0, 0, 180));
        Interpreter.boardController.allQueueLocations.ToList().ForEach((TileController t) =>
        {
            t.GetComponent<SpriteRenderer>().flipY = !t.GetComponent<SpriteRenderer>().flipY;
            t.GetComponent<SpriteRenderer>().flipX = !t.GetComponent<SpriteRenderer>().flipX;
        });
        userScore.transform.Rotate(Vector3.forward, 180);
        opponentScore.transform.Rotate(Vector3.forward, 180);
    }

    public void SetButtons(bool b)
    {
        StepBackButton.interactable = StepForwardButton.interactable = BackToPresent.interactable = SubmitCommands.interactable = b;
    }

    public void LightUpPanel(bool bright, bool isUser)
    {
        Image panel = (isUser ? UsersRobots : OpponentsRobots).transform.parent.GetComponent<Image>();
        Color regular = (isUser ? new Color(0, 0.5f, 1.0f, 1.0f) : new Color(1.0f, 0, 0, 1.0f));
        float mult = (bright ? 1.0f : 0.5f);
        panel.color = new Color(regular.r * mult, regular.g*mult, regular.b * mult, regular.a * mult);
    }

}
