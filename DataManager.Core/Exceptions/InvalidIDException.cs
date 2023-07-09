namespace Exceptions
{
    public class InvalidIDException : ArgumentException
    {
        public InvalidIDException()
        {
        }

        public InvalidIDException(string message) : base(message)
        {
        }

        public InvalidIDException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}