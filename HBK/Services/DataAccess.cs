using Autofac;
using Autofac.Integration.Mvc;
using HBK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace HBK.Services
{
    public interface IDbFactory : IDisposable
    {
        HBKDbContext Init();
    }

    public class DbFactory : Disposable, IDbFactory
    {
        private HBKDbContext dbContext;

        public HBKDbContext Init()
        {
            return dbContext ?? (dbContext = new HBKDbContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }

    public class Disposable : IDisposable
    {
        private bool isDisposed;

        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                DisposeCore();
            }

            isDisposed = true;
        }

        // Ovveride this to dispose custom objects
        protected virtual void DisposeCore()
        {
        }
    }
    public class Starter
    {
        public static void builder()
        {
            SetAutofacContainer();
        }

        private static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<DbFactory>().As<IDbFactory>().InstancePerRequest();
            builder.RegisterType<CommonService>().As<ICommonService>().InstancePerRequest();
            builder.RegisterType<MembershipService>().As<IMembershipService>().InstancePerRequest();
            builder.RegisterType<UsersPhotoService>().As<IUsersPhotoService>().InstancePerRequest();
            builder.RegisterType<CommunityService>().As<ICommunityService>().InstancePerRequest();
            builder.RegisterType<CommunityAttachmentService>().As<ICommunityAttachmentService>().InstancePerRequest();
            builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerRequest();
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}