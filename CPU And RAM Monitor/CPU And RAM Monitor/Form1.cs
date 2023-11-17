using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPU_And_RAM_Monitor
{
    public partial class Form1 : Form
    {
        private MetricsDatabase metricsDatabase;
        private List<string[]> temporaryMetricsList; 


        public Form1()
        {
            InitializeComponent();

            InitializeProcessGridView();  
            timer1.Interval = 700;
            timer1.Start();
            button1.Click += button1_Click;
            metricsDatabase = new MetricsDatabase();
        }

        private void InitializeProcessGridView()
        {
            dataGridView1.Columns.Add("ProcessName", "Process Name");
            dataGridView1.Columns.Add("CPUUsage", "CPU Usage");
            dataGridView1.Columns.Add("MemoryUsage", "RAM Usage");
        }

        private async void UpdateProcessGridView()
        {
            Process[] processes = Process.GetProcesses();

            List<string> processNames = new List<string>();
            List<string> cpuUsages = new List<string>();
            List<string> ramUsages = new List<string>();

            foreach (var process in processes)
            {
                try
                {
                    double cpuUsage = Math.Round((process.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount) / 100, 2);
                    double ramUsage = Math.Round((double)process.WorkingSet64 / (1024 * 1024), 2);

                    string cpuUsageFormatted = cpuUsage.ToString("F1");
                    string ramUsageFormatted = ramUsage.ToString("F1");

                    processNames.Add(process.ProcessName);
                    cpuUsages.Add($"{cpuUsage:F2}%");
                    ramUsages.Add($"{ramUsage:F2} MB");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing process {process.ProcessName}: {ex.Message}");
                }
            }

            await Task.Run(() =>
            {
                BeginInvoke((Action)(() =>
                {
                    if (dataGridView1 != null)
                    {
                         
                        dataGridView1.Rows.Clear();

                        for (int i = 0; i < processNames.Count; i++)
                        {
                            dataGridView1.Rows.Add(processNames[i], cpuUsages[i], ramUsages[i]);
                        }
                    }
                }));
            });
            
            temporaryMetricsList = new List<string[]>();
            for (int i = 0; i < processNames.Count; i++)
            {
                string[] metricsArray = { processNames[i], cpuUsages[i], ramUsages[i] };
                temporaryMetricsList.Add(metricsArray);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            if (metricsDatabase == null)
            {
                metricsDatabase = new MetricsDatabase();
            }

            
            List<string[]> metricsList = metricsDatabase.GetMetrics();

            
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();

            
            string[] columnNames = { "Timestamp", "CPU Usage", "RAM Usage" };

            
            for (int i = 0; i < columnNames.Length; i++)
            {
                dataGridView2.Columns.Add(columnNames[i], columnNames[i]);
            }

            
            foreach (var metrics in metricsList)
            {
                dataGridView2.Rows.Add(metrics);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
             
            UpdateProcessGridView();

            int intCpu = (int)pCPU.NextValue();
            int intRam = (int)pRam.NextValue();

            
            lblcpu.Text = intCpu.ToString() + "%";
            lblram.Text = intRam.ToString() + "%";

            chart1.Series["CPU"].Points.AddY(intCpu);

            chart1.Series["RAM"].Points.AddY(intRam);
 
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 100;
        }
    }
}

