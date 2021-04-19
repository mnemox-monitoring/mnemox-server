namespace Mnemox.Timescale.DM.Dal
{
    public interface IDbFactory
    {
        IDbBase GetDbBase();
    }
}