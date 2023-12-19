using System;
using MSCLoader;
using Newtonsoft.Json;
using UnityEngine;

namespace MSC_Remote_Control_Mod
{
    public static class PlayerNeedsHandler
    {
        private class PlayerNeedsMessage
        {
            public string Need { get; set; }
            public float Value { get; set; }
            public int ResetAfter { get; set; }
        }
        
        private static void SetAndResetValue(string valueName, PlayerNeedsMessage msg)
        {
            var oldValue = PlayMakerGlobals.Instance.Variables.GetFsmFloat(valueName).Value;
            PlayMakerGlobals.Instance.Variables.GetFsmFloat(valueName).Value = msg.Value;

            if (msg.ResetAfter <= 0) return;
            
            ResetHandler.ResetAfter(
                DateTime.Now.AddSeconds(msg.ResetAfter),
                // () => PlayMakerGlobals.Instance.Variables.GetFsmFloat(valueName).Value = oldValue)
                // Reset to 0? We only do this for alcohol so that'd be an idea...
                () => PlayMakerGlobals.Instance.Variables.GetFsmFloat(valueName).Value = 0.0f
            );
        }
        
        public static bool Process(WebSocketMessage msg)
        {
            if (msg.Type != "player_needs") return false;
            
            var needsMsg = JsonConvert.DeserializeObject<PlayerNeedsMessage>(msg.Message);

            switch (needsMsg.Need)
            {
                case "thirst":
                {
                    SetAndResetValue("PlayerThirst", needsMsg);
                    return true;
                }
                case "hunger":
                {
                    SetAndResetValue("PlayerHunger", needsMsg);
                    return true;
                }
                case "stress":
                {
                    SetAndResetValue("PlayerStress", needsMsg);
                    return true;
                }
                case "urine":
                {
                    SetAndResetValue("PlayerUrine", needsMsg);
                    return true;
                }
                case "fatigue":
                {
                    SetAndResetValue("PlayerFatigue", needsMsg);
                    return true;
                }
                case "dirtiness":
                {
                    SetAndResetValue("PlayerDirtiness", needsMsg);
                    return true;
                }
                case "drunk":
                {
                    SetAndResetValue("PlayerDrunkAdjusted", needsMsg);
                    return true;
                }
                case "swear":
                {
                    _swearFsm.SendEvent("SWEARING");
                    return true;
                }
                case "blind":
                {
                    _blindnessFsm.SendEvent("BLIND");
                    ResetHandler.ResetAfter(
                        DateTime.Now.AddSeconds(needsMsg.ResetAfter),
                        () => _blindnessFsm.SendEvent("FINISHED")
                    );
                    return true;
                }
            }

            return false;
        }

        private static PlayMakerFSM _swearFsm;
        private static GameObject _camera;
        private static PlayMakerFSM _blindnessFsm;

        private static void EnsureFsMs()
        {
            _swearFsm = _swearFsm ? _swearFsm : GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/SpeakDatabase").GetComponent<PlayMakerFSM>();
            _camera = _camera ? _camera : GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera");
            _blindnessFsm = _blindnessFsm ? _blindnessFsm : _camera.GetPlayMaker("Blindness");
        }
        
        public static void OnUpdate()
        {
            EnsureFsMs();
        }
    }
}