using System;
using System.Collections.Generic;
using System.Text;
using DAO.Interfaces;

namespace DAO
{
    public class Singleton
    {
        private static Singleton _instance = null;

        public static Singleton Instance
        {
            get { return _instance ?? (_instance = new Singleton()); }
        }

        public IUserDao UserDao { get; private set; }
        public ICompetitionDao CompetitionDao { get; private set; }
        public ITeamDao TeamDao { get; private set; }
        public IMatchDao MatchDao { get; private set; }

        public IUserDao SetUserDao(IUserDao userDao)
        {
            return UserDao = userDao;
        }

        public ICompetitionDao SetCompetitionDao(ICompetitionDao competitionDao)
        {
            return CompetitionDao = competitionDao;
        }

        public ITeamDao SetTeamDao(ITeamDao teamDao)
        {
            return TeamDao = teamDao;
        }

        public IMatchDao SeMatchDao(IMatchDao matchDao)
        {
            return MatchDao = matchDao;
        }

    }
}
