using Laymaann.Entities.Dedicated.Message;
using Laymaann.Entities.Shared;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laymaann.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        protected readonly IOptionsMonitor<LaymaannConfig> _config;
        protected readonly ILogger<IMessageRepository> _logger;
        private string _conStr;
        public MessageRepository(IOptionsMonitor<LaymaannConfig> config, ILogger<IMessageRepository> logger)
        {
            _config = config;
            _logger = logger;
            _conStr = _config.CurrentValue.ConnectionString;
        }

        public async Task AddFeedbackMessageAsync(FeedbackMessage feedbackMessage)
        {
            using var connection = new SqlConnection(_conStr);
            var sql = "usp_AddFeedbackMessage";

            await connection.ExecuteAsync(
                sql,
                new
                {
                    feedbackMessage.UserId,
                    feedbackMessage.Origin,
                    feedbackMessage.Message,
                    feedbackMessage.UserAgent
                },
                commandType: System.Data.CommandType.StoredProcedure
            );
        }
        public async Task<bool> FeedbackMessageExistsAsync(string message)
        {
            //using var connection = new SqlConnection(_conStr);
            //var sql = "usp_CheckFeedbackMessageExists";

            //var result = await connection.QueryFirstOrDefaultAsync<int>(
            //    sql,
            //    new { UserId = userId, Origin = origin },
            //    commandType: System.Data.CommandType.StoredProcedure
            //);

            //return result > 0;
            return false;
        }
    }
}
