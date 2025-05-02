using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class SessionManager : ISessionManager
    {
        private readonly Dictionary<string, ISession> sessionsById = new Dictionary<string, ISession>();
        private readonly Dictionary<string, ISession> sessionsByUserId = new Dictionary<string, ISession>();

        public void AddSession(ISession session)
        {
            sessionsById[session.sessionId] = session;
            sessionsByUserId[session.userId] = session;
        }

        public void RemoveSession(ISession session)
        {
            sessionsById.Remove(session.sessionId);
            sessionsByUserId.Remove(session.userId);
        }

        public ISession GetSessionById(string sessionId)
        {
            return sessionsById[sessionId];
        }

        public ISession GetSessionByUserId(string userId)
        {
            return sessionsByUserId[userId];
        }

        public T GetSessionById<T>(string sessionId) where T : ISession
        {
            return (T)sessionsById[sessionId];
        }

        public T GetSessionByUserId<T>(string userId) where T : ISession
        {
            return (T)sessionsByUserId[userId];
        }

        public bool TryGetSessionById(string sessionId, out ISession session)
        {
            if (sessionsById.TryGetValue(sessionId, out var value))
            {
                session = value;
                return true;
            }
            else
            {
                session = default;
                return false;
            }
        }

        public bool TryGetSessionByUserId<T>(string userId, out T session) where T : ISession
        {
            if (sessionsByUserId.TryGetValue(userId, out var value))
            {
                session = (T)value;
                return true;
            }
            else
            {
                session = default;
                return false;
            }
        }

        public void RemoveSessionById(string sessionId)
        {
            if (sessionsById.TryGetValue(sessionId, out var session))
            {
                sessionsById.Remove(sessionId);
                sessionsByUserId.Remove(session.userId);
            }
        }

        public void RemoveSessionByUserId(string userId)
        {
            if (sessionsByUserId.TryGetValue(userId, out var session))
            {
                sessionsByUserId.Remove(userId);
                sessionsById.Remove(session.sessionId);
            }
        }

        public IReadOnlyCollection<ISession> GetAllSessions()
        {
            return sessionsById.Values;
        }
    }
}
