using NAudio.Dsp;

namespace NAudioWpfDemo
{
    public interface ISpectrumAnalyser
    {
        void Update(Complex[] result);
    }
}