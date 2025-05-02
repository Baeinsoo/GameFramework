using System.Collections.Generic;

namespace GameFramework
{
    public interface ISessionManager
    {
        void AddSession(ISession session);
        void RemoveSession(ISession session);

        ISession GetSessionById(string sessionId);
        ISession GetSessionByUserId(string userId);

        T GetSessionById<T>(string sessionId) where T : ISession;
        T GetSessionByUserId<T>(string userId) where T : ISession;

        bool TryGetSessionById(string sessionId, out ISession session);
        bool TryGetSessionByUserId<T>(string userId, out T session) where T : ISession;

        void RemoveSessionById(string sessionId);
        void RemoveSessionByUserId(string userId);

        IReadOnlyCollection<ISession> GetAllSessions();
    }
}
