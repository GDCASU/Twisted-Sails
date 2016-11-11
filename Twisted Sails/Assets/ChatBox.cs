using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class ChatBox : NetworkBehaviour
{
	public string messageSendKey;
	public InputField inputField;
	public GameObject chatEnterField;
	public int maxNumberOfMessagesStored = 5;

	private Queue chatQueue;
	public Text[] textSlots;

	private string playerCurrentChatValue;
	private MultiplayerManager mpManager;

	private void Start()
	{
		chatQueue = new Queue();
		mpManager = GameObject.FindObjectOfType<MultiplayerManager>();
		playerCurrentChatValue = String.Empty;
	}

	private void Update()
	{
		//if the player presses down the enter button
		if (Input.GetButtonDown(messageSendKey))
		{
			if (inputField.IsActive())
			{
				//make sure the player has entered a message that isn't just whitespace
				if (playerCurrentChatValue.Length > 0 && Regex.Replace(playerCurrentChatValue, @"\s+", "").Length > 0)
				{
					ChatMessage newMessage = new ChatMessage();
					newMessage.message = playerCurrentChatValue;
					newMessage.playerName = mpManager.localPlayerName;
					newMessage.hour = System.DateTime.Now.Hour;
					newMessage.second = System.DateTime.Now.Second;

					if (!isServer)
					{
						CmdSendChatMessageToAllPlayers(newMessage);
					}
					else
					{
						RpcReceiveChatMessage(newMessage);
					}

					inputField.text = String.Empty;
					playerCurrentChatValue = String.Empty;
				}
				inputField.DeactivateInputField();
				chatEnterField.SetActive(false);
			}
			else
			{
				chatEnterField.SetActive(true);
				inputField.Select();
				inputField.ActivateInputField();
			}
		}

		System.Object[] array = chatQueue.ToArray();
		for (int i = 0; i < textSlots.Length; i++)
		{
			if (i < chatQueue.Count)
			{
				textSlots[i].text = ChatMessageToString(((ChatMessage)array[i]));
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

	[Command]
	private void CmdSendChatMessageToAllPlayers(ChatMessage message)
	{
		RpcReceiveChatMessage(message);
	}

	[ClientRpc]
	private void RpcReceiveChatMessage(ChatMessage message)
	{
		chatQueue.Enqueue(message);

		if (chatQueue.Count > maxNumberOfMessagesStored)
		{
			chatQueue.Dequeue();
		}

		print("Recieved a message!");
	}

	private string  ChatMessageToString(ChatMessage message)
	{
		return "(" + message.hour + ":" + message.second + ") " + message.playerName + ": " + message.message;
	}

}

struct ChatMessage
{
	public string message;
	public string playerName;
	public int hour;
	public int second;
}
