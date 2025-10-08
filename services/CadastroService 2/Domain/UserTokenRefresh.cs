using System;
namespace Cadastro.API.Domain
{
    public class UserTokenRefresh
    {
       
        public string Token { get; set; } = null!;

        public UserTokenRefresh(string token)
        { Token = token;}
    }
}

