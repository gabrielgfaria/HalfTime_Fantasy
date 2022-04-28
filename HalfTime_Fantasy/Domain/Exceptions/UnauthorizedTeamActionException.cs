namespace Domain.Exceptions
{
    public class UnauthorizedTeamActionException : Exception
    {
        public UnauthorizedTeamActionException()
        {
        }

        public UnauthorizedTeamActionException(string message)
            : base(message)
        {
        }

        public UnauthorizedTeamActionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
