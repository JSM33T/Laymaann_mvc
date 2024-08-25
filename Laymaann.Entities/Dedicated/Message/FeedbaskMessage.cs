namespace Laymaann.Entities.Dedicated.Message
{
	public class FeedbackMessage
	{
        public string UserId { get; set; }
        public string Origin { get; set; }
		public string Message { get; set; }
        public string UserAgent { get; set; }
    }
    public class AddFeedbackMessage
    {
        public string Origin { get; set; }
        public string Message { get; set; }
    }
}
