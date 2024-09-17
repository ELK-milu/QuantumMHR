namespace Quantum
{
    public unsafe class PlayerDisconnectSystem : SystemSignalsOnly, ISignalOnPlayerDisconnected
    {
        public void OnPlayerDisconnected(Frame f, PlayerRef player)
        {
            Log.Info($"Player {player} has disconnected!");
            foreach (var playerLink in f.GetComponentIterator<PlayerLink>())
            {
                if (playerLink.Component.PlayerRef == player)
                {
                    continue;
                }
                Log.Debug("Quantum: OnPlayerDisconnected player destoryed");
                f.Destroy(playerLink.Entity);
            }
        }
    }
}