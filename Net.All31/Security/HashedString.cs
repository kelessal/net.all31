using System;

namespace Net.Security
{
    public class HashedString :IEquatable<HashedString>
    {
        public string Salt { get; set; }

        public string Hash { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as HashedString);
        }
        public bool Equals(HashedString other)
        {
            if (other == null) return false;
            return other.Salt == this.Salt && other.Hash == this.Hash;
        }
        public override int GetHashCode()
        {
            return $"{this.Salt}${this.Hash}".GetHashCode();
        }

        
    }
}
