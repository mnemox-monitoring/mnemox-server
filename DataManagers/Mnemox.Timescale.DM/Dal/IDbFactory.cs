namespace Mnemox.Timescale.DM.Dal
{
    public interface IDbFactory
    {
        string CreateConnectionString(string databaseAddress, string username, string password, int? port = 5432, string database = "mnemox");
        IDbBase GetDbBase(string connectionString = null);
    }
}