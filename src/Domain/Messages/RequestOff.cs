namespace Domain
{
    internal class RequestOff
    {
        public string SwitchName { get; set; }

        public RequestOff(string switchName)
        {
            this.SwitchName = switchName;
        }
    }
}