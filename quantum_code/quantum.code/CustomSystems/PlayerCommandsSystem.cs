using Quantum.CustomState.Commands;

namespace Quantum
{
    public class PlayerCommandsSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            // command
            for (int i = 0; i < f.PlayerCount; i++)
            {
                var commandResetPos = f.GetPlayerCommand(i) as CommandResetPos;
                commandResetPos?.Execute(f);
                var commandPlayerAttributeCost = f.GetPlayerCommand(i) as CommandPlayerAttributeCost;
                commandPlayerAttributeCost?.Execute(f);
                var commandRespawnEntity = f.GetPlayerCommand(i) as CommandRespawnEntity;
                commandRespawnEntity?.Execute(f);
                var commandSetWire = f.GetPlayerCommand(i) as CommandSetWire;
                commandSetWire?.Execute(f);
                var commandPlayerExhaust = f.GetPlayerCommand(i) as CommandPlayerExhaust;
                commandPlayerExhaust?.Execute(f);
                var commandPlayerJump = f.GetPlayerCommand(i) as CommandPlayerJump;
                commandPlayerJump?.Execute(f);
                var commandPlayerStateSync = f.GetPlayerCommand(i) as CommandPlayerStateSync;
                commandPlayerStateSync?.Execute(f);
            }
        }
    }
}