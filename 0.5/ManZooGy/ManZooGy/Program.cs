﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RBP.IPY;

namespace ManZooGy
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Launcher.Start(args);
        }
    }
}