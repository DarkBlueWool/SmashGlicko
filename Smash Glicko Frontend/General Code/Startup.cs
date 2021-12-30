namespace Smash_Glicko_Frontend.Shortcuts
{
    public class Startup
    {
        //For now the Smash.gg AuthToken is put into AppData. Will likely change later for security.
        public static string SGAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SmashGlicko");
        public static string SGSettingsFolderPath = Path.Combine(SGAppDataPath, "Settings");
        public static string DefaultAuthSettingsFile = "SmashGGAuthToken = XXXXXXXXXXXXXXXXXXX\n";
        public static void VerifyAppData()
        {
            Directory.CreateDirectory(SGAppDataPath);
            Directory.CreateDirectory(Path.Combine(SGAppDataPath, "Settings"));

            if (!File.Exists(Path.Combine(SGSettingsFolderPath, "AuthSettings.txt")))
            {
                File.WriteAllText(Path.Combine(SGSettingsFolderPath, "AuthSettings.txt"), DefaultAuthSettingsFile);
            }

        }
    }
}
