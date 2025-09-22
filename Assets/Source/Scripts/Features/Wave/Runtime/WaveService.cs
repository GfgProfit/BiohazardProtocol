public sealed class WaveService
{
    public int WaveIndex { get; private set; } = 0;

    public int CurrentBaseHP => (int)(100 * System.Math.Pow(1.1f, WaveIndex - 1));

    public void NextWave() => WaveIndex++;
    public void ResetWaves() => WaveIndex = 1;
}