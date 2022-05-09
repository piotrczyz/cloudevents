package main

import (
	"context"
	"encoding/json"
	"fmt"

	"cloud.google.com/go/pubsub"
	cloudevents "github.com/cloudevents/sdk-go"
	"github.com/google/uuid"
)

func main() {
	publish("Message published from GoLang application")
}

func publish(msg string) error {
	projectID := "bcc-pubsub-example"
	topicID := "test2"
	// msg := "Hello World"
	ctx := context.Background()
	client, err := pubsub.NewClient(ctx, projectID)
	if err != nil {
		return fmt.Errorf("pubsub.NewClient: %v", err)
	}
	defer client.Close()

	t := client.Topic(topicID)
	data := createEvent(msg)

	if data == nil {
		return fmt.Errorf("Data is nil")
	}
	result := t.Publish(ctx, &pubsub.Message{
		Data: createEvent(msg),
		Attributes: map[string]string{
			"bccustron":       "true",
			"bccwroclaw":      "true",
			"tenantId_poland": "true",
		},
	})
	// Block until the result is returned and a server-generated
	// ID is returned for the published message.
	id, err := result.Get(ctx)
	if err != nil {
		return fmt.Errorf("Get: %v", err)
	}

	fmt.Printf("Published a message; msg ID: %v\n", id)
	return nil
}

func createEvent(msg string) []byte {
	event := cloudevents.NewEvent()
	event.SetID(uuid.New().String())
	event.SetSource("example/uri")
	event.SetType("example.type")

	data := HelloWorldMessage{msg}
	event.SetData(data)

	bytes, err := json.Marshal(event)
	if err != nil {
		println("Error", err.Error())
		return nil
	}

	return bytes
}

type HelloWorldMessage struct {
	Text string
}
