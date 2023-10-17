namespace User.API.Application.IntegrationEvents
{
    public class CreateUserIntegrationEvent
    {
        public CreateUserIntegrationEvent(string userName)
        {
            UserName = userName;
        }
        public string UserName { get; }
    }
}
