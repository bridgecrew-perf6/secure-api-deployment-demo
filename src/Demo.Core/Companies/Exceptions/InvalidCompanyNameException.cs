using System;

namespace Demo.Core.Companies.Exceptions
{
    public class InvalidCompanyNameException : Exception
    {
        public InvalidCompanyNameException(string value, Exception innerException = null)
        : base($"The name '{value}' for a company name is invalid. Company names must be at least 2 characters long", innerException)
        {

        }
    }
}
