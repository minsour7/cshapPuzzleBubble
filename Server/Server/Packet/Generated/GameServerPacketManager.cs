using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

public enum MsgId
{
	S_Connect = 1,
	C_RoomInfo = 2,
	S_RoomInfo = 3,
	C_JoinGameRoom = 4,
	S_JoinGameRoom = 5,
	S_LeaveGameRoom = 6,
	S_Spawn = 7,
	S_Despawn = 8,
	C_StartGame = 9,
	S_StartGame = 10,
	C_Shoot = 11,
	S_Shoot = 12,
	C_Move = 13,
	S_Move = 14,
	C_NextBubbles = 15,
	S_NextBubbles = 16,
	S_NextBubblesPeer = 17,
	C_NextColsBubble = 18,
	S_NextColsBubble = 19,
	S_NextColsBubblePeer = 20,
	C_NextColsBubbleList = 21,
	S_NextColsBubbleList = 22,
	C_FixedBubbleSlot = 23,
	S_FixedBubbleSlotPeer = 24,
	C_PlayerGameOver = 25,
	S_PlayerGameOverBroadCast = 26,
	S_GameResult = 27,
}

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.C_RoomInfo, MakePacket<C_RoomInfo>);
		_handler.Add((ushort)MsgId.C_RoomInfo, PacketHandler.C_RoomInfoHandler);		
		_onRecv.Add((ushort)MsgId.C_JoinGameRoom, MakePacket<C_JoinGameRoom>);
		_handler.Add((ushort)MsgId.C_JoinGameRoom, PacketHandler.C_JoinGameRoomHandler);		
		_onRecv.Add((ushort)MsgId.C_StartGame, MakePacket<C_StartGame>);
		_handler.Add((ushort)MsgId.C_StartGame, PacketHandler.C_StartGameHandler);		
		_onRecv.Add((ushort)MsgId.C_Shoot, MakePacket<C_Shoot>);
		_handler.Add((ushort)MsgId.C_Shoot, PacketHandler.C_ShootHandler);		
		_onRecv.Add((ushort)MsgId.C_Move, MakePacket<C_Move>);
		_handler.Add((ushort)MsgId.C_Move, PacketHandler.C_MoveHandler);		
		_onRecv.Add((ushort)MsgId.C_NextBubbles, MakePacket<C_NextBubbles>);
		_handler.Add((ushort)MsgId.C_NextBubbles, PacketHandler.C_NextBubblesHandler);		
		_onRecv.Add((ushort)MsgId.C_NextColsBubble, MakePacket<C_NextColsBubble>);
		_handler.Add((ushort)MsgId.C_NextColsBubble, PacketHandler.C_NextColsBubbleHandler);		
		_onRecv.Add((ushort)MsgId.C_NextColsBubbleList, MakePacket<C_NextColsBubbleList>);
		_handler.Add((ushort)MsgId.C_NextColsBubbleList, PacketHandler.C_NextColsBubbleListHandler);		
		_onRecv.Add((ushort)MsgId.C_FixedBubbleSlot, MakePacket<C_FixedBubbleSlot>);
		_handler.Add((ushort)MsgId.C_FixedBubbleSlot, PacketHandler.C_FixedBubbleSlotHandler);		
		_onRecv.Add((ushort)MsgId.C_PlayerGameOver, MakePacket<C_PlayerGameOver>);
		_handler.Add((ushort)MsgId.C_PlayerGameOver, PacketHandler.C_PlayerGameOverHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}