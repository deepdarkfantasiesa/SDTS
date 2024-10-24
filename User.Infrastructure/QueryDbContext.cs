using Microsoft.EntityFrameworkCore;
using User.Domain.AggregatesModel.UserAggregate;
using User.Infrastructure.EntityConfigurations;

namespace User.Infrastructure
{
	/// <summary>
	/// 查询专用上下文
	/// </summary>
	public class QueryDbContext : DbContext
	{
		#region DbSets

		/// <summary>
		/// 用户表
		/// </summary>
		public DbSet<Users> Users { get; set; }

		#endregion

		/// <summary>
		/// 查询专用上下文
		/// </summary>
		/// <param name="options"></param>
		public QueryDbContext(DbContextOptions<QueryDbContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
			base.OnModelCreating(modelBuilder);
		}
	}
}
