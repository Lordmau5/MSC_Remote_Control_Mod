import WebSocket from "ws";

const ws = new WebSocket("ws://localhost:8050");

ws.on('open', function open() {
	  ws.send('something');
});