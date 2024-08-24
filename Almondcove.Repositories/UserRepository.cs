using Laymaann.Entities.Shared;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;

namespace Laymaann.Repositories
{
	public class UserRepository : IUserRepository
	{
		protected readonly IOptionsMonitor<LaymaannConfig> _config;
		protected readonly ILogger<IUserRepository> _logger;
		private string _conStr;
        public UserRepository(IOptionsMonitor<LaymaannConfig> config, ILogger<IUserRepository> logger)
        {
			_config = config;
			_logger = logger;
			_conStr = _config.CurrentValue.ConnectionString;
		}
        public async Task<AcUser> AddOrUpdateUser(AcUser user)
		{
			using (SqlConnection conn = new(_conStr))
			{
                SqlCommand cmd = new("usp_AddOrUpdateUser", conn);
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@GoogleId", user.GoogleId);
				cmd.Parameters.AddWithValue("@Username", user.Username ?? (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@Email", user.Email);
				cmd.Parameters.AddWithValue("@FirstName", user.FirstName ?? (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@LastName", user.LastName ?? (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@Avatar", user.ProfilePicture ?? (object)DBNull.Value);

				await conn.OpenAsync();
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new AcUser
                    {
                        Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                        GoogleId = reader.IsDBNull(reader.GetOrdinal("GoogleId")) ? null : reader.GetString(reader.GetOrdinal("GoogleId")),
                        Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                        FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                        ProfilePicture = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString(reader.GetOrdinal("Avatar")),
                        RoleId = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? 0 : reader.GetInt32(reader.GetOrdinal("RoleId")),
                    };
                }
            }

			return null;
		}



        public async Task<AcUser> GetUserByGoogleId(string googleId)
		{
			if (string.IsNullOrEmpty(googleId))
			{
				return null;
			}

			using (SqlConnection conn = new SqlConnection(_conStr))
			{
                SqlCommand cmd = new("usp_GetUserByGoogleId", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@GoogleId", googleId);

				await conn.OpenAsync();
				using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						return new AcUser
						{
							Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
							GoogleId = reader.IsDBNull(reader.GetOrdinal("GoogleId")) ? null : reader.GetString(reader.GetOrdinal("GoogleId")),
							Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
							Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
							FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
							LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
							ProfilePicture = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString(reader.GetOrdinal("Avatar")),
							RoleId = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? 0 : reader.GetInt32(reader.GetOrdinal("RoleId")),
						};
					}
				}
			}

			return null;
		}

		public async Task<List<AcUser>> GetAllUsers()
		{
			var users = new List<AcUser>();

			using (SqlConnection conn = new(_conStr))
			{
				SqlCommand cmd = new("usp_GetAllUsers", conn)
				{
					CommandType = CommandType.StoredProcedure
				};

				await conn.OpenAsync();
				using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						var user = new AcUser
						{
							Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
							GoogleId = reader.IsDBNull(reader.GetOrdinal("GoogleId")) ? null : reader.GetString(reader.GetOrdinal("GoogleId")),
							Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
							Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
							FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
							LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
							ProfilePicture = reader.IsDBNull(reader.GetOrdinal("ProfilePicture")) ? null : reader.GetString(reader.GetOrdinal("ProfilePicture")),
							RoleId = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? 0 : reader.GetInt32(reader.GetOrdinal("RoleId"))
						};

						users.Add(user);
					}
				}
			}

			return users;
		}

		public async Task<int> GetUserCountAsync()
		{
			using (var connection = new SqlConnection(_conStr))
			{
				var sql = "usp_GetUserCount";

				return await connection.ExecuteScalarAsync<int>(
					sql,
					commandType: System.Data.CommandType.StoredProcedure
				);
			}
		}

	}
}
