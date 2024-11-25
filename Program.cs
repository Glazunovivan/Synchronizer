using Synchronizer.Application;
using Synchronizer.Services;
namespace Synchronizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var settings = SettingLoader.GetSettings();
            var dataservice = new DataService(settings.DbConnectionString);
            var sync = new Synchronize(settings);

            while (true)
            {
                try
                {
                    sync.Sync();
                }
                catch (Exception ex)
                {
                    //что делать тут?
                }

                //проверка по времени
                Thread.Sleep((settings.TimeSync * 1000));   //ждем в секундах
            }
        }
    }
}
