namespace MathLiberator.Analysis
{
    public interface IStepper
    {
        void Start();
        bool Step();
    }
}