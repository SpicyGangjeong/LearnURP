using System;
using System.Collections.Generic;

namespace Logic
{
    namespace Room
    {
        public class RoomGenerator
        {
            public List<Room> Build(IReadOnlyList<RoomData> vData, int iStartRoomID)
            {
                if (null == vData)
                {
                    throw new ArgumentNullException(nameof(vData));
                }
                if (0 == vData.Count)
                {
                    throw new InvalidOperationException("RoomGenerator.Build requires at least one RoomData.");
                }

                Dictionary<int, Room> vRoomsByID = new Dictionary<int, Room>();
                List<Room> vRooms = new List<Room>(vData.Count);

                for (int i = 0; i < vData.Count; ++i)
                {
                    RoomData pData = vData[i];
                    if (null == pData)
                    {
                        throw new InvalidOperationException($"RoomGenerator.Build RoomData at index {i} is null.");
                    }
                    if (true == vRoomsByID.ContainsKey(pData.ID))
                    {
                        throw new InvalidOperationException($"RoomGenerator.Build duplicate room ID {pData.ID}.");
                    }

                    Room pRoom = new Room(pData);
                    pRoom.SetActive(false);
                    pRoom.SetVisited(false);
                    vRoomsByID.Add(pData.ID, pRoom);
                    vRooms.Add(pRoom);
                }

                if (false == vRoomsByID.TryGetValue(iStartRoomID, out Room pStartRoom))
                {
                    throw new InvalidOperationException($"RoomGenerator.Build start room ID {iStartRoomID} not found.");
                }

                for (int i = 0; i < vRooms.Count; ++i)
                {
                    Room pRoom = vRooms[i];
                    IReadOnlyList<int> vConnectedIDs = pRoom.Data.ConnectedRoomIDs;
                    List<Room> vConnectedRooms = new List<Room>();
                    for (int j = 0; j < vConnectedIDs.Count; ++j)
                    {
                        int iConnectedID = vConnectedIDs[j];
                        if (false == vRoomsByID.TryGetValue(iConnectedID, out Room pConnected))
                        {
                            throw new InvalidOperationException(
                                $"RoomGenerator.Build room {pRoom.Data.ID} connects to missing ID {iConnectedID}.");
                        }
                        vConnectedRooms.Add(pConnected);
                    }
                    pRoom.SetConnectedRooms(vConnectedRooms);
                }

                // Start-room activation is evaluated by RoomManager (conditions).
                return vRooms;
            }
        }
    }
}
