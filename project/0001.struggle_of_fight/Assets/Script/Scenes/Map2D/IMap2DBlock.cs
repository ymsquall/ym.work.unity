namespace Assets.Script.Scenes.Map2D
{
    public enum Map2DBlockType : byte
    {
        none,
        texture,
        physics,
        custom,
    }
    public interface IMap2DBlock
    {
        Map2DBlockType BType { get; }
    }
}