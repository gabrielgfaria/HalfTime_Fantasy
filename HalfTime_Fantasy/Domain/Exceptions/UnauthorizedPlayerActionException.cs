namespace Domain.Exceptions
{
    public class UnauthorizedPlayerActionException : Exception
    {
        public UnauthorizedPlayerActionException()
        {
        }

        public UnauthorizedPlayerActionException(string message)
            : base(message)
        {
        }

        public UnauthorizedPlayerActionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
