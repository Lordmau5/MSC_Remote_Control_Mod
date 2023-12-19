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

        private bool _isReconnecting = false;
        
        public override void ModSetup()
        {
            this.SetupFunction(Setup.OnLoad, this.Mod_OnLoad);
            this.SetupFunction(Setup.OnGUI, this.Mod_OnGUI);
            this.SetupFunction(Setup.Update, this.Mod_Update);
            this.SetupFunction(Setup.ModSettings, this.Mod_Settings);
            this.SetupFunction(Setup.FixedUpdate, this.Mod_FixedUpdate);

            WebSocketClient.OnMessageReceived += this.OnMessageReceived;
            WebSocketClient.OnOpened += this.OnWebSocketOpened;
        }

        private void OnWebSocketOpened(object sender)
        {
            if (!this._isReconnecting) return;

            ModUI.ShowMessage("WebSocket successfully reconnected.", "WebSocket connected");
            this._isReconnecting = false;
        }

        private void Mod_Settings()
        {
            // All settings should be created here. 
            // DO NOT put anything that isn't settings or keybinds in here!
            Settings.AddHeader(this, "WebSocket");
            Settings.AddButton(this, "reconnect_websocket", "Reconnect WebSocket", ReconnectWebSocket);

            Settings.AddHeader(this, "Stats");
            Settings.AddButton(this, "request_thirst", "Set Max Thirst", () => WebSocketClient.Send("request_thirst"));
            Settings.AddButton(this, "request_hunger", "Set Max Hunger", () => WebSocketClient.Send("request_hunger"));
            Settings.AddButton(this, "request_stress", "Set Max Stress", () => WebSocketClient.Send("request_stress"));
            Settings.AddButton(this, "request_urine", "Set Max Urine", () => WebSocketClient.Send("request_urine"));
            Settings.AddButton(this, "request_fatigue", "Set Max Fatigue", () => WebSocketClient.Send("request_fatigue"));
            Settings.AddButton(this, "request_dirtiness", "Set Max Dirtiness", () => WebSocketClient.Send("request_dirtiness"));
            Settings.AddButton(this, "request_drunk", "Set Max Alcohol", () => WebSocketClient.Send("request_drunk"));
            
            Settings.AddButton(this, "request_swear", "Swear", () => WebSocketClient.Send("request_swear"));
            Settings.AddButton(this, "request_blind", "Set Blind", () => WebSocketClient.Send("request_blind"));
            
            Settings.AddHeader(this, "Vehicle");
            Settings.AddButton(this, "steer_reset", "Steer Reset", () => WebSocketClient.Send("steer_reset"));
            Settings.AddButton(this, "steer_left", "Steer Left", () => WebSocketClient.Send("steer_left"));
            Settings.AddButton(this, "steer_right", "Steer Right", () => WebSocketClient.Send("steer_right"));
            
            Settings.AddButton(this, "accel_reset", "Accel Reset", () => WebSocketClient.Send("accel_reset"));
            Settings.AddButton(this, "accel_max", "Accel Max", () => WebSocketClient.Send("accel_max"));
            
            Settings.AddButton(this, "brake_reset", "Brake Reset", () => WebSocketClient.Send("brake_reset"));
            Settings.AddButton(this, "brake_max", "Brake Max", () => WebSocketClient.Send("brake_max"));
            
            Settings.AddButton(this, "no_fuel", "No Fuel (Satsuma)", () => WebSocketClient.Send("no_fuel"));
            Settings.AddButton(this, "max_fuel", "Max Fuel (Satsuma)", () => WebSocketClient.Send("max_fuel"));
        }

        private void ReconnectWebSocket()
        {
            _isReconnecting = true;
            WebSocketClient.Reconnect();
        }

        private void SetupWebSocket() => WebSocketClient.Setup("ws://localhost:8050");

        private void Mod_OnLoad() => this.SetupWebSocket();

        private void OnMessageReceived(object sender, WebSocketMessage msg)
        {
            if (PlayerNeedsHandler.Process(msg)) return;
            if (VehicleControlsHandler.Process(msg)) return;
            
            if (msg.Type == "ping")
            {
                WebSocketClient.Send($"Returning: {msg.Message}");
            }
        }

        private void Mod_OnGUI()
        {
            // Draw unity OnGUI() here
        }
        private void Mod_Update()
        {
            // Update is called once per frame
        }

        private void Mod_FixedUpdate()
        {
            PlayerNeedsHandler.OnUpdate();
            VehicleControlsHandler.OnUpdate();
        }
    }
}
