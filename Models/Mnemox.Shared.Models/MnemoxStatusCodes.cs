namespace Mnemox.Shared.Models
{
    public enum MnemoxStatusCodes
    {
        UNAUTHORIZED = 401,
        NOT_FOUND = 404,
        INTERNAL_SERVER_ERROR = 500,
        INVALID_MODEL = 4000,
        INVALID_USERNAME_OR_PASSWORD = 4010,
        CANNOT_CONNECT_TO_THE_DATABASE = 8000
    }
}
