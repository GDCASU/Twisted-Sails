using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

/**
	The Chat UI, is the manager of the UI elements responsible for detecting chat message typing.
	It also keeps a queue of all messages the client has sent or received from other clients.
	Then handles showing that queue of messages on the screen (using components in the scenes canvas).
	Kyle Chapman
	Novemeber 9, 2016
*/

public class ChatUI : MonoBehaviour {

	//the string name of the input axis the player is using to send messages
	public string messageSendKey;

	//the objects in the canvas managed by the ChatUI
	[Header("Canvas Objects")]
	public InputField inputField;
	public GameObject chatEnterField;
	public GameObject chatRecord;
	public GameObject chatSlotPrefab;

	//settings for how large the chat queue is
	//and how many chat messages should be shown on the screen at once
	[Header("Chat Window Settings")]
	public int messagesShownOnScreen = 10;
	public int maxNumberOfMessagesStored = 5;

	//the queue of chat messages and the canvas text slots for showing the messages
	private Queue chatQueue;
	private Text[] textSlots;

	//the value the player currently is typing inside the chat area
	private string playerCurrentChatValue;

	private MultiplayerManager mpManager;
	private ChatHandler chatHandler;

	private void Start()
	{
		chatQueue = new Queue();

		mpManager = GameObject.FindObjectOfType<MultiplayerManager>();

		playerCurrentChatValue = String.Empty;

		//initialize the chat slots
		//make as many as messages shown on screen
		//do math to fit all of them in a children of the chat record panel
		textSlots = new Text[messagesShownOnScreen];
		for (int i = 0; i < messagesShownOnScreen; i++)
		{
			GameObject chatSlot = Instantiate(chatSlotPrefab);
			chatSlot.transform.SetParent(chatRecord.transform);
			RectTransform slotsRect = chatSlot.GetComponent<RectTransform>();
			slotsRect.anchorMin = new Vector2(0, i / (float)messagesShownOnScreen);
			slotsRect.anchorMax = new Vector2(1, (i + 1) / (float)messagesShownOnScreen);
			slotsRect.offsetMin = Vector2.zero;
			slotsRect.offsetMax = Vector2.zero;

			textSlots[i] = chatSlot.GetComponent<Text>();
		}
	}

	private void Update()
	{
		//if the player presses down the button to access the chat
		if (Input.GetButtonDown(messageSendKey))
		{
			//if already typing, player is trying to send a message
			//or close the window if their text is empty
			if (inputField.IsActive())
			{
				//make sure the player has entered a message that isn't just whitespace
				if (playerCurrentChatValue.Length > 0 && Regex.Replace(playerCurrentChatValue, @"\s+", "").Length > 0)
				{
					ChatMessage newMessage = new ChatMessage();
					newMessage.message = playerCurrentChatValue;
					newMessage.playerName = mpManager.localPlayerName;
					newMessage.hour = System.DateTime.Now.Hour;
					newMessage.minute = System.DateTime.Now.Minute;

					//find the chathandler attached to the player
					//and tell it to send the message to the other clients
					ChatHandler chatHandler = GameObject.FindObjectOfType<ChatHandler>();
					chatHandler.SendOutMessage(newMessage);

					inputField.text = String.Empty;
					playerCurrentChatValue = String.Empty;
				}
				//deactivate the chat entry area
				inputField.DeactivateInputField();
				chatEnterField.SetActive(false);
			}
			//if not typing already
			//open the chat entry area so they can begin typing
			else
			{
				chatEnterField.SetActive(true);
				inputField.Select();
				inputField.ActivateInputField();
			}
		}

		//update the UI with whatever chat messages are currently in the array
		System.Object[] array = chatQueue.ToArray();
		Array.Reverse(array);
		for (int i = 0; i < textSlots.Length; i++)
		{
			if (i < chatQueue.Count)
			{
				textSlots[i].text = ChatHandler.ChatMessageToString(((ChatMessage)array[i]));
			}
			else
			{
				textSlots[i].text = String.Empty;
			}
		}
	}

	//update the value of the chat message the player is typing in the chat box
	public void UpdateChatEnterValue(string newChatValue)
	{
		playerCurrentChatValue = newChatValue;
	}

	//inserts a new message into the queue
	//if this overflows the queue, remove the oldest message
	public void ReceiveNewMessage(ChatMessage message)
	{
		chatQueue.Enqueue(message);

		if (chatQueue.Count > maxNumberOfMessagesStored)
		{
			chatQueue.Dequeue();
		}

		print("Message received!");
	}

}
