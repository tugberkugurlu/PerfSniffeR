namespace PerfSniffeR.Web.Models
{
    public class PerfCounterSampleCarrier
    {
        public string FullName { get; set; }
        public string MachineName { get; set; }
        public string CategoryName { get; set; }
        public string CounterName { get; set; }
        public string InstanceName { get; set; }
        public float Value { get; set; }
    }
}