package main

import (
	"context"
	"fmt"

	"cloud.google.com/go/pubsub"
	"rsc.io/quote"
)

func main() {
	fmt.Println(quote.Go())

	publish("Message published from GoLang application")
}

func publish(msg string) error {
	projectID := "bcc-pubsub-example"
	topicID := "golang-application"
	// msg := "Hello World"
	ctx := context.Background()
	client, err := pubsub.NewClient(ctx, projectID)
	if err != nil {
		return fmt.Errorf("pubsub.NewClient: %v", err)
	}
	defer client.Close()

	t := client.Topic(topicID)
	result := t.Publish(ctx, &pubsub.Message{
		Data: []byte(msg),
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
