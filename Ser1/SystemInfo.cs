using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class SystemInfo
{
    public int m_ProcessorCount = 0;
    public PerformanceCounter PC_CPU_Load;
    public long m_PhysicalMemory = 0;
    public SystemInfo()
    {
        PC_CPU_Load = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
        PC_CPU_Load.MachineName = ".";
        PC_CPU_Load.NextValue();
        m_ProcessorCount = Environment.ProcessorCount;
        Console.WriteLine("m_ProcessorCount  " + m_ProcessorCount);
    }

}

