using System;
using Models;

namespace Manager.Interfaces
{
    public interface IEmailManager
    {
        void SendResetPasswordEmail(User user);
        void SendVerifyAccountEmail(User user);
        void SendWebMasterEmail(Exception exception);
    }
}