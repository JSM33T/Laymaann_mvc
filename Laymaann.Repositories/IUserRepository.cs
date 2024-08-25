using Laymaann.Entities.Shared;

namespace Laymaann.Repositories
{
	public interface IUserRepository
	{
		public Task<AcUser> AddOrUpdateUser(AcUser user);
		public Task<AcUser> GetUserByGoogleId(string googleId);
		public Task<List<AcUser>> GetAllUsers();
		public Task<int> GetUserCountAsync();
	}
}
