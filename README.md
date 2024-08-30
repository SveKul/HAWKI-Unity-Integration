# About
HAWKI-Unity-Integration is a Unity client for the [HAWKI](https://github.com/HAWK-Digital-Environments/HAWKI) didactic interface for universities based on the OpenAI API.
The application allows access to HAWKI instances from within Unity. This enables the use of OpenAI's API within software projects at the respective university to further process generated texts or integrate contextual information in requests. A valid university ID is required for use.

![Login](https://github.com/user-attachments/assets/8c7edf35-6f58-4a24-b098-6608fa5dd738)


![Chat](https://github.com/user-attachments/assets/d09003e9-96ad-4933-95bb-e92f1da8873d)


# Prerequisites
To use, it is necessary to specify both the model being used and the subdomain of the HAWKI interface within the respective university in the config.txt file.

e.g.
Domain=https://ki.th-koeln.de
Model=gpt-4o

Unity Version: 2022.3.30f1

# Unity Configuration
The application uses Newtonsoft.Json to deserialize the received JSON chunks. It is therefore necessary to install the corresponding Unity package.

- Open unity's Package Manager (Window -> Package Manager)
- Click on the "+" button. Select "Update package by name..."
- Enter com.unity.nuget.newtonsoft-json
- The package gets installed
- Then in the C# file : using Newtonsoft.Json;
