using UnityEngine;

public class AudioListener : MonoBehaviour
{
    private const int FFTSize = 1024;
    private float[] samples;
    private Complex[] complexSamples;
    private string selectedDevice;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        if (Microphone.devices.Length > 0)
            selectedDevice = Microphone.devices[0];

        //StartRecording();
    }

    public void StopRecording()
    {
        Microphone.End(selectedDevice);
    }
    public void StartRecording()
    {
        if (selectedDevice != null)
        {
            audioSource.clip = Microphone.Start(selectedDevice, true, 1, 44100);
            //audioSource.Play();
            audioSource.loop = true;

            samples = new float[FFTSize];
            complexSamples = new Complex[FFTSize];
        }
    }

    void Update()
    {
        if (Microphone.IsRecording(selectedDevice))
        {
            int micPosition = Microphone.GetPosition(null) - FFTSize;

            if (micPosition < 0)
                return;

            audioSource.clip.GetData(samples, micPosition);

            // Convert to complex numbers
            for (int i = 0; i < FFTSize; i++)
            {
                complexSamples[i] = new Complex(samples[i], 0.0f);
            }

            // Perform FFT
            FFT(complexSamples);

            // Get the dominant frequency
            int pitch = GetDominantFrequency();
            Debug.Log("Detected Pitch: " + pitch);
        }
    }

    void FFT(Complex[] buffer)
    {
        int n = buffer.Length;
        if (n <= 1) return;

        var even = new Complex[n / 2];
        var odd = new Complex[n / 2];
        for (int i = 0; i < n / 2; i++)
        {
            even[i] = buffer[i * 2];
            odd[i] = buffer[i * 2 + 1];
        }

        FFT(even);
        FFT(odd);

        for (int k = 0; k < n / 2; k++)
        {
            Complex t = Complex.Exp(-2.0f * Mathf.PI * k / n) * odd[k];
            buffer[k] = even[k] + t;
            buffer[k + n / 2] = even[k] - t;
        }
    }

    int GetDominantFrequency()
    {
        float freqPerBin = 44100 / FFTSize;
        int maxIndex = 0;
        float maxMagnitude = 0;

        for (int i = 0; i < complexSamples.Length; i++)
        {
            float magnitude = complexSamples[i].Magnitude;
            float frequency = i * freqPerBin;

            if (magnitude > maxMagnitude && frequency <= 10000)
            {
                maxMagnitude = magnitude;
                maxIndex = i;
            }
        }

        int maxHz = (int)Mathf.Round(maxIndex * freqPerBin);
        return maxHz;
    }

    public struct Complex
    {
        public float real;
        public float imag;

        public Complex(float r, float i)
        {
            real = r;
            imag = i;
        }

        public float Magnitude => Mathf.Sqrt(real * real + imag * imag);

        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.real + b.real, a.imag + b.imag);
        }

        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.real - b.real, a.imag - b.imag);
        }

        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.real * b.real - a.imag * b.imag, a.real * b.imag + a.imag * b.real);
        }

        public static Complex Exp(float arg)
        {
            return new Complex(Mathf.Cos(arg), Mathf.Sin(arg));
        }
    }
}
