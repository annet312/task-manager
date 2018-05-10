using NUnit.Framework;
using TaskManagerBLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TaskManagerDAL.Repositories;

namespace TaskManagerBLL.Services.Tests
{
    [TestFixture()]
    public class TaskServiceTests
    {
        private const string ConnectionString = "C:\\Users\\Anna\\Documents\\LabsEpam\\Project\\TaskMng\\App_Data\\TaskMng.mdf";

        [Test()]
        public void TaskServiceTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void createTeamTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetAllTeamsTest()
        {
            //var unitOfWork = new UnitOfWork(ConnectionString);
            //var service = new TaskService(unitOfWork);
            //var teams = service.GetAllTeams();
            Assert.Fail();
        }
    }
}