namespace Web.User.HttpAggregator.LoadBalancer
{
    public class RoundRobin : IRoundRobin
    {
        private static int num=0;
        public async Task<string> Load(IEnumerable<string> urls)
        {
            var urlslength = urls.Count();
            if (urlslength > num)
            {
                var list = urls.ElementAt(num);
                num++;
                return list;
            }
            else
            {
                num = 0;
                return await Load(urls);
            }

        }
    }
}
