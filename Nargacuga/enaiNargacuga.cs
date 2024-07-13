using ImGuiNET;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;

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
        private Monster? _monster = null;
        private uint _lastStage = 0;
        private void ResetState()
        {
            _monster = null;
            _actionCounts = 0;
            _transparentMode = false;
            _nargaDies = false;
        }
        public void OnMonsterCreate(Monster monster) 
        {
            _lastStage = (uint)Area.CurrentStage;
        }
        public void OnQuestLeave(int questId) { ResetState(); _inQuest = false; }
        public void OnQuestComplete(int questId) { ResetState(); _inQuest = false; }
        public void OnQuestFail(int questId) { ResetState(); _inQuest = false; }
        public void OnQuestReturn(int questId) { ResetState(); _inQuest = false; }
        public void OnQuestAbandon(int questId) { ResetState(); _inQuest = false; }
        public void OnQuestEnter(int questId) { ResetState(); _inQuest = true; }
        public void OnMonsterDestroy(Monster monster) 
        { 
            if (_monster != null && _monster.Type == MonsterType.Nargacuga && _monster == monster) 
            {
                ResetState();
            } 
        }
        public void OnMonsterDeath(Monster monster) 
        { 
            if (_monster != null && _monster.Type == MonsterType.Nargacuga && _monster == monster) 
            { 
                _nargaDies = true; 
                _actionCounts = 0;
                _monster = null;
            } 
        }
        public void OnMonsterEnrage(Monster monster)
        {
            if (_monster != null && _monster.Type == MonsterType.Nargacuga && _monster == monster)
            { 
                _actionCounts = 15; 
            }
        }
        public void OnMonsterUnenrage(Monster monster)
        {
            if (_monster != null && _monster.Type == MonsterType.Nargacuga && _monster == monster) 
            { 
                _actionCounts = 0; 
            }
        }

        public void OnMonsterAction(Monster monster, ref int actionId)
        {
            if (_monster != null && _monster.Type == MonsterType.Nargacuga && _monster == monster)  
            {
                if (_actionCounts > 0)
                {
                    if (_actionCounts == 12 || _actionCounts == 0 || _actionCounts == 4)
                    {
                        _transparentMode = true;
                    }
                    if (_actionCounts == 9 || _actionCounts == 6 || _actionCounts == 2)
                    {
                        _transparentMode = false;
                    }
                    if (_actionCounts == 1)
                    {
                        _actionCounts += 14;
                    }
                    _actionCounts--;
                }
                if (_actionCounts == 0)
                {
                    _transparentMode = false;
                }
            }
        }

        public void OnImGuiRender()
        {
            var player = Player.MainPlayer;
            if (player == null) return;
            if (ImGui.Button("LUNARUGA"))
            {
                if (_inQuest) 
                {
                    _statusMessage = "Cannot toggle while in quest.";
                } 
                else 
                {
                    _modEnabled = !_modEnabled;
                    _statusMessage = _modEnabled ? "LuNarga enabled." : "LuNarga disabled.";
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
                ResetState();
            }

            if (!_modEnabled) return;

            var monsters = Monster.GetAllMonsters().TakeLast(5).ToArray();
            foreach (var monster in monsters)
            {
                if (monster.Type == MonsterType.Nargacuga)
                {
                    _monster = monster;
                    break; // Assumes only handling one Nargacuga at a time.
                }
            }

            if (_monster == null) return;

            ref float transparency = ref _monster.GetRef<float>(0x314);
            transparency = Clamp(transparency, 0.05f, 1.0f);

            if (!_transparentMode)
            {
                transparency = Math.Min(transparency + 0.025f, 1f);
            }
            else
            {
                transparency -= 0.025f;
            }

            if (_nargaDies)
            {
                _actionCounts = 0;
                _transparentMode = false;
                _nargaDies = false;
            }
        }
    }
}