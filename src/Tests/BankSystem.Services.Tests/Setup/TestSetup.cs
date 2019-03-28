namespace BankSystem.Services.Tests.Setup
{
    using AutoMapper;
    using Common.AutoMapping.Profiles;

    public class TestSetup
    {
        private static readonly object Sync = new object();
        private static bool mapperInitialized = false;

        public static void InitializeMapper()
        {
            lock (Sync)
            {
                if (!mapperInitialized)
                {
                    Mapper.Initialize(config =>
                    {
                        config.AddProfile<DefaultProfile>();
                    });

                    mapperInitialized = true;
                }
            }
        }
    }
}
