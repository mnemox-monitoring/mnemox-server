using Mnemox.Shared.Models.Enums;
using System;

namespace Mnemox.Shared.Models
{
    public class OutputException : Exception
    {
        public int HttpStatusCode { get; set; }

        public MnemoxStatusCodes MnemoxStatusCode { get; set; }

        public OutputException(
            Exception exception,
            int httpStatusCode,
            MnemoxStatusCodes mnemoxStatusCode) : base(exception.Message, exception.InnerException)
        {
            HttpStatusCode = httpStatusCode;

            MnemoxStatusCode = mnemoxStatusCode;
        }
    }
}
