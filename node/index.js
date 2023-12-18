import { WebSocketServer } from 'ws';

const wss = new WebSocketServer({ port: 8050 });

function sendNeed(ws, need, value, resetAfter = 0) {
	ws.send(JSON.stringify({
		type: 'player_needs',
		message: JSON.stringify({
			need,
			value,
			resetAfter
		})
	}));
}

function sendVehicle(ws, type, value, resetAfter = 0) {
	ws.send(JSON.stringify({
		type: 'vehicle',
		message: JSON.stringify({
			type,
			value,
			resetAfter
		})
	}));
}

wss.on('connection', function connection(ws) {
	console.log('connection');
	ws.on('error', console.error);

	ws.on('message', function message(data) {
		const msg = data.toString();
		console.log('received', msg);

		switch(msg) {
			case 'request_thirst': {
				sendNeed(ws, 'thirst', 100.0);
				return;
			}
			case 'request_hunger': {
				sendNeed(ws, 'hunger', 100.0);
				return;
			}
			case 'request_stress': {
				sendNeed(ws, 'stress', 100.0);
				return;
			}
			case 'request_urine': {
				sendNeed(ws, 'urine', 100.0);
				return;
			}
			case 'request_fatigue': {
				sendNeed(ws, 'fatigue', 100.0);
				return;
			}
			case 'request_dirtiness': {
				sendNeed(ws, 'dirtiness', 100.0);
				return;
			}
			case 'request_drunk': {
				sendNeed(ws, 'drunk', 4.0, 10);
				return;
			}
			case 'steer_reset': {
				sendVehicle(ws, 'steer', 0);
				return;
			}
			case 'steer_left': {
				setTimeout(() => sendVehicle(ws, 'steer', -0.2), 1000 * 3);
				return;
			}
			case 'steer_right': {
				setTimeout(() => sendVehicle(ws, 'steer', 0.2), 1000 * 3);
				return;
			}
			case 'accel_reset': {
				sendVehicle(ws, 'accel', 0);
				return;
			}
			case 'accel_max': {
				sendVehicle(ws, 'accel', 1);
				return;
			}
			case 'brake_reset': {
				sendVehicle(ws, 'brake', 0);
				return;
			}
			case 'brake_max': {
				sendVehicle(ws, 'brake', 1);
				return;
			}
			default: {
				console.log('Unknown request', msg);
				return;
			}
		}
	});

	ws.send(JSON.stringify({ type: 'ping', message: new Date() }));
});