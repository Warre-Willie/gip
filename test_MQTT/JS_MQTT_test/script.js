const broker = "";
const broker_port = 8884;
const username = "";
const password = "";

const topic = 'gitok/test';


// Connect to your MQTT broker over WebSocket with username and password
const client = mqtt.connect(`wss://${broker}:${broker_port}/mqtt`, {
  username: username,
  password: password
});

// Reassurance that the connection worked
client.on('connect', () => {
  console.log('Connected!');

  client.subscribe(topic, (err) => {
    if (!err) {
      console.log(`Subscribed to ${topic}`);
    }
  });
});

// Handle incoming messages
client.on('message', (topic, message) => {
  var message = message.toString();
  console.log('Received message:', topic, message);

  document.getElementById('myLabel').textContent = topic;
  document.getElementById('myInput').value = message;
});
