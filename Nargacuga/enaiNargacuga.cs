using ImGuiNET;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.IO;
using System;
using System.Threading;

namespace enaiNargacuga
{
    public class enaiNargacuga : IPlugin
    {
        public string Name => "enaiNargacuga";
        public string Author => "Seka";

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private int _actionCounts;
        private bool _modEnabled = false;
        private bool _inQuest = false;
        private bool _transparentMode = false;
        private bool _nargaDies = false;
        private string _statusMessage = "";
        private int _frameCountdown = 0;
        private const int _framesForMessage = 60;
        private Monster? _monster;
        private uint _lastStage;
        public void OnMonsterCreate(Monster monster) { uint stageID = (uint)Area.CurrentStage; _lastStage = stageID; }
        public void OnQuestLeave(int questId) { _monster = null; _inQuest = false; _actionCounts = 0; _transparentMode = false; }
        public void OnQuestComplete(int questId) { _monster = null; _inQuest = false; _actionCounts = 0; _transparentMode = false; }
        public void OnQuestFail(int questId) { _monster = null; _inQuest = false; _actionCounts = 0; _transparentMode = false; }
        public void OnQuestReturn(int questId) { _monster = null; _inQuest = false; _actionCounts = 0; _transparentMode = false; }
        public void OnQuestAbandon(int questId) { _monster = null; _inQuest = false; _actionCounts = 0; _transparentMode = false; }
        public void OnQuestEnter(int questId) { _monster = null; _inQuest = true; _actionCounts = 0; _transparentMode = false; }
        public void OnMonsterDestroy(Monster monster) { 
            if (_monster != null && _monster.Type == MonsterType.Nargacuga) { 
                _monster = null; _transparentMode = false; _actionCounts = 0;
            } 
        }
        public void OnMonsterDeath(Monster monster) { 
            if (_monster != null && _monster.Type == MonsterType.Nargacuga) { 
                _monster = null; _nargaDies = true; _actionCounts = 0;
            } 
        }
        public void OnMonsterEnrage(Monster monster)
        {
            if (_monster != null && _monster.Type == MonsterType.Nargacuga) { _actionCounts = 15; }
        }
        public void OnMonsterUnenrage(Monster monster)
        {
            if (_monster != null && _monster.Type == MonsterType.Nargacuga) { _actionCounts = 0; }
        }

        public void OnMonsterAction(Monster monster, ref int actionId)
        {
            if (_monster != null && _monster != null && _monster.Type == MonsterType.Nargacuga && _actionCounts > 0)  
            {
                if (_actionCounts == 12) { _transparentMode = true; } if (_actionCounts == 9) { _transparentMode = false; } if (_actionCounts == 8) { _transparentMode = true; }
                if (_actionCounts == 6) { _transparentMode = false; } if (_actionCounts == 4) { _transparentMode = true; } if (_actionCounts == 2) { _transparentMode = false; }
                if (_actionCounts == 1) { _actionCounts += 14; } _actionCounts -= 1;
            }
            if (_monster != null && _monster.Type == MonsterType.Nargacuga && _actionCounts == 0)
            {
                _transparentMode = false;
            }
        }

        public void OnImGuiRender()
        {
            var player = Player.MainPlayer;
            if (player == null) return;
            if (ImGui.Button("LUNARUGA"))
            {
                if (_inQuest) {
                    _statusMessage = "Cannot toggle while in quest.";
                } else {
                    if (_modEnabled) 
                    { 
                        _modEnabled = false; _statusMessage = "LuNarga disabled."; 
                    }
                    else
                    {
                        _modEnabled = true; _statusMessage = "LuNarga enabled.";
                    }
                }
                _frameCountdown = _framesForMessage;
            }
            if (_frameCountdown > 0)
            {
                ImGui.Text(_statusMessage);
                _frameCountdown--;
            }
        }
        public void OnUpdate(float deltaTime)
        {
            var player = Player.MainPlayer;
            if (player == null) return;
            if ((uint)Area.CurrentStage != _lastStage)
            {
                _monster = null;
            }
            var monsters = Monster.GetAllMonsters().TakeLast(5).ToArray();
            foreach (var monster in monsters)
            {
                if (monster.Type == MonsterType.Nargacuga)
                {
                    _monster = monster;
                }
            }
            if (!_modEnabled) return;
            if (_monster == null)
                return;

            ref float transparency = ref _monster.GetRef<float>(0x314);

            if (transparency < 0.05f) { transparency = 0.05f; }
            else if (transparency > 1.0f) { transparency = 1.0f; }


            if (!_transparentMode) {
                if (transparency < 1f) { transparency += 0.025f; }
                else { transparency = 1f; }
            } else { 
                transparency -= 0.025f; 
            }

            if (_nargaDies) { _actionCounts = 0; _transparentMode = false; }
        }
    }
}