/**
 * TODO(developer): Uncomment these variables before running the sample.
 */
const subscriptionNameOrId = 'dotnet-nodejs-sub';
const timeout = 60;

// Imports the Google Cloud client library
const {PubSub, Schema, Encodings} = require('@google-cloud/pubsub');


// var obj = proto.HelloWorldDotNetMessage();

// And the protobufjs library
const protobuf = require('protobufjs');

// Creates a client; cache this for further use
const pubSubClient = new PubSub();

function listenForProtobufMessages() {
    // References an existing subscription
    const subscription = pubSubClient.subscription(subscriptionNameOrId);

    // Make an decoder using the protobufjs library.
    //
    // Since we're providing the test message for a specific schema here, we'll
    // also code in the path to a sample proto definition.
    const ce = protobuf.loadSync('cloudevents.proto');
    const CloudEvent = ce.lookupType('io.cloudevents.v1.CloudEvent')

    const root = protobuf.loadSync('messages.proto');

    // Create an event handler to handle messages
    let messageCount = 0;
    
    const messageHandler = message => {

        message.ack();
        console.log(`Validation of message: ${  CloudEvent.verify(message)}`);
      
        // var parsedMessage = obj.parse(message.data.toString);

        // console.log(`Parsed message: ${parsedMessage}`);

        const Province = root.lookupType('HelloWorldDotNetMessage');
        
        result = JSON.parse(message.data.toString());

        Province.create()
        console.log(`Validation of JSON: ${Province.verify(message)}`);

        console.log(`Received message ${message.id}:`);
        console.log(`\tData: ${JSON.stringify(result, null, 4)}`);
        console.log(`\tAttributes: ${JSON.stringify(message.attributes, null, 4)}`);
        messageCount += 1;

        // "Ack" (acknowledge receipt of) the message
        message.ack();
    };

    // Listen for new messages until timeout is hit
    subscription.on('message', messageHandler);

    setTimeout(() => {
        subscription.removeListener('message', messageHandler);
        console.log(`${messageCount} message(s) received.`);
    }, timeout * 1000);
}

listenForProtobufMessages();