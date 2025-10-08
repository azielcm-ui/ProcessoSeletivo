using System;
namespace Cadastro.API.Domain
{
	public class UserToken
	{
        public string Token { get; set; } = null!;
        public DateTime DtExpira { get; set; }

		public UserToken(string token, DateTime dtExpira)
		{ Token = token; DtExpira = dtExpira; }

	}
}

