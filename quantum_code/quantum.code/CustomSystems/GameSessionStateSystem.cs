using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public unsafe class GameSessionStateSystem : SystemMainThread<GameSessionStateSystem.Filter>
    {

        public struct Filter
        {
            public EntityRef Entity;
            public GameSession* GameSession;
        }

        public override void OnInit(Frame f)
        {
            Log.Info("Quantum GameSessionStateSystem::OnInit");
        }

        public override void Update(Frame f, ref Filter filter)
        {
            GameSession* gameSession = f.Unsafe.GetPointerSingleton<GameSession>();
            if(gameSession == null)
            {
                return;
            }
            // 倒计时
            gameSession->TimeUntilStart = gameSession->TimeUntilStart - f.DeltaTime;
            if(gameSession->TimeUntilStart < 1 && gameSession->State == GameState.Countdown)
            {
                Log.Debug("Quantum GameSessionStateSystem CountDown Completed");
                gameSession->State = GameState.Playing;
            }
        }

    }

}
