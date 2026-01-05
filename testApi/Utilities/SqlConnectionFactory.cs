
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;


namespace testApi.Utilities;

public class SqlConnectionFactory
{

    // Azure SQL resource ID scope for Entra tokens
    private const string SqlScope = "https://database.windows.net/.default";

    /// <summary>
    /// Creates and opens a SqlConnection using an Entra ID access token.
    /// Works with Managed Identity in Azure and dev credentials locally.
    /// </summary>
    public static async Task<SqlConnection> CreateAsync(string baseConnectionString, CancellationToken ct = default)
    {
        // DefaultAzureCredential order: Managed Identity (Azure), Env, CLI, VS...
        var credential = new DefaultAzureCredential();

        AccessToken token = await credential.GetTokenAsync(
            new TokenRequestContext(new[] { SqlScope }), ct);

        var conn = new SqlConnection(baseConnectionString)
        {
            AccessToken = token.Token
        };

        await conn.OpenAsync(ct);
        return conn;
    }


}
