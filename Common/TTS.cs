using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace DPVS_DetectionTrackingSupport
{
    public class TextReading : IDisposable
    {
        private SpeechSynthesizer m_speechSynthesizer = new SpeechSynthesizer();
        public void AsyncTextSpeak(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            m_speechSynthesizer.SpeakAsync(text);
        }

        public void PauseTextSpeak()
        {
            if (m_speechSynthesizer.State == SynthesizerState.Speaking)
            {
                m_speechSynthesizer.Pause();
            }
        }

        public void ResumeTextSpeak()
        {
            if (m_speechSynthesizer.State == SynthesizerState.Paused)
            {
                m_speechSynthesizer.Resume();
            }
        }

        public void Dispose()
        {
            m_speechSynthesizer.Dispose();
        }
    }


    
}
