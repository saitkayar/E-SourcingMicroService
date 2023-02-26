namespace Esourcing.Sourcing.Settings.Interfaces
{
    public interface ISourcingDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DataBaseName { get; set; }
    }
}
