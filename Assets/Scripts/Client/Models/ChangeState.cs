namespace Assets.Scripts.Client.Models
{
    public enum ChangeState : byte
    {
        Spawn = 1 << 0,
        Moved = 1 << 1,
        Damaged = 1 << 2,
        Died = 1 << 3,
        Attack = 1 << 4,
    }
}
