using AutoMapper;
using TaskManagerBLL.Models;
using TaskMng.Models;

namespace TaskMng.Mapping
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<PersonBLL, PersonView>();
        }
    }

    public class StatusProfile : Profile
    {
        public StatusProfile()
        {
            CreateMap<StatusBLL, StatusView>();
        }
    }

    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<TaskBLL, TaskView>();
        }
    }

    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<TeamBLL, TeamView>();
        }
    }

    
}