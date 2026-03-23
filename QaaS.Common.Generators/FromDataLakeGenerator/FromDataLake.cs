using System.Collections.Immutable;
using System.Data;
using System.Text.Json.Nodes;
using QaaS.Common.Generators.ConfigurationObjects.FromDataLakeConfigurations;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Generator;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;
using Trino.Client.Auth;
using Trino.Data.ADO.Server;

namespace QaaS.Common.Generators.FromDataLakeGenerator;

/// <summary>
/// Retrieves rows from the configured data lake query and exposes each row as a generated JSON object.
/// </summary>
public class FromDataLake: BaseGenerator<FromDataLakeConfiguration>
{
    public override IEnumerable<Data<object>> Generate(IImmutableList<SessionData> sessionDataList, 
        IImmutableList<DataSource> dataSourceList)
    {
        ITrinoAuth auth = new LDAPAuth {User = Configuration.Username, Password = Configuration.Password};
        var serverUri = new Uri(Configuration.TrinoServerUri);

        var clientTags = new HashSet<string> { Configuration.ClientTag };
            
        var properties = new TrinoConnectionProperties
        {
            Catalog = Configuration.Catalog,
            Server = serverUri,
            ClientTags = clientTags,
            Auth = auth
        };

        using var connection = new TrinoConnection(properties);
        using var command = new TrinoCommand(connection, statement:Configuration.Query);
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            var row = new JsonObject();
            for (var col = 0; col < reader.FieldCount; col++)
            {
                var columnName = reader.GetName(col);
                if (Configuration.ColumnsToIgnore.Contains(columnName)) continue;

                var value = reader.GetValue(col);
                row[columnName] = value == DBNull.Value ? null : JsonValue.Create(value);
            }
            yield return new Data<object>
            {
                Body = row
            };
        }
        if (connection.State != ConnectionState.Closed)
            connection.Close();
    }
}
