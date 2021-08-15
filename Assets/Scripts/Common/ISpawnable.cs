namespace CustomInterfaces
{
    public interface ISpawnable
    {
        void RespawnAtTop();
        SpawnLimit CalculateSpawnLimits();
    }
}
