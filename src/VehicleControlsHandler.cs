using System;
using HutongGames.PlayMaker;
using Newtonsoft.Json;
using UnityEngine;

namespace MSC_Remote_Control_Mod
{
    public static class VehicleControlsHandler
    {
        private class VehicleControlsMessage
        {
            public string Type { get; set; }
            public float Value { get; set; }
            public int ResetAfter { get; set; }
        }
        
        private static FsmString _playerCurrentVehicle;
        
        private static string PlayerCurrentVehicle 
        {
            get
            {
                if (_playerCurrentVehicle == null)
                    _playerCurrentVehicle = PlayMakerGlobals.Instance.Variables.FindFsmString("PlayerCurrentVehicle");
                return _playerCurrentVehicle.Value;
            }
        }
        
        private static string CurrentVehicleKey 
        {
            get
            {
                switch (PlayerCurrentVehicle)
                {
                    case "Satsuma":
                        return "SATSUMA(557kg, 248)";
                    case "Jonnez":
                        return "JONNEZ ES(Clone)";
                    case "Kekmet":
                        return "KEKMET(350-400psi)";
                    case "Hayosiko":
                        return "HAYOSIKO(1500kg, 250)";
                    case "Gifu":
                        return "GIFU(750/450psi)";
                    case "Ruscko":
                        return "RCO_RUSCKO12(270)";
                    case "Ferndale":
                        return "FERNDALE(1630kg)";
                    case "Combine":
                        return "COMBINE(350-400psi)";
                    default:
                        return "";
                }
            }
        }

        private static float _steerModifier = 0.0f;
        private static float _accelModifier = 0.0f;
        private static float _brakeModifier = 0.0f;

        private static void SetSteeringOfCurrentCar()
        {
            if (CurrentVehicleKey == "") return;

            var acc = GameObject.Find(CurrentVehicleKey).GetComponent<AxisCarController>();
            if (!acc) return;
            
            // var dyn = GameObject.Find(CurrentVehicleKey).GetComponent<CarDynamics>();
            // if (!dyn) return;

            acc.steerInput += _steerModifier;
            acc.throttleInput += _accelModifier;
            acc.brakeInput += _brakeModifier;

            // TODO lol
            // dyn.forceFeedback += (_steerModifier * -1) * 720;
        }

        public static bool Process(WebSocketMessage msg)
        {
            if (msg.Type != "vehicle") return false;
            
            var controlsMsg = JsonConvert.DeserializeObject<VehicleControlsMessage>(msg.Message);
            
            switch (controlsMsg.Type)
            {
                case "steer":
                {
                    _steerModifier = controlsMsg.Value;
                    if (controlsMsg.ResetAfter <= 0) return true;
                    
                    ResetHandler.ResetAfter(
                        DateTime.Now.AddSeconds(controlsMsg.ResetAfter),
                        () => _steerModifier = 0.0f
                    );
                    return true;
                }
                case "accel":
                {
                    _accelModifier = controlsMsg.Value;
                    if (controlsMsg.ResetAfter <= 0) return true;
                    
                    ResetHandler.ResetAfter(
                        DateTime.Now.AddSeconds(controlsMsg.ResetAfter),
                        () => _accelModifier = 0.0f
                    );
                    return true;
                }
                case "brake":
                {
                    _brakeModifier = controlsMsg.Value;
                    if (controlsMsg.ResetAfter <= 0) return true;
                    
                    ResetHandler.ResetAfter(
                        DateTime.Now.AddSeconds(controlsMsg.ResetAfter),
                        () => _brakeModifier = 0.0f
                    );
                    return true;
                }
            }

            return false;
        }

        public static void OnUpdate()
        {
            SetSteeringOfCurrentCar();
        }
    }
}