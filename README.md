1.Insert connection string to databbase in appsettings.json  <br/>
2.Insert your SMTP server settings in smtpConfiguration.json file <br/>
3.Open the project folder in the console
4.Copy and paste these commands sequentially "dotnet tool install --global dotnet-ef", "dotnet ef migrations add initialDb", "dotnet ef database update"
5.In order to run applications you need to use this command "dotnet run"