namespace RpiControl.Domain.Entities
{
    public class SonarConfig
    {
        public int TriggerPin { get; set; }
        public int EchoPin { get; set; }
        public double BorderDistanceCentimeters { get; set; }
    }
}