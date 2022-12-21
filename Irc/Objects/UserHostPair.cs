using Irc.Interfaces;

namespace Irc.Objects;

public partial class Address
{
    public record UserHostPair : IUserHostPair
    {
        public string User { get; set; }
        public string Host { get; set; }

        public override string ToString()
        {
            return $"{User}@{Host}";
        }
    }
}