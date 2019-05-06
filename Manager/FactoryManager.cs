
namespace Manager
{
    public class FactoryManager
    {
        public static IManager<T> Create<T>()
        {
            if (typeof(T) == typeof(AssignmentPointManager))
            {
                return  new AssignmentPointManager() as IManager<T>;
            }

            if (typeof(T) == typeof(BetManager))
            {

            }

            if (typeof(T) == typeof(EmailManager))
            {

            }

            if (typeof(T) == typeof(FootballDataManager))
            {

            }

            if (typeof(T) == typeof(ItemManager))
            {

            }

            if (typeof(T) == typeof(MatchManager))
            {

            }

            if (typeof(T) == typeof(MonitoringManager))
            {

            }

            if (typeof(T) == typeof(TokenManager))
            {

            }

            if (typeof(T) == typeof(UserManager))
            {

            }

            return null;
        }

    }
}
