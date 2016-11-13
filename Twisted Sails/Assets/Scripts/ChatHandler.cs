using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;

/**
	The Chat Handler, while attached to the player, allows chat message structs to be sent between clients connected to the server.
	Kyle Chapman
	Novemeber 9, 2016
*/

public class ChatHandler : NetworkBehaviour
{
	//attempts to send out a new chat message to the each client in the server
	public void SendOutMessage(ChatMessage newMessage)
	{
		//if we are not the server, send out a COMMAND to the server
		if (!isServer)
		{
			CmdSendChatMessageToAllPlayers(newMessage);
		}
		//if we are the server, send RPCs directly
		else
		{
			RpcReceiveChatMessage(newMessage);
		}
	}

	//clients run this command so the server will send the message to all connected clients
	[Command]
	public void CmdSendChatMessageToAllPlayers(ChatMessage message)
	{
		RpcReceiveChatMessage(message);
	}

	//RPC for a client receiving a chat message
	//when a new message is recieved, find a ChatUI in the scene and tell it about the new message
	[ClientRpc]
	public void RpcReceiveChatMessage(ChatMessage message)
	{
		ChatUI chatUI = GameObject.FindObjectOfType<ChatUI>();
		if (chatUI != null)
		{
			chatUI.ReceiveNewMessage(message);
		}
	}

	//convert a chat message struct to string
	public static string ChatMessageToString(ChatMessage message)
	{
		return "(" + message.hour + ":" + message.minute + ") " + message.playerName + ": " + message.message;
	}

}

//struct used to represent a chat message
public struct ChatMessage
{
	public string message;
	public string playerName;
	public int hour;
	public int minute;
}
