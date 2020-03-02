namespace Domain
{
    internal class RequestOn
    {
        public string SwitchName { get; set; }

        public RequestOn(string switchName)
        {
            this.SwitchName = switchName;
        }
    }
}