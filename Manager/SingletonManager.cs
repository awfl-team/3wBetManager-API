using Manager.Interfaces;

namespace Manager
{
    public class SingletonManager
    {
        private static SingletonManager _instance;

        public static SingletonManager Instance => _instance ?? (_instance = new SingletonManager());

        public IAssignmentPointManager AssignmentPointManager { get; private set; }
        public IBetManager BetManager { get; private set; }
        public IEmailManager EmailManager { get; private set; }
        public IFootballDataManager FootballDataManager { get; private set; }
        public IItemManager ItemManager { get; private set; }
        public IMatchManager MatchManager { get; private set; }
        public IMonitoringManager MonitoringManager { get; private set; }
        public ITokenManager TokenManager { get; private set; }
        public IUserManager UserManager { get; private set; }
        public ICompetitionManager CompetitionManager { get; private set; }


        public IAssignmentPointManager SetAssignmentPointManager(IAssignmentPointManager assignmentPointManager)
        {
            return AssignmentPointManager = assignmentPointManager;
        }

        public IBetManager SetBetManager(IBetManager betManager)
        {
            return BetManager = betManager;
        }

        public IEmailManager SetEmailManager(IEmailManager emailManager)
        {
            return EmailManager = emailManager;
        }

        public IFootballDataManager SetFootballDataManager(IFootballDataManager footballDataManager)
        {
            return FootballDataManager = footballDataManager;
        }

        public IItemManager SetItemManager(IItemManager itemManager)
        {
            return ItemManager = itemManager;
        }

        public IMatchManager SetMatchManager(IMatchManager matchManager)
        {
            return MatchManager = matchManager;
        }

        public IMonitoringManager SetMonitoringManager(IMonitoringManager monitoringManager)
        {
            return MonitoringManager = monitoringManager;
        }

        public ITokenManager SetTokenManager(ITokenManager tokenManager)
        {
            return TokenManager = tokenManager;
        }

        public IUserManager SetUserManager(IUserManager userManager)
        {
            return UserManager = userManager;
        }

        public ICompetitionManager SetCompetitionManager(ICompetitionManager competitionManager)
        {
            return CompetitionManager = competitionManager;
        }
    }
}