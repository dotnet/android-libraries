using Android.Runtime;
using AndroidX.Sqlite.Db;

namespace AndroidX.Sqlite.Driver
{
    public partial class SupportSQLiteDriver
    {
        // Explicit interface implementation to resolve return type mismatch
        ISQLiteConnection ISQLiteDriver.Open(string path)
        {
            return this.Open(path);
        }
    }
}