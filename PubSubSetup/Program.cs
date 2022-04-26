// See https://aka.ms/new-console-template for more information

using PubSubSetup;

var schema = new ProtoSchema();
schema.Create("bcc-pubsub-example", "protobuf", "pubsub_schema.proto");

Console.WriteLine("ProtoSchema created");