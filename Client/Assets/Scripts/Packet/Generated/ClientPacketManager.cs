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
		_onRecv.Add((ushort)MsgId.S_Connect, MakePacket<S_Connect>);
		_handler.Add((ushort)MsgId.S_Connect, PacketHandler.S_ConnectHandler);		
		_onRecv.Add((ushort)MsgId.S_RoomInfo, MakePacket<S_RoomInfo>);
		_handler.Add((ushort)MsgId.S_RoomInfo, PacketHandler.S_RoomInfoHandler);		
		_onRecv.Add((ushort)MsgId.S_JoinGameRoom, MakePacket<S_JoinGameRoom>);
		_handler.Add((ushort)MsgId.S_JoinGameRoom, PacketHandler.S_JoinGameRoomHandler);		
		_onRecv.Add((ushort)MsgId.S_LeaveGameRoom, MakePacket<S_LeaveGameRoom>);
		_handler.Add((ushort)MsgId.S_LeaveGameRoom, PacketHandler.S_LeaveGameRoomHandler);		
		_onRecv.Add((ushort)MsgId.S_Spawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.S_Spawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.S_Despawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.S_Despawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.S_StartGame, MakePacket<S_StartGame>);
		_handler.Add((ushort)MsgId.S_StartGame, PacketHandler.S_StartGameHandler);		
		_onRecv.Add((ushort)MsgId.S_Shoot, MakePacket<S_Shoot>);
		_handler.Add((ushort)MsgId.S_Shoot, PacketHandler.S_ShootHandler);		
		_onRecv.Add((ushort)MsgId.S_Move, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.S_Move, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.S_NextBubbles, MakePacket<S_NextBubbles>);
		_handler.Add((ushort)MsgId.S_NextBubbles, PacketHandler.S_NextBubblesHandler);		
		_onRecv.Add((ushort)MsgId.S_NextBubblesPeer, MakePacket<S_NextBubblesPeer>);
		_handler.Add((ushort)MsgId.S_NextBubblesPeer, PacketHandler.S_NextBubblesPeerHandler);		
		_onRecv.Add((ushort)MsgId.S_NextColsBubble, MakePacket<S_NextColsBubble>);
		_handler.Add((ushort)MsgId.S_NextColsBubble, PacketHandler.S_NextColsBubbleHandler);		
		_onRecv.Add((ushort)MsgId.S_NextColsBubblePeer, MakePacket<S_NextColsBubblePeer>);
		_handler.Add((ushort)MsgId.S_NextColsBubblePeer, PacketHandler.S_NextColsBubblePeerHandler);		
		_onRecv.Add((ushort)MsgId.S_NextColsBubbleList, MakePacket<S_NextColsBubbleList>);
		_handler.Add((ushort)MsgId.S_NextColsBubbleList, PacketHandler.S_NextColsBubbleListHandler);		
		_onRecv.Add((ushort)MsgId.S_FixedBubbleSlotPeer, MakePacket<S_FixedBubbleSlotPeer>);
		_handler.Add((ushort)MsgId.S_FixedBubbleSlotPeer, PacketHandler.S_FixedBubbleSlotPeerHandler);		
		_onRecv.Add((ushort)MsgId.S_PlayerGameOverBroadCast, MakePacket<S_PlayerGameOverBroadCast>);
		_handler.Add((ushort)MsgId.S_PlayerGameOverBroadCast, PacketHandler.S_PlayerGameOverBroadCastHandler);		
		_onRecv.Add((ushort)MsgId.S_GameResult, MakePacket<S_GameResult>);
		_handler.Add((ushort)MsgId.S_GameResult, PacketHandler.S_GameResultHandler);
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