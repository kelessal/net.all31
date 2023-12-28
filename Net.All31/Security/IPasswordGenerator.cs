using System;

namespace Net.Security
{
    public interface IPasswordGenerator
    {
        string CreateRandomPassword(int length,bool onlyNumber=false);
        HashedString CreateHashedString(string password);
        bool IsMatch(HashedString hash, string password);
    }
}
