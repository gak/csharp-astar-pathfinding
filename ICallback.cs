namespace Slowchop
{
    public interface ICallback<T>
    {
        float ApproximateDistance(T src, T dst);
    }
}
