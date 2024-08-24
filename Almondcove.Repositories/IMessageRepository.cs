using Laymaann.Entities.Dedicated.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laymaann.Repositories
{
    public interface IMessageRepository
    {
        public Task AddFeedbackMessageAsync(FeedbackMessage feedbackMessage);
        public Task<bool> FeedbackMessageExistsAsync(string Message);

    }
}
