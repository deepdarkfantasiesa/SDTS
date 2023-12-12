using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace User.Infrastructure.Interceptors
{
    internal class MasterSlaveShiftInterceptor: DbCommandInterceptor
    {
        private string _masterConnectionString;
        private string _slaveConnectionString;
        public MasterSlaveShiftInterceptor(string master,string slave)
        {
            Console.WriteLine(master);
            Console.WriteLine(slave);
            _masterConnectionString = master;
            _slaveConnectionString = slave;
        }

        private string GetSlaveConnectionString()
        {
            var readArr = _slaveConnectionString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var resultConn = string.Empty;
            if (readArr != null && readArr.Any())
            {
                resultConn = readArr[Convert.ToInt32(Math.Floor((double)new Random().Next(0, readArr.Length)))];
            }
            return resultConn;
        }

        private void UpdateToSlave(DbCommand command)
        {
            //判断是否配置了主从分离
            if (!string.IsNullOrWhiteSpace(GetSlaveConnectionString()))//如果配置了读写分离，就进入判断
            {
                //判断是否为插入语句(EF 插入语句会通过Reader执行并查询主键),否则进入
                if (command.CommandText.ToLower().StartsWith("insert", StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    // 判断当前会话是否处于分布式事务中
                    bool isDistributedTran = Transaction.Current != null &&
                                             Transaction.Current.TransactionInformation.Status !=
                                             TransactionStatus.Committed;
                    //判断该 context 是否处于普通数据库事务中
                    bool isDbTran = command.Transaction != null;
                    //如果不处于事务中,则执行从服务器查询
                    if (!isDbTran && !isDistributedTran)
                    {
                        command.Connection.Close();
                        command.Connection.ConnectionString = GetSlaveConnectionString();
                        command.Connection.Open();

                    }

                }
            }
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            //this.UpdateToSlave(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            //this.UpdateToSlave(command);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }


        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            //this.UpdateToSlave(command);
            return base.ScalarExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            //this.UpdateToSlave(command);
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }
    }
}
