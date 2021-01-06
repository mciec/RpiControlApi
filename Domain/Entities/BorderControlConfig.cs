namespace RpiControl.Domain.Entities
{
    public class BorderControlConfig
    {
        public int ServoHatI2c { get; set; }
        public int DelayBeforeOpeningMiliseconds { get; set; }
        public ServoConfig ServoConfig { get; set; }
    }
}