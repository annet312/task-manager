using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerBLL.Models;
using TaskManagerDAL.Entities;

namespace TaskManagerBLL.Infrastructure
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<Person, PersonBLL>();
        }
    }

    public class StatusProfile : Profile
    {
        public StatusProfile()
        {
            CreateMap<Status, StatusBLL>();
        }
    }

    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<_Task, TaskBLL>();
        }
    }

    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<Team, TeamBLL>();
        }
    }

    public class TaskTemplateProfile : Profile
    {
        public TaskTemplateProfile()
        {
            CreateMap<TaskTemplate, TaskTemplateBLL>();
        }
    }
}
