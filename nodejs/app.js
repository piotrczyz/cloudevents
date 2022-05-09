/**
 * TODO(developer): Uncomment these variables before running the sample.
 */
const subscriptionNameOrId = 'dotnet-nodejs-sub';
const timeout = 60;

// Imports the Google Cloud client library
const {PubSub, Schema, Encodings} = require('@google-cloud/pubsub');

// Creates a client; cache this for further use
const pubSubClient = new PubSub();

// And the protobufjs library
const protobuf = require('protobufjs');
const serializer = require('proto3-json-serializer');

const ceProto = protobuf.loadSync('cloudevents.proto');
const CloudEventType = ceProto.lookupType('io.cloudevents.v1.CloudEvent')

const msgProto = protobuf.loadSync('messages.proto');


async function listenForProtobufMessages(subscriptionNameOrId, timeout) {

    let messageCount = 0;
    const subscription = pubSubClient.subscription(subscriptionNameOrId);
    
    const messageHandler = async message => {

        message.ack();
        let result;

        console.log(message);
        
        if (!message){
            console.log("Messages is null")
            return;
        }

        const serialziedMessage = serializer.fromProto3JSON(CloudEventType, message);
      
        console.log(`Type ${serialziedMessage.type}`);
        const MsgType = msgProto.lookupType(serialziedMessage.type);

        if (!serialziedMessage || !serialziedMessage.data.toString()){
            console.log("Data is null")
            return;
        }
        console.log(serialziedMessage.data.toString());
        const payload = serializer.fromProto3JSON(MsgType, serialziedMessage.data.toString());

        
        console.log(`Received message ${message.id}:`);
        console.log(`\tData: ${JSON.stringify(payload, null, 4)}`);
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

listenForProtobufMessages(subscriptionNameOrId, 10);