using MSCLoader;

namespace MSC_Remote_Control_Mod
{
    public class MscRemoteControlMod : Mod
    {
        public override string ID => "MSC_Remote_Control_Mod"; // Your (unique) mod ID 
        public override string Name => "Remote Control Mod"; // Your mod name
        public override string Author => "Lordmau5"; // Name of the Author (your name)
        public override string Version => "1.0"; // Version
        public override string Description => "A mod to remotely control aspects of the game through a websocket."; // Short description of your mod

        private bool isReconnecting = false;
        private WebSocketClient webSocketClient;

        public override void ModSetup()
        {
            this.SetupFunction(Setup.OnLoad, this.Mod_OnLoad);
            this.SetupFunction(Setup.OnGUI, this.Mod_OnGUI);
            this.SetupFunction(Setup.Update, this.Mod_Update);
            this.SetupFunction(Setup.ModSettings, this.Mod_Settings);
            this.SetupFunction(Setup.FixedUpdate, this.Mod_FixedUpdate);

            this.webSocketClient = new WebSocketClient("ws://localhost:8050");

            this.webSocketClient.OnMessageReceived += this.OnMessageReceived;
            this.webSocketClient.OnOpened += this.OnWebSocketOpened;
        }

        private void OnWebSocketOpened(object sender)
        {
            if (!this.isReconnecting) return;

            ModUI.ShowMessage("WebSocket successfully reconnected.", "WebSocket connected");
            this.isReconnecting = false;
        }

        private void Mod_Settings()
        {
            // All settings should be created here. 
            // DO NOT put anything that isn't settings or keybinds in here!
            Settings.AddHeader(this, "WebSocket");
            Settings.AddButton(this, "reconnect_websocket", "Reconnect WebSocket", this.ReconnectWebSocket);

            Settings.AddHeader(this, "Stats");
            Settings.AddButton(this, "request_thirst", "Set Max Thirst", () => this.webSocketClient.Send("request_thirst"));
            Settings.AddButton(this, "request_hunger", "Set Max Hunger", () => this.webSocketClient.Send("request_hunger"));
            Settings.AddButton(this, "request_stress", "Set Max Stress", () => this.webSocketClient.Send("request_stress"));
            Settings.AddButton(this, "request_urine", "Set Max Urine", () => this.webSocketClient.Send("request_urine"));
            Settings.AddButton(this, "request_fatigue", "Set Max Fatigue", () => this.webSocketClient.Send("request_fatigue"));
            Settings.AddButton(this, "request_dirtiness", "Set Max Dirtiness", () => this.webSocketClient.Send("request_dirtiness"));
            Settings.AddButton(this, "request_drunk", "Set Max Alcohol", () => this.webSocketClient.Send("request_drunk"));
            
            Settings.AddButton(this, "request_swear", "Swear", () => this.webSocketClient.Send("request_swear"));
            Settings.AddButton(this, "request_blind", "Set Blind", () => this.webSocketClient.Send("request_blind"));
            
            Settings.AddHeader(this, "Vehicle");
            Settings.AddButton(this, "steer_reset", "Steer Reset", () => this.webSocketClient.Send("steer_reset"));
            Settings.AddButton(this, "steer_left", "Steer Left", () => this.webSocketClient.Send("steer_left"));
            Settings.AddButton(this, "steer_right", "Steer Right", () => this.webSocketClient.Send("steer_right"));
            
            Settings.AddButton(this, "accel_reset", "Accel Reset", () => this.webSocketClient.Send("accel_reset"));
            Settings.AddButton(this, "accel_max", "Accel Max", () => this.webSocketClient.Send("accel_max"));
            
            Settings.AddButton(this, "brake_reset", "Brake Reset", () => this.webSocketClient.Send("brake_reset"));
            Settings.AddButton(this, "brake_max", "Brake Max", () => this.webSocketClient.Send("brake_max"));
        }

        private void ReconnectWebSocket()
        {
            this.isReconnecting = true;

            this.webSocketClient?.Close();
            this.SetupWebSocket();
        }

        private void SetupWebSocket() => this.webSocketClient.Connect();

        private void Mod_OnLoad() => this.SetupWebSocket();

        private void OnMessageReceived(object sender, WebSocketMessage msg)
        {
            if (PlayerNeedsHandler.Process(msg)) return;
            if (VehicleControlsHandler.Process(msg)) return;
            
            if (msg.Type == "ping")
            {
                this.webSocketClient.Send($"Returning: {msg.Message}");
            }
            // Also ask on the Discord how to modify the car steering perhaps?
        }

        private void Mod_OnGUI()
        {
            // Draw unity OnGUI() here
        }
        private void Mod_Update()
        {
            // Update is called once per frame
            // VehicleControlsHandler.OnUpdate();
        }

        private void Mod_FixedUpdate()
        {
            PlayerNeedsHandler.OnUpdate();
            VehicleControlsHandler.OnUpdate();
        }
    }
}
