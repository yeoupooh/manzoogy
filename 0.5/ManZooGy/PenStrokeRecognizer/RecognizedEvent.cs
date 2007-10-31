using System;
using System.Collections.Generic;
using System.Text;

namespace PenStrokeRecognizer
{
    public class RecognizedEventArgs: EventArgs
    {
        private string m_text;

        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        public RecognizedEventArgs(string text)
        {
            m_text = text;
        }
    }

    public delegate void RecognizedEventHanler(object sender, RecognizedEventArgs e);
}
