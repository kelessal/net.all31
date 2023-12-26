using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Net.Security
{
    public interface ITokenGenerator
    {
        string CreateToken(string userName, DateTime? expires = null, params KeyValuePair<string, string>[] claims);
    }
}
