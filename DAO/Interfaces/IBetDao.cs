using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAO.Interfaces
{
    public interface IBetDao
    {
        void AddBet(Bet bet);
        void AddListBet(List<Bet> bets);
    }
}
