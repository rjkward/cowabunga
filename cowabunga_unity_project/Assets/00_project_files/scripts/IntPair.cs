public struct IntPair
{
    public readonly int I;
    public readonly int J;
    public IntPair(int i, int j)
    {
        I = i;
        J = j;
    }

    public bool IsEqual(IntPair other)
    {
        return other.I == I && other.J == J;
    }
}