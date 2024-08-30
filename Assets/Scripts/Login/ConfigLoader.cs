// --------------------------------------------------------------------------------------------------------------------
// Copyright (C) 2023 TH Köln – University of Applied Sciences

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
// --------------------------------------------------------------------------------------------------------------------

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