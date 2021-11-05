using System;

namespace BookApi.Controllers
{
    internal interface IBaseController
    {
        void LogInformation(string message);

        void LogException(string message, Exception ex);
    }
}