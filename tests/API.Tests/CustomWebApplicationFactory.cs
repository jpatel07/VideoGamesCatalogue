using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Tests;
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{

    //should be configured in secrets 
    private const string DockerConnectionString =
        "Data Source=localhost;Database=VGCatalogueTestDb;User ID=sa;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=True;Application Name=VGCAPI.Tests;";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DBConnection"] = DockerConnectionString
            });
        });

        return base.CreateHost(builder);
    }
}
