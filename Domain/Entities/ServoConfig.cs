namespace RpiControl.Domain.Entities
{
    public class ServoConfig
    {
        public int ChannelNumber { get; set; }
        public int MaximumAngle { get; set; }
        public int MinimumPulseWidthMicroseconds { get; set; }
        public int MaximumPulseWidthMicroseconds { get; set; }
    }
}