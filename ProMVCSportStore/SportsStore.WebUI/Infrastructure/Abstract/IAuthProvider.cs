using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsStore.WebUI.Infrastructure.Abstract
{
    public interface IAuthProvider
    {
        bool Autheticate(string username, string password);
    }
}