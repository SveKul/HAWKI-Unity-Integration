using DefaultNamespace;

public class ConfigManager
{
    public string LoadDomainFromConfig()
    {
        ConfigLoader configLoader = new ConfigLoader();
        return configLoader.LoadDomainFromConfig();
    }
}