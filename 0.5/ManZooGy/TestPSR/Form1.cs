using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PenStrokeRecognizer;

namespace TestPSR
{
    public partial class Form1 : Form
    {
        private RecognizerController m_rc;

        public Form1()
        {
            InitializeComponent();

            m_rc = new RecognizerController(this);
            m_rc.Recognized += new RecognizedEventHanler(m_rc_Recognized);
        }

        void m_rc_Recognized(object sender, RecognizedEventArgs e)
        {
            textBox1.Text = e.Text;
        }
    }
}