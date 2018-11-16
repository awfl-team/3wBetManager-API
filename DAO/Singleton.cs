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

        public IUserDao SetUserDao(IUserDao userDao)
        {
            return UserDao = userDao;
        }

    }
}
