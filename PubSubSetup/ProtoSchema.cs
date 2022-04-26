using Google.Cloud.PubSub.V1;
using Grpc.Core;

namespace PubSubSetup;

public class ProtoSchema
{
    public Schema Create(string projectId, string schemaId, string pathToDefinition)
    {
        SchemaServiceClient schemaService = SchemaServiceClient.Create();
        var schemaName = SchemaName.FromProjectSchema(projectId, schemaId);
        string schemaDefinition = File.ReadAllText(pathToDefinition);
        Schema schema = new Schema
        {
            Name = schemaName.ToString(),
            Type = Schema.Types.Type.ProtocolBuffer,
            Definition = schemaDefinition
        };
        CreateSchemaRequest createSchemaRequest = new CreateSchemaRequest
        {
            Parent = "projects/" + projectId,
            SchemaId = schemaId,
            Schema = schema
        };

        try
        {
            schema = schemaService.CreateSchema(createSchemaRequest);
            Console.WriteLine($"Schema {schema.Name} created.");
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            Console.WriteLine($"Schema {schemaName} already exists.");
        }
        return schema;
    }
}