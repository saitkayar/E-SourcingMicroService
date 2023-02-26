using Esourcing.Sourcing.Settings.Interfaces;

namespace Esourcing.Sourcing.Settings
{
    public class SourcingDatabaseSettings : ISourcingDatabaseSettings
    {
        public string ConnectionString { get ; set ; }
        public string DataBaseName { get; set; }
    }
}
