using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;
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
        private bool _transparentMode = false;
        private Monster? _monster;
        public void OnMonsterDestroy(Monster monster) { _monster = null; }
        public void OnMonsterDeath(Monster monster) { _monster = null; }
        public void OnMonsterEnrage(Monster monster)
        {
            if (_monster == null) return;
            if (_monster.Type == MonsterType.Nargacuga) { _actionCounts = 22; }
        }
        public void OnMonsterUnenrage(Monster monster)
        {
            if (_monster == null) return;
            if (_monster.Type == MonsterType.Nargacuga) { _actionCounts = 0; }
        }

        public void OnMonsterAction(Monster monster, ref int actionId)
        {
            if (_monster == null) return;
            if (_monster.Type == MonsterType.Nargacuga && _actionCounts > 0)  
            {
                if (_actionCounts == 20) { _transparentMode = true; }
                if (_actionCounts == 16) { _transparentMode = false; }
                if (_actionCounts == 13) { _transparentMode = true; }
                if (_actionCounts == 10) { _transparentMode = false; }
                if (_actionCounts == 7) { _transparentMode = true; }
                if (_actionCounts == 4) { _transparentMode = false; }
                if (_actionCounts == 1) { _actionCounts += 21; }
                _actionCounts -= 1;
            }
            if (_monster.Type == MonsterType.Nargacuga && _actionCounts == 0)
            {
                _transparentMode = false;
            }
        }

        public void OnUpdate(float deltaTime)
        {
            var player = Player.MainPlayer;
            if (player == null) return;
            var monsters = Monster.GetAllMonsters().TakeLast(5).ToArray();
            foreach (var monster in monsters)
            {
                if (monster.Type == MonsterType.Nargacuga)
                {
                    _monster = monster;
                }
            }
            if (_monster == null)
                return;

            ref float transparency = ref _monster.GetRef<float>(0x314);

            if (transparency < 0.01f) { transparency = 0.01f; }
            else if (transparency > 1.0f) { transparency = 1.0f; }


            if (!_transparentMode) {
                if (transparency < 1f) { transparency += 0.005f; }
                else { transparency = 1f; }
            } else { 
                transparency -= 0.005f; 
            }
        }
    }
}