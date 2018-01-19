﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour {

	private GameObject BackgroundPanel;
	private GameObject PlayerTurnTextObject;
	private GameObject PlayerAPanel;
	private GameObject PlayerBPanel;
	private GameObject robotInfoPanel;
	private GameObject robotInfoPanelRobotName;
	private GameObject robotInfoPanelRobotSprite;
	private GameObject robotInfoPanelRobotAttributes;
	private GameObject robotInfoPanelRobotStatus;
	private GameObject robotInfoPanelRobotEquipment;
    private GameObject modalTextBackdrop;
    private GameObject modalDisplayPanel;

    public List<string> submittedActions = new List<string>();

    public Text placeholder;
    public GameObject modalPanelObject;
    public Button cancelButton;
    public Sprite[] sprites;

	private GameObject robotImagePanel;

	private string[] playerNames;
	private Sprite robotSprite;
    private bool myTurn;

    void Start()
    {
        Interpreter.InitializeUI(this);
    }

    //Loads the UICanvas and it's child components
    public void InitializeUICanvas(Game.Player[] playerTurnObjects) 
	{
        // get child components  
        BackgroundPanel = GameObject.Find("UICanvas");

        TMP_Text  playerTurnText = getChildTMP_Text(BackgroundPanel, "PlayerTurnText");

        // playerturntextobject = getchildgameobject (backgroundpanel, "playerturntext");

        PlayerAPanel = getChildGameObject(BackgroundPanel, "PlayerAPanel");
        PlayerBPanel = getChildGameObject(BackgroundPanel, "PlayerBPanel");
        GameObject[] playerPanels = { PlayerAPanel, PlayerBPanel };

        // set the components of the uicanvas
        SetPlayerTurnText(playerTurnText, playerTurnObjects[0]);
        SetPlayerPanels(playerPanels, playerTurnObjects);

	}

	// Set's header text of UICanvas
	void SetPlayerTurnText(TMP_Text playerTurnText, Game.Player currentPlayer)
	{
		playerTurnText.SetText(currentPlayer.name + "'s Turn");
	}

	// Sets each players panels on the UICanvas (Contains robot info)
	void SetPlayerPanels (GameObject[] PlayerPanels, Game.Player[] PlayerTurnObjects)
	{
		// for each playerPanel
			// Set headertext
			// for each robot
				// get correct panel
				//attach info

		for (int i = 0; i < PlayerPanels.Length; i++) {

			TMP_Text playerPanelHeader = getChildTMP_Text(PlayerPanels [i], "Player Robots Summary");

			playerPanelHeader.SetText(PlayerTurnObjects[i].name);

			for (int j = 1; j < 1 + PlayerTurnObjects[i].team.Length; j++){
				string currentRobotInfoPanel = "RobotInfoPanel (" + (j) + ")";
				Robot currentRobot = PlayerTurnObjects[i].team[j-1];
				robotInfoPanel = getChildGameObject(PlayerPanels[i], currentRobotInfoPanel);
//			
//
//				//Robot name 
				TMP_Text robotInfoPanelRobotText = getChildTMP_Text(robotInfoPanel, "RobotName");
				robotInfoPanelRobotText.SetText(currentRobot.name);
//
				//Robot sprite
				robotInfoPanelRobotSprite = getChildGameObject(robotInfoPanel, "RobotSprite");
				attachRobotSprite(robotInfoPanelRobotSprite, currentRobot.name);
//
//				//Robot attributes

				TMP_Text robotInfoPanelRobotAttributes = getChildTMP_Text (robotInfoPanel, "Attributes");
				robotInfoPanelRobotAttributes.SetText("A: " + currentRobot.attack.ToString() + " P: " + currentRobot.priority.ToString() + " H: " + currentRobot.health.ToString());

			}
		}

	}

	void attachRobotSprite(GameObject robotImagePanel, string robotName){
        robotSprite = Array.Find(sprites, (Sprite s) => s.name.StartsWith(robotName));
		robotImagePanel.GetComponent<Image>().sprite = robotSprite;
	}

	GameObject getChildGameObject(GameObject parentGameObject, string searchName) {
		Transform[] childGameObjects = parentGameObject.transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform in childGameObjects) {
			if (transform.gameObject.name == searchName) {
				return transform.gameObject;
			} 
		}
		return null;
	}

	TMP_Text getChildTMP_Text(GameObject parentGameObject, string searchName) {
		TMP_Text[] childGameObjects = parentGameObject.GetComponentsInChildren<TMP_Text>();
		foreach (TMP_Text TMPtext in childGameObjects) {
			if (TMPtext.name == searchName) {
				return TMPtext;
			} 
		}
		return null;
	}

    // Modal functions
    public void ShowHandButtonPress()
    {
        
       modalPanelObject.SetActive(true);
       cancelButton.onClick.RemoveAllListeners();
       cancelButton.onClick.AddListener(ClosePanel);

       cancelButton.gameObject.SetActive(true);
    }

    public void SubmitActionsButtonPress()
    {
        Interpreter.SubmitActions();
        submittedActions.Clear();
    }

    public void ClearActionsButtonPress()
    {
        submittedActions.Clear();
    }



    public void ShowQueuedActionsButtonPress()
    {
        formatActionsModalTextLines(submittedActions);
        modalPanelObject.SetActive(true);
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(ClosePanel);
        cancelButton.gameObject.SetActive(true);
    }

    public void addSubmittedCommand(Command cmd, string robotIdentifier)
    {
        string CommandText = robotIdentifier + " " + cmd.ToString();
        submittedActions.Add(CommandText);
        //Dropdown ActionsDropdown = GameObject.Find("Submitted Actions Dropdown").GetComponent<Dropdown>();
        //ActionsDropdown.AddOptions(submittedActions);
    }

    public void resetModal()
    {
        modalDisplayPanel = getChildGameObject(modalPanelObject, "ModalDisplay");
        modalTextBackdrop = getChildGameObject(modalDisplayPanel, "Text Backdrop");
        foreach (Transform child in modalTextBackdrop.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void formatActionsModalTextLines(List<string> textLines)
    {
        // grab size of whole box, 390, 575
        modalDisplayPanel = getChildGameObject(modalPanelObject, "ModalDisplay");
        modalTextBackdrop = getChildGameObject(modalDisplayPanel, "Text Backdrop");
        float xWidth = 390f - 35;
        //float yWidth = 575f;
        float ySpacer = 0;
        float yStart = -200;
        // For each textline in textLines, create new gameObject with text in it
        for (int i = 0; i < textLines.Count; i++)
        {
            GameObject textBox = new GameObject("textBox");
            textBox.transform.SetParent(modalTextBackdrop.transform);
            Text textToAdd = textBox.AddComponent<Text>();
            textToAdd.text = textLines[i];
            textToAdd.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textToAdd.transform.Rotate(new Vector3(180, 0, 0));
            textToAdd.transform.localScale = new Vector3(1.15f, 2.5f, 0);
            textToAdd.transform.localPosition = new Vector3(35f, ySpacer + yStart, 0f);
            textToAdd.rectTransform.sizeDelta= new Vector2(xWidth,50);
            ySpacer = ySpacer + 75;

        }
    }

    void ClosePanel()
    {
        modalPanelObject.SetActive(false);
        resetModal();
    }

}
