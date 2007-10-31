using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PenStrokeRecognizer
{
    // Pen stroke recognition
    // ref: http://blog.monstuff.com/archives/000012.html

    public class RecognizerController
    {
        private Control m_control;
        private Recognizer m_recognizer = null;
        private Point m_previousPoint; // the last mouse pt in the
        private bool m_stylusDown = false;
        private Brush m_brush;
        private string m_text;

        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        public event RecognizedEventHanler Recognized;

        public RecognizerController(Control control)
        {
            m_control = control;

            m_recognizer = new AlphaRecognizer();
            m_recognizer.Initialize();

            m_brush = new SolidBrush(Color.Black);

            m_control.MouseDown += new MouseEventHandler(control_MouseDown);
            m_control.MouseMove += new MouseEventHandler(control_MouseMove);
            m_control.MouseUp += new MouseEventHandler(control_MouseUp);
        }

        private void control_MouseDown(object sender, MouseEventArgs e)
        {
            m_stylusDown = true;

            try
            {
                Point p = new Point(e.X, e.Y);
                Graphics g = m_control.CreateGraphics();
                //g.Clear(Color.White);
                //m_control.Refresh();
                g.FillEllipse(m_brush, e.X - 5, e.Y - 5, 10, 10);
                m_previousPoint = p;
                m_recognizer.AddPoint(p, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void control_MouseMove(Object sender, MouseEventArgs e)
        {
            try
            {
                if (m_stylusDown)
                {
                    Point p = new Point(e.X, e.Y);
                    Graphics g = m_control.CreateGraphics();

                    for (int i = 1; i <= 29; i++)
                    {
                        g.FillEllipse(m_brush, (p.X * i + m_previousPoint.X * (30 - i)) / 30 - 2, (p.Y * i + m_previousPoint.Y * (30 - i)) / 30 - 2, 4, 4);
                    }

                    m_previousPoint = p;
                    m_recognizer.AddPoint(p, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void control_MouseUp(Object sender, MouseEventArgs e)
        {
            m_stylusDown = false;
            Text = m_recognizer.Recognize();
            Recognized(this, new RecognizedEventArgs(Text));
        }
    }
}
