using DefaultNamespace;

public class ConfigManager
{
    public string Domain { get; private set; }
    public string Model { get; private set; }

    public ConfigManager()
    {
        LoadConfigurations();
    }

    private void LoadConfigurations()
    {
        ConfigLoader configLoader = new ConfigLoader();
        Domain = configLoader.LoadDomainFromConfig();
        Model = configLoader.LoadModelFromConfig();
    }
}