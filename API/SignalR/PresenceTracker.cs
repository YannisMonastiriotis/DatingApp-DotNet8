using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsersDict = [];

        public Task<bool> UserConnected(string username, string connectionId)
        {
            var isOnline = false;
            lock(OnlineUsersDict)
            {
                if(OnlineUsersDict.ContainsKey(username))
                {
                    OnlineUsersDict[username].Add(connectionId);
                }
                else
                {
                    var connectionIdList = new List<string>();

                    connectionIdList.Add(connectionId);

                    OnlineUsersDict.Add(username, connectionIdList);
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            var isOffline = false;
            lock(OnlineUsersDict)
            {
                if(!OnlineUsersDict.ContainsKey(username))
                {
                    return Task.FromResult(isOffline);
                }
                
                    
               OnlineUsersDict[username].Remove(connectionId);

               if(OnlineUsersDict[username].Count == 0)
               {
                OnlineUsersDict.Remove(username);
                isOffline = false;
               }
            }

            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUsersDict)
            {
                onlineUsers = OnlineUsersDict.OrderBy(k => k.Key).Select(k =>k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;

            if(OnlineUsersDict.TryGetValue(username, out var connections))
            {
                lock(connections)
                {
                    connectionIds = connections.ToList();
                }
            }
            else
            {
                connectionIds = [];
            }

            return Task.FromResult(connectionIds);
        }
    }
}