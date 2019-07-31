using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using PaymentAPI.Entities;

namespace PaymentAPI
{
    public class NHibernateHelper
    {
        public static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
            .Database(MsSqlConfiguration.MsSql2012.ConnectionString("Server=.; Database=PaymentAPI; Trusted_Connection=true; MultipleActiveResultSets=true"))
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Payment>())
            .ExposeConfiguration(cfg =>
            {
                cfg.SetInterceptor(new SqlStatementInterceptor());
                new SchemaExport(cfg).Execute(true, false, false);
            })
            .BuildSessionFactory();
        }
        public class SqlStatementInterceptor : EmptyInterceptor
        {
            public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
            {
                Console.WriteLine(sql.ToString());
                return sql;
            }
        }
    }
}