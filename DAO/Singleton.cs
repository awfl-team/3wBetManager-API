using DAO.Interfaces;
using MongoDB.Driver;

namespace DAO
{
    public class Singleton
    {
        private static Singleton _instance = null;

        public static Singleton Instance => _instance ?? (_instance = new Singleton());

        public IUserDao UserDao { get; private set; }
        public ICompetitionDao CompetitionDao { get; private set; }
        public ITeamDao TeamDao { get; private set; }
        public IMatchDao MatchDao { get; private set; }
        public IBetDao BetDao { get; private set; }

        public void SetAll(IMongoDatabase database)
        {
            SetTeamDao(new TeamDao(database));
            SetMatchDao(new MatchDao(database));
            SetBetDao(new BetDao(database));
            SetCompetitionDao(new CompetitionDao(database));
            SetUserDao(new UserDao(database));
        }

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

        public IMatchDao SetMatchDao(IMatchDao matchDao)
        {
            return MatchDao = matchDao;
        }

        public IBetDao SetBetDao(IBetDao betDao)
        {
            return BetDao = betDao;
        }


    }
}
