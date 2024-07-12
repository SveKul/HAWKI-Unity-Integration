using System;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public class ConfigLoader
    {
        private string _domain;
        private string _model;
        
        
        public string LoadDomainFromConfig()
        {
            try
            {
                // Read the configuration file
                string configPath = Path.Combine(Application.dataPath, "config.txt");
                string[] configLines = File.ReadAllLines(configPath);

                foreach (string line in configLines)
                {
                    if (line.StartsWith("Domain="))
                    {
                        _domain = line.Substring("Domain=".Length);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(_domain))
                {
                    Debug.LogError("Domain not found in config file!");
                    return null;
                }
                
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to read config file: " + e.Message);
            }

            return _domain;
        }

        public string LoadModelFromConfig()
        {
            try
            {
                // Read the configuration file
                string configPath = Path.Combine(Application.dataPath, "config.txt");
                string[] configLines = File.ReadAllLines(configPath);

                foreach (string line in configLines)
                {
                    if (line.StartsWith("Model="))
                    {
                        _domain = line.Substring("Model=".Length);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(_domain))
                {
                    Debug.LogError("Model not found in config file!");
                    return null;
                }
                
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to read config file: " + e.Message);
            }

            return _domain;
        }
    }
}