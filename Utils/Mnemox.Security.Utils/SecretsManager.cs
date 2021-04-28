using System;

namespace Mnemox.Security.Utils
{
    public class SecretsManager : ISecretsManager
    {
        public string GenerateToken()
        {
            var token = $"{CteateTokenPart()}{CteateTokenPart()}{CteateTokenPart()}";

            return token;
        }

        private string CteateTokenPart()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }
    }
}
