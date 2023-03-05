import { HubConnectionBuilder } from "@microsoft/signalr"

class QueueHub {
	constructor() {
		this.client = new HubConnectionBuilder()
			.withUrl(`${import.meta.env.VITE_API_URL}/queue`)
			.withAutomaticReconnect()
			.build();
	}

	start() {
		this.client.start();
	}
}

export default new QueueHub();