namespace ChatApp.Interfaces
{
    public class Time : ITimeDependency
    {
        public System.DateTime Current() => System.DateTime.Now;
    }
}
