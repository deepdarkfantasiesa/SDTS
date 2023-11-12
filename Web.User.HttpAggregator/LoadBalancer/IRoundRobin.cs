namespace Web.User.HttpAggregator.LoadBalancer
{
    public interface IRoundRobin
    {
        public Task<string> Load(IEnumerable<string> urls);
    }
}
