using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MJFramework.Properties;


namespace MJFramework.mj.connections.utils
{
    public class MJAppClass
    {

        public Configuration config;


        public static void changeDataServer(string localhost, string user, string pass, string databasename)
        {
            String cadenaNueva = "server=" + localhost + ";user id=" + user + ";password=" + pass + ";persistsecurityinfo=True;database="+ databasename;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings["ServerDb"].ConnectionString = cadenaNueva;
            config.Save(ConfigurationSaveMode.Modified, true);
            Settings.Default.Save();
            ConnectionStringSettingsCollection settings =
            ConfigurationManager.ConnectionStrings;
            if (settings != null)
            {
                foreach (ConnectionStringSettings cs in settings)
                {
                    Console.WriteLine(cs.Name);
                    Console.WriteLine(cs.ProviderName);
                    Console.WriteLine(cs.ConnectionString);
                }
            }

        }

    }
}
