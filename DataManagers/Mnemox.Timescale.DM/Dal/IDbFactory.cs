namespace Mnemox.Timescale.DM.Dal
{
    public interface IDbFactory
    {
        DbBase GetDbBase();
    }
}