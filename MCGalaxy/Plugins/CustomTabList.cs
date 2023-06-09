using System;
using System.IO;

using MCGalaxy;
using MCGalaxy.Config;
using MCGalaxy.Events.EntityEvents;

namespace MCGalaxy
{
    public class CustomTabList : Plugin
    {
        public override string creator { get { return "Venk"; } }
        public override string MCGalaxy_Version { get { return "1.9.3.4"; } }
        public override string name { get { return "CustomTabList"; } }

        public static string path = "./Plugins/CustomTabList";

        public class Config
        {
            [ConfigString("syntax", "Extra", "[username]")]
            public static string Syntax = "[username]";

            static ConfigElement[] cfg;
            public void Load()
            {
                if (cfg == null) cfg = ConfigElement.GetAll(typeof(Config));
                ConfigElement.ParseFile(cfg, path + "/config.properties", this);
            }

            public void Save()
            {
                if (cfg == null) cfg = ConfigElement.GetAll(typeof(Config));
                ConfigElement.SerialiseSimple(cfg, path + "/config.properties", this);
            }
        }

        public static void MakeConfig()
        {
            using (StreamWriter w = new StreamWriter(path + "/config.properties"))
            {
                w.WriteLine("# Edit the settings below to modify how the plugin operates.");
                w.WriteLine("# The syntex you wish to use for the tab list.");
                w.WriteLine("# Use &[colour code] to use colour codes.");
                w.WriteLine("# Valid variables are: [nick], [username], [color], [title], [money], [team], [muted], [afk]");
                w.WriteLine("syntax = [username]");
                w.WriteLine();
            }
        }

        public static Config cfg = new Config();

        public override void Load(bool startup)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!File.Exists(path + "/config.properties")) MakeConfig();

            // Initialize config
            cfg.Load();

            OnTabListEntryAddedEvent.Register(HandleTabListEntryAdded, Priority.High);
        }

        void HandleTabListEntryAdded(Entity entity, ref string name, ref string group, Player p)
        {
            name = Config.Syntax
                .Replace("[nick]", p.ColoredName)
                .Replace("[username]", p.truename)
                .Replace("[color]", p.color)
                .Replace("[title]", p.title)
                .Replace("[money]", p.money.ToString())
                .Replace("[team]", p.Game.Team != null ? p.Game.Team.Name : "")
                .Replace("[muted]", p.muted ? "(muted)" : "")
                .Replace("[afk]", p.IsAfk ? "(afk)" : "");
        }

        public override void Unload(bool shutdown)
        {
            OnTabListEntryAddedEvent.Unregister(HandleTabListEntryAdded);
        }
    }
}
