using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StatisticsLibrary;

namespace Examples
{
    public partial class Form1 : Form
    {
        double[] data = Analyzer.UniformDistribution(new Random(5), 1000);
        Analyzer analyzer;

        public Form1()
        {
            InitializeComponent();

            analyzer = new Analyzer(data);
        }

        // 1-й квантиль
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Первый квантиль: "+analyzer.FirstQuartile());
        }

        // 3-й квантиль
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Третий квантиль: " + analyzer.ThirdQuartile());
        }

        // Среднее
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Среднее: " + analyzer.Mean());
        }

        // Медиана
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Среднее: " + analyzer.Median());
        }
    }
}
