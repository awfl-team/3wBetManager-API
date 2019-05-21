using DAO.Interfaces;
using MongoDB.Driver;
using Proxy;

namespace DAO
{
    public class SingletonDao
    {
        private static SingletonDao _instance;

        public static SingletonDao Instance => _instance ?? (_instance = new SingletonDao());

        public IUserDao UserDao { get; private set; }
        public ICompetitionDao CompetitionDao { get; private set; }
        public ITeamDao TeamDao { get; private set; }
        public IMatchDao MatchDao { get; private set; }
        public IBetDao BetDao { get; private set; }
        public IItemDao ItemDao { get; private set; }

        public void SetAllDao(IMongoDatabase database)
        {
            SetTeamDao(new TeamDao(database));
            SetMatchDao(new MatchDao(database));
            SetBetDao(new BetDao(database));
            SetCompetitionDao(new CompetitionDao(database));
            SetUserDao(new UserDao(database));
            SetItemDao(new ItemDao(database));
        }

        public IUserDao SetUserDao(IUserDao userDao)
        {
            var dynamicUserDaoProxy = new Proxy<IUserDao>(userDao);
            return UserDao = dynamicUserDaoProxy.GetTransparentProxy() as IUserDao;
        }

        public ICompetitionDao SetCompetitionDao(ICompetitionDao competitionDao)
        {
            var dynamicCompetitionDaoProxy = new Proxy<ICompetitionDao>(competitionDao);
            return CompetitionDao = dynamicCompetitionDaoProxy.GetTransparentProxy() as ICompetitionDao;
        }

        public ITeamDao SetTeamDao(ITeamDao teamDao)
        {
            var dynamicTeamDaoProxy = new Proxy<ITeamDao>(teamDao);
            return TeamDao = dynamicTeamDaoProxy.GetTransparentProxy() as ITeamDao;
        }

        public IMatchDao SetMatchDao(IMatchDao matchDao)
        {
            var dynamicMatchDaoProxy = new Proxy<IMatchDao>(matchDao);
            return MatchDao = dynamicMatchDaoProxy.GetTransparentProxy() as IMatchDao;
        }

        public IBetDao SetBetDao(IBetDao betDao)
        {
            var dynamicCompetitionDaoProxy = new Proxy<IBetDao>(betDao);
            return BetDao = dynamicCompetitionDaoProxy.GetTransparentProxy() as IBetDao;
        }

        public IItemDao SetItemDao(IItemDao itemDao)
        {
            var dynamicCompetitionDaoProxy = new Proxy<IItemDao>(itemDao);
            return ItemDao = dynamicCompetitionDaoProxy.GetTransparentProxy() as IItemDao;
        }
    }
}